using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class ClassOutfitSelectorPCView : ViewBase<ClassOutfitSelectorVM>
    {

        public ReactiveProperty<ClassOutfitSelectorButtonPCView> Selected = new();
        public Dictionary<string /*GUID*/, ClassOutfitSelectorButtonPCView> Buttons = new();
        public void SetupSelectedState()
        {
            foreach (ClassOutfitSelectorButtonPCView button in this.Buttons.Values)
            {
                button.SetSelected(this.Selected.Value == button);
            }
        }
        public void OnUnitChanged(UnitDescriptor unit)
        {
            var settings = unit.Unit.GetSettings();
            if (settings.ClassOverride.GUID.IsNullOrEmpty()) Selected.Value = Buttons.First().Value;
            else Selected.Value = Buttons.FirstOrDefault(a => a.Key == settings.ClassOverride.GUID).Value;
        }
        public void SetClass(string guid)
        {
            var unit = Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit;
            var settings = unit.GetSettings();
            settings.ClassOverride.HasCustomOutfit = !(guid == "");
            var needsreset = (guid.IsNullOrEmpty() && unit.IsStoryCompanion() && !settings.ClassOverride.GUID.IsNullOrEmpty());
            if (needsreset)
            {
                unit.View.CharacterAvatar.RemoveAllEquipmentEntities(false);
                //Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.UpdateClassEquipment();
                unit.View.CharacterAvatar.RestoreSavedEquipment();
                //  foreach (var ee in ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(settings.ClassGUID).GetClothesLinks(unit.Gender,unit.Progression.Race.RaceId).Select(a => a.Load()))
                {
                    //     if(unit.View.CharacterAvatar.EquipmentEntities.Contains(ee) && unit.View.CharacterAvatar.SavedEquipmentEntities.Any(A => A.Load() == ee)) unit.View.CharacterAvatar.RemoveEquipmentEntity(ee);
                }
            }
            settings.ClassOverride.GUID = guid;
            Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.ForcceUseClassEquipment = guid != "";
            //Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.UpdateClassEquipment();
            Kingmaker.Game.Instance.UI.Common.DollRoom.m_Avatar.UpdateCharacter();
            Kingmaker.Game.Instance.UI.Common.DollRoom.m_Avatar.RebuildOutfit();
            Kingmaker.Game.Instance.UI.Common.DollRoom.Unit.View.UpdateClassEquipment();
            if (needsreset) unit.View.UpdateBodyEquipmentVisibility();
            Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.UpdateClassEquipment();
        }
        public override void BindViewImplementation()
        {
            try
            {
                var settings = Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings();
                if (settings.ClassOverride.GUID.IsNullOrEmpty()) Selected.Value = Buttons.First().Value;
                else Selected.Value = Buttons.First(a => a.Key == settings.ClassOverride.GUID).Value;

                base.AddDisposable(this.Selected.Subscribe((ClassOutfitSelectorButtonPCView _) =>
                {
                    this.SetupSelectedState();
                    SetClass(_.GUID);
                }));
                this.AddDisposable(Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(OnUnitChanged));
            }
            catch (Exception e)
            {
                Main.Logger.Log(e.ToString());
            }
        }
        public override void DestroyViewImplementation()
        {
        }
    }
}
