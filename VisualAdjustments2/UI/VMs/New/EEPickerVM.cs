using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.BundlesLoading;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Kingmaker.Visual.CharacterSystem;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using VisualAdjustments2.Infrastructure;
using Owlcat.Runtime.UniRx;

namespace VisualAdjustments2.UI
{
    [HarmonyLib.HarmonyPatch(typeof(Character), nameof(Character.CopyEquipmentFrom))]
    public static class Character_CopyEquipmentFrom_Patch
    {
        //Prevents equipment from being reset in the preview
        public static bool Prefix(Character __instance)
        {
            if (ServiceWindowsVM_ShowWindow_Patch.swPCView?.m_EEPickerPCView?.ViewModel != null && __instance.EquipmentEntities?.Count != 0)
            {
                return false;
            }
            else return true;
        }
    }
    public class EEPickerVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public Dictionary<string, EEApplyAction> applyActions = new Dictionary<string, EEApplyAction>();

        public void ResetChanges()
        {
            this.applyActions.Clear();
            var CurrentReactive = new ReactiveCollection<ListViewItemVM>();
            foreach (var ee in Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.CharacterAvatar.EquipmentEntities)
            {
                //Main.Logger.Log(ee.name);
                var inf = ee.ToEEInfo();
                if (inf != null && !CurrentReactive.Any(a => a.Guid == inf.Value.GUID))
                {
                    CurrentReactive.Add(new ListViewItemVM(inf.Value, false, RemoveListItem,true));
                }
            }
            CurrentEEs.Value?.Dispose();
            base.AddDisposable(CurrentEEs.Value = new ListViewVM(CurrentReactive, new ReactiveProperty<ListViewItemVM>(CurrentReactive.FirstOrDefault())));
        }

        public ReactiveProperty<UnitDescriptor> UnitDescriptor;
        public ReactiveProperty<ListViewVM> AllEEs = new ReactiveProperty<ListViewVM>();
        public ReactiveProperty<ListViewVM> CurrentEEs = new ReactiveProperty<ListViewVM>();
        public EEPickerVM(UnitEntityData data)
        {
            base.AddDisposable(this);
            ReactiveCollection<ListViewItemVM> reactive = new ReactiveCollection<ListViewItemVM>();
            foreach (var kv in ResourceLoader.AllEEs)
            {
                reactive.Add(new ListViewItemVM(kv, true, AddListItem,true));
            }
            var CurrentReactive = new ReactiveCollection<ListViewItemVM>();
            foreach (var ee in Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.CharacterAvatar.EquipmentEntities)
            {
                //Main.Logger.Log(ee.name);
                var inf = ee.ToEEInfo();
                if (inf != null && !CurrentReactive.Any(a => a.Guid == inf.Value.GUID))
                {
                    CurrentReactive.Add(new ListViewItemVM(inf.Value, false, RemoveListItem,true));
                }
            }
            this.UnitDescriptor = Game.Instance.SelectionCharacter.SelectedUnit;
            base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(delegate (UnitDescriptor _)
            {
                this.OnUnitChanged();
            }));
            base.AddDisposable(AllEEs.Value = new ListViewVM(reactive, new ReactiveProperty<ListViewItemVM>(null)));
            base.AddDisposable(CurrentEEs.Value = new ListViewVM(CurrentReactive, new ReactiveProperty<ListViewItemVM>(CurrentReactive.FirstOrDefault())));
            

