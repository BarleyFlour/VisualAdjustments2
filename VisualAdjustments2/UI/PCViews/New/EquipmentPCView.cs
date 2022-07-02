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

namespace VisualAdjustments2.UI
{
    public class EquipmentPCView : VisualAdjustments2ServiceWindowVM<EquipmentVM>
    {
        public void OnUnitChanged(UnitDescriptor _)
        {
            this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            this.m_weaponOverridePCView.Bind(this.ViewModel.m_weaponOverride);
            foreach(var button in this.m_EquipmentHideButtons)
            {
                button.Bind(button.m_Label.text,button.type);
            }
        }
        public void Initialize()
        {

        }
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            this.AddDisposable(Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(OnUnitChanged));
            this.m_VisualSettings.Dispose();
            this.m_weaponOverridePCView.Bind(this.ViewModel.m_weaponOverride);
            this.m_classOutfitSelectorPCView.Bind(this.ViewModel.m_classOutfitSelectorVM);
            this.m_ColorPicker.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            this.m_ColorPicker.m_ConfirmButton.OnLeftClick.RemoveAllListeners();
            this.m_ColorPicker.m_ConfirmButton.OnLeftClick.AddListener(() =>
            {
                if (m_ColorPicker.CustomColor)
                    this.ViewModel?.ApplyColor(m_ColorPicker.m_Color.m_ToColor.color, m_ColorPicker.PrimaryOrSecondary);
                else
                    this.ViewModel?.ApplyColor(m_ColorPicker.Index.Value, m_ColorPicker.PrimaryOrSecondary);

            });
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
        public CharacterVisualSettingsView m_VisualSettings;
        public DollCharacterController m_dollCharacterController;
        public ClassOutfitSelectorPCView m_classOutfitSelectorPCView;
        public EEColorPickerView m_ColorPicker;
    }
}
