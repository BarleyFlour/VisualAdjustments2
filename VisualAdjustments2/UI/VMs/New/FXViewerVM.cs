using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class FXViewerVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public ReactiveProperty<BuffListViewVM> m_AllFX = new ReactiveProperty<BuffListViewVM>();
        public ReactiveProperty<BuffListViewVM> m_CurrentFX = new ReactiveProperty<BuffListViewVM>();
        public ReactiveProperty<UnitDescriptor> UnitDescriptor;
        public FXViewerVM(UnitEntityData data)
        {
            base.AddDisposable(this);
            var allBuffs = new ReactiveCollection<BuffButtonVM>();
            foreach (var buff in ResourceLoader.AllFXs)
            {
                allBuffs.Add(new BuffButtonVM(ResourcesLibrary.TryGetBlueprint<BlueprintUnitFact>(buff.GUID), true, this.AddListItem));
            }
            ReactiveCollection<BuffButtonVM> reactive = new ReactiveCollection<BuffButtonVM>();
            /*foreach (var kv in ResourceLoader.AllEEs)
            {
                reactive.Add(new ListViewItemVM(kv, true, AddListItem));
            }*/
            // var CurrentReactive = new ReactiveCollection<ListViewItemVM>();
            foreach (var buff in data.GetSettings().Fx_Settings.fXBlockerHolder.FXBlockers)
            {
                reactive.Add(new BuffButtonVM(ResourcesLibrary.TryGetBlueprint<BlueprintUnitFact>(buff.AbilityGUID), false, this.RemoveListItem));
            }
            /*foreach (var ee in Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.CharacterAvatar.EquipmentEntities)
            {
                var inf = ee.ToEEInfo();
                if (inf != null && !CurrentReactive.Any(a => a.Guid == inf.Value.GUID))
                {
                    CurrentReactive.Add(new ListViewItemVM(inf.Value, false, RemoveListItem));
                }
            }*/
            this.UnitDescriptor = Game.Instance.SelectionCharacter.SelectedUnit;
            base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(delegate (UnitDescriptor _)
            {
                // this.OnUnitChanged();
            }));
            base.AddDisposable(m_AllFX.Value = new BuffListViewVM(allBuffs, new ReactiveProperty<BuffButtonVM>(null)));
            base.AddDisposable(m_CurrentFX.Value = new BuffListViewVM(reactive, new ReactiveProperty<BuffButtonVM>(null)));

        }
        public override void DisposeImplementation()
        {

        }
        public void RemoveListItem(BuffButtonVM item)
        {
            try
            {
                if (this.m_CurrentFX?.Value?.EntitiesCollection?.Contains(item) == true)
                {
                    this.m_CurrentFX?.Value?.EntitiesCollection.Remove(item);
                    var settings = this.UnitDescriptor.Value.Unit.GetSettings();
                   // if (settings.Fx_Settings.fXBlockerHolder.FXBlockers.Any(a => a.AbilityGUID == item.Feature.AssetGuidThreadSafe))
                    {
                        settings.Fx_Settings.fXBlockerHolder.FXBlockers.Remove(ResourceLoader.AbilityGuidToFXBlocker[item.Feature.AssetGuidThreadSafe]);
                        settings.Fx_Settings.fXBlockerHolder.Recache();
                    }

                }
            }
            catch (Exception e)
            {

                Main.Logger.Error(e.ToString());
            }
        }
        public void AddListItem(BuffButtonVM item)
        {
            try
            {
                if (!this.m_CurrentFX?.Value?.EntitiesCollection.Any(a => a.Feature.AssetGuid == item.Feature.AssetGuid) == true) this.m_CurrentFX?.Value.EntitiesCollection.Add(new BuffButtonVM(item.Feature, false, RemoveListItem));
                var settings = this.UnitDescriptor.Value.Unit.GetSettings();
               // if (!settings.Fx_Settings.fXBlockerHolder.FXBlockers.Any(a => a.AbilityGUID == item.Feature.AssetGuidThreadSafe))
                {
                    settings.Fx_Settings.fXBlockerHolder.FXBlockers.Add(ResourceLoader.AbilityGuidToFXBlocker[item.Feature.AssetGuidThreadSafe]);
                    settings.Fx_Settings.fXBlockerHolder.Recache();
                }
                //Main.Logger.Log(ResourcesLibrary.TryGetResource<EquipmentEntity>(item.Guid).ToString());
                /*if(Game.Instance.UI.Common.DollRoom.m_Avatar.EquipmentEntities.Any(a => a.name == item.InternalName))*/
                // Game.Instance.UI.Common.DollRoom.m_Avatar.AddEquipmentEntity(ResourcesLibrary.TryGetResource<EquipmentEntity>(item.Guid));
                // if (!this.applyActions.ContainsKey(item.Guid)) this.applyActions.Add(item.Guid, new AddEE(item.Guid));
            }
            catch (Exception e)
            {

                Main.Logger.Error(e.ToString());
            }
        }
    }
}
