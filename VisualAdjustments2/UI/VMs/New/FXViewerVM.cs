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
        public IReactiveProperty<UnitReference> UnitDescriptor;
        public FXViewerVM(UnitEntityData data)
        {
            base.AddDisposable(this);
            var allBuffs = new ReactiveCollection<BuffButtonVM>();
            foreach (var buff in ResourceLoader.AllFXs)
            {
                allBuffs.Add(new BuffButtonVM(ResourcesLibrary.TryGetBlueprint<BlueprintUnitFact>(buff.GUID), true, this.AddListItem));
            }
            ReactiveCollection<BuffButtonVM> reactive = new ReactiveCollection<BuffButtonVM>();
            foreach (var buff in data.GetSettings().BuffSettings.fXBlockerHolder.FXBlockers)
            {
                reactive.Add(new BuffButtonVM(ResourcesLibrary.TryGetBlueprint<BlueprintUnitFact>(buff.AbilityGUID), false, this.RemoveListItem));
            }
            this.UnitDescriptor = Game.Instance.SelectionCharacter.SelectedUnit;
            base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(delegate (UnitReference _)
            {
                this.OnUnitChanged();
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
                    var settings = this.UnitDescriptor.Value.Value.GetSettings();
                    // if (settings.Fx_Settings.fXBlockerHolder.FXBlockers.Any(a => a.AbilityGUID == item.Feature.AssetGuidThreadSafe))
                    {
                        settings.BuffSettings.fXBlockerHolder.FXBlockers.Remove(ResourceLoader.AbilityGuidToFXBlocker[item.Feature.AssetGuidThreadSafe]);
                        settings.BuffSettings.fXBlockerHolder.Recache();
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
                var settings = this.UnitDescriptor.Value.Value.GetSettings();
                // if (!settings.Fx_Settings.fXBlockerHolder.FXBlockers.Any(a => a.AbilityGUID == item.Feature.AssetGuidThreadSafe))
                {
                    settings.BuffSettings.fXBlockerHolder.FXBlockers.Add(ResourceLoader.AbilityGuidToFXBlocker[item.Feature.AssetGuidThreadSafe]);
                    settings.BuffSettings.fXBlockerHolder.Recache();
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
        public void OnUnitChanged()
        {
            {
                var CurrentReactive = new ReactiveCollection<BuffButtonVM>();
                foreach (var buff in Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().BuffSettings.fXBlockerHolder.FXBlockers)
                {
                    CurrentReactive.Add(new BuffButtonVM(ResourcesLibrary.TryGetBlueprint<BlueprintUnitFact>(buff.AbilityGUID), false, this.RemoveListItem));
                }
                m_CurrentFX.Value?.Dispose();
                base.AddDisposable(m_CurrentFX.Value = new BuffListViewVM(CurrentReactive, new ReactiveProperty<BuffButtonVM>(CurrentReactive.FirstOrDefault())));
            }
            if (this.UnitDescriptor.Value == null)
            {
                return;
            }
            // this.UpdateCanChangeEquipment();
            //InventoryStashVM stashVM = this.StashVM;
            //if (stashVM == null)
            {
                //  return;
            }
        }
    }
}
