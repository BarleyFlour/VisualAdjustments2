using Kingmaker;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Owlcat.Runtime.UniRx;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Inventory.VisualSettings;
using Kingmaker.Visual.CharacterSystem;
using Kingmaker.View;

namespace VisualAdjustments2.UI
{
    public class EquipmentPCView : VisualAdjustments2ServiceWindowVM<EquipmentVM>
    {
        public void OnUnitChanged(UnitReference _)
        {
            this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            this.m_weaponOverridePCView.Bind(this.ViewModel.m_weaponOverride);
            foreach (var button in this.m_EquipmentHideButtons)
            {
                button.Bind(button.m_Label.text, button.type);
            }
            Gender gender = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Gender;
            var race = UnitEntityView.GetActualRace(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            if(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Progression.GetEquipmentClass() != null && Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Progression.GetEquipmentClass().GetClothesLinks(gender, race).Count > 0) this.m_ColorPicker.UpdateRampSlider(Game.Instance.SelectionCharacter.SelectedUnit?.Value.Value?.Progression?.GetEquipmentClass()?.GetClothesLinks(gender, race)?.FirstOrDefault(a => a.Load()?.PrimaryColorsProfile?.Ramps?.Count() > 0).Load());
        }
        public void Initialize()
        {

        }
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            this.AddDisposable(Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(OnUnitChanged));
            this.m_VisualSettings.gameObject.SetActive(false);
            this.m_weaponOverridePCView.Bind(this.ViewModel.m_weaponOverride);
            this.m_classOutfitSelectorPCView.Bind(this.ViewModel.m_classOutfitSelectorVM);
            this.m_ColorPicker.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            this.m_ColorPicker.m_ConfirmButton.OnLeftClick.RemoveAllListeners();
            this.m_ColorPicker.m_ConfirmButton.OnLeftClick.AddListener(() =>
            {
                if (m_ColorPicker.CustomColor)
                {
                    this.ViewModel?.ApplyColor(m_ColorPicker.m_Color.m_ToColor.color, m_ColorPicker.PrimaryOrSecondary);
                    Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;
                    Game.Instance.UI.Common.DollRoom.m_Avatar.IsAtlasesDirty = true;
                    Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.UpdateClassEquipment();
                }
                else
                {
                    this.ViewModel?.ApplyColor(m_ColorPicker.Index.Value, m_ColorPicker.PrimaryOrSecondary);
                    Game.Instance.UI.Common.DollRoom.m_Avatar.IsDirty = true;
                    Game.Instance.UI.Common.DollRoom.m_Avatar.IsAtlasesDirty = true;
                    Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.UpdateClassEquipment();
                }
            });

            Gender gender = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Gender;
            var race = UnitEntityView.GetActualRace(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            
            this.AddDisposable(this.m_ColorPicker.Index.Subscribe((int index) => { this.m_ColorPicker.UpdateColorFromIndex(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Progression.GetEquipmentClass().GetClothesLinks(gender, race).FirstOrDefault(b => b.Load().PrimaryColorsProfile.Ramps.Count > 0).Load(), index); }));
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            this.m_dollCharacterController.Unbind();
        }
        public List<HideEquipmentButtonPCView> m_EquipmentHideButtons = new();
        public WeaponOverridePCView m_weaponOverridePCView;
        public OwlcatButton m_ApplyButton;
        public EquipmentListPCView m_ListPCView;
        public CharacterVisualSettingsPCView m_VisualSettings;
        public DollCharacterController m_dollCharacterController;
        public ClassOutfitSelectorPCView m_classOutfitSelectorPCView;
        public EEColorPickerView m_ColorPicker;
    }
}