            //CurrentEEs = new ListViewVM();
        }
        private void OnUnitChanged()
        {
            this.applyActions.Clear();
            var CurrentReactive = new ReactiveCollection<ListViewItemVM>();
            foreach (var ee in Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.CharacterAvatar.EquipmentEntities)
            {
                //Main.Logger.Log(ee.name);
                var inf = ee.ToEEInfo();
                if (inf != null && !CurrentReactive.Any(a => a.Guid == inf.Value.GUID))
                {
                    CurrentReactive.Add(new ListViewItemVM(inf.Value, false, RemoveListItem,true));
                }
            }
            CurrentEEs.Value?.Dispose();
            base.AddDisposable(CurrentEEs.Value = new ListViewVM(CurrentReactive, new ReactiveProperty<ListViewItemVM>(CurrentReactive.FirstOrDefault())));
        }
        public void RemoveListItem(ListViewItemVM item)
        {
            try
            {
                if (this.CurrentEEs?.Value?.EntitiesCollection?.Contains(item) == true)
                {

                    this.CurrentEEs?.Value?.EntitiesCollection?.Remove(item);
                    if(Game.Instance.UI.Common.DollRoom.m_Avatar.EquipmentEntities.Any(a => a.name == item.InternalName)) Game.Instance.UI.Common.DollRoom.m_Avatar.RemoveEquipmentEntity(ResourcesLibrary.TryGetResource<EquipmentEntity>(item.Guid));
                    if(this.applyActions.ContainsKey(item.Guid))
                    {
                        this.applyActions.Remove(item.Guid);
                    }
                    else if (!this.applyActions.ContainsKey(item.Guid)) this.applyActions.Add(item.Guid, new RemoveEE(item.Guid));
                }
            }
            catch (Exception e)
            {

                Main.Logger.Error(e.ToString());
            }
        }
        public void AddListItem(ListViewItemVM item)
        {
            try
            {
                if (!this.CurrentEEs?.Value?.EntitiesCollection.Any(a => a.Guid == item.Guid) == true) this.CurrentEEs?.Value.EntitiesCollection.Add(new ListViewItemVM(item, false, RemoveListItem,true));
               // Main.Logger.Log(ResourcesLibrary.TryGetResource<EquipmentEntity>(item.Guid).ToString());
                if(!Game.Instance.UI.Common.DollRoom.m_Avatar.EquipmentEntities.Any(a => a.name == item.InternalName))
                Game.Instance.UI.Common.DollRoom.m_Avatar.AddEquipmentEntity(ResourcesLibrary.TryGetResource<EquipmentEntity>(item.Guid));
                if (!this.applyActions.ContainsKey(item.Guid)) this.applyActions.Add(item.Guid, new AddEE(item.Guid));
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public void ApplyColor(Color col, bool PrimOrSec)
        {
#if DEBUG
            Main.Logger.Log("TriedApply");
#endif
            try
            {
                if (this.applyActions.TryGetValue(this.CurrentEEs.Value.SelectedEntity.Value.Guid, out EEApplyAction val) && val.GetType() == typeof(AddEE))
                {
                    var loaded = ResourcesLibrary.TryGetResource<EquipmentEntity>(this.CurrentEEs.Value.SelectedEntity.Value.Guid);
                    var addee = (AddEE)val;
                    if (PrimOrSec && loaded.PrimaryColorsProfile?.Ramps?.Count > 0)
                    {
                        //Main.Logger.Log("Prim");
                        var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                        ColInf.CustomColor = true;
                        ColInf.CustomColorRGB = new SerializableColor(col);
                        addee.PrimaryCol = ColInf;
                        ColInf.Apply(loaded, Game.Instance.UI.Common.DollRoom.m_Avatar);
                        //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;
                    }
                    else if(loaded.SecondaryColorsProfile?.Ramps?.Count > 0)
                    {
                        //Main.Logger.Log("Sec");
                        var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                        ColInf.CustomColor = true;
                        ColInf.CustomColorRGB = new SerializableColor(col);
                        addee.SecondaryCol = ColInf;
                        ColInf.Apply(loaded, Game.Instance.UI.Common.DollRoom.m_Avatar);
                        //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;
                    }
                }
                else
                {
                    var ee = this.UnitDescriptor.Value.Unit.GetSettings().EeSettings.EEs.FirstOrDefault(a => a.GUID == this.CurrentEEs.Value.SelectedEntity.Value.Guid);
                    var loaded = ResourcesLibrary.TryGetResource<EquipmentEntity>(this.CurrentEEs.Value.SelectedEntity.Value.Guid);
                    if (ee != null)
                    {
                        var addee = ee;
                        if (PrimOrSec && loaded.PrimaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = true;
                            ColInf.CustomColorRGB = new SerializableColor(col);
                            addee.Primary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = ColInf;
                            addaction.SecondaryCol = addee.Secondary;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                        else if(loaded.SecondaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = true;
                            ColInf.CustomColorRGB = new SerializableColor(col);
                            addee.Secondary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = addee.Primary;
                            addaction.SecondaryCol = ColInf;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                    }
                    else
                    {
                        var addee = new EE_Applier(this.CurrentEEs.Value.SelectedEntity.Value.Guid, EE_Applier.ActionType.Add);
                        if (PrimOrSec && loaded.PrimaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = true;
                            ColInf.CustomColorRGB = new SerializableColor(col);
                            addee.Primary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = ColInf;
                            addaction.SecondaryCol = addee.Secondary;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                        else if(loaded.SecondaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = true;
                            ColInf.CustomColorRGB = new SerializableColor(col);
                            addee.Secondary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = addee.Primary;
                            addaction.SecondaryCol = ColInf;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public void ApplyColor(int rampIndex, bool PrimOrSec)
        {
#if DEBUG
            Main.Logger.Log("TriedApply");
#endif
            try
            {
                if (this.applyActions.TryGetValue(this.CurrentEEs.Value.SelectedEntity.Value.Guid, out EEApplyAction val) && val.GetType() == typeof(AddEE))
                {
                    var loaded = ResourcesLibrary.TryGetResource<EquipmentEntity>(this.CurrentEEs.Value.SelectedEntity.Value.Guid);
                    var addee = (AddEE)val;
                    if (PrimOrSec && loaded.PrimaryColorsProfile?.Ramps?.Count > 0)
                    {
                        //Main.Logger.Log("Prim");
                        var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                        ColInf.CustomColor = false;
                        ColInf.Index = rampIndex;
                        addee.PrimaryCol = ColInf;
                        ColInf.Apply(loaded, Game.Instance.UI.Common.DollRoom.m_Avatar);
                        //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;
                    }
                    else if (loaded.SecondaryColorsProfile?.Ramps?.Count > 0)
                    {
                        //Main.Logger.Log("Sec");
                        var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                        ColInf.CustomColor = false;
                        ColInf.Index = rampIndex;
                        addee.SecondaryCol = ColInf;
                        ColInf.Apply(loaded, Game.Instance.UI.Common.DollRoom.m_Avatar);
                        //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;
                    }
                }
                else
                {
                    var ee = this.UnitDescriptor.Value.Unit.GetSettings().EeSettings.EEs.FirstOrDefault(a => a.GUID == this.CurrentEEs.Value.SelectedEntity.Value.Guid);
                    var loaded = ResourcesLibrary.TryGetResource<EquipmentEntity>(this.CurrentEEs.Value.SelectedEntity.Value.Guid);
                    if (ee != null)
                    {
                        var addee = ee;
                        if (PrimOrSec && loaded.PrimaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = false;
                            ColInf.Index = rampIndex;
                            addee.Primary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = ColInf;
                            addaction.SecondaryCol = addee.Secondary;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                        else if (loaded.SecondaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = false;
                            ColInf.Index = rampIndex;
                            addee.Secondary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = addee.Primary;
                            addaction.SecondaryCol = ColInf;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                    }
                    else
                    {
                        var addee = new EE_Applier(this.CurrentEEs.Value.SelectedEntity.Value.Guid, EE_Applier.ActionType.Add);
                        if (PrimOrSec && loaded.PrimaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = false;
                            ColInf.Index = rampIndex;
                            addee.Primary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = ColInf;
                            addaction.SecondaryCol = addee.Secondary;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                        else if (loaded.SecondaryColorsProfile?.Ramps?.Count > 0)
                        {
                            var ColInf = new EE_Applier.ColorInfo(PrimOrSec);
                            ColInf.CustomColor = false;
                            ColInf.Index = rampIndex;
                            addee.Secondary = ColInf;
                            addee.Apply(Game.Instance.UI.Common.DollRoom.m_Avatar);
                            //Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;

                            var addaction = new AddEE(addee.GUID);
                            addaction.PrimaryCol = addee.Primary;
                            addaction.SecondaryCol = ColInf;
                            this.applyActions.Add(addaction.GUID, addaction);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public override void DisposeImplementation()
        {

        }
    }
}
