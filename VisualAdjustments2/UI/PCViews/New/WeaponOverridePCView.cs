using Kingmaker.UI.Common;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Owlcat.Runtime.UniRx;
using Kingmaker.View.Animation;
using Kingmaker;

namespace VisualAdjustments2.UI
{
    public class WeaponOverridePCView : VisualAdjustments2ServiceWindowVM<WeaponOverrideVM>
    {
        public void Initialize()
        {
            m_DropDown.onValueChanged = new TMPro.TMP_Dropdown.DropdownEvent();
            m_DropDown.onValueChanged.AddListener((int i) => { this?.ViewModel?.OnWeaponTypeChanged(i); });
            m_ToggleGroup.PrimOrSec.Subscribe((bool b) => { if (this.ViewModel != null) this.ViewModel.hand.Value = b; });
            //m_ToggleGroup.PrimOrSec.Subscribe((bool b) => { if (this.ViewModel != null) this.ViewModel.hand.Value = b; });
        }

        public void OnUnitChanged()
        {
            
        }

        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            base.AddDisposable(this.ViewModel.slot.Subscribe((int i) =>
            {
                var animstyle = Game.Instance.SelectionCharacter.SelectedUnit.Value?.Unit?.View?.HandsEquipment?.m_ActiveSet?.MainHand?.VisibleItemBlueprint?.VisualParameters?.AnimStyle;
                if (animstyle != null) this.m_DropDown.value = WeaponOverrideVM.m_AnimToInt[(WeaponAnimationStyle)animstyle];
                var viewVM = this.ViewModel.SelectFromSettings();
                if (viewVM != null)
                {
                    this.ViewModel.m_ListViewVM?.Value?.TrySelectEntity(viewVM);
                }
                else this.ViewModel.m_ListViewVM?.Value?.TryUnselectEntity(this.ViewModel.m_ListViewVM.Value.SelectedEntity.Value);
            }));
            var animstyle2 = Game.Instance.SelectionCharacter.SelectedUnit.Value?.Unit?.View?.HandsEquipment?.m_ActiveSet?.MainHand?.VisibleItemBlueprint?.VisualParameters?.AnimStyle;
            if (animstyle2 != null) this.m_DropDown.value = WeaponOverrideVM.m_AnimToInt[(WeaponAnimationStyle)animstyle2];
            //base.AddDisposable(this.animStyle.Subscribe((WeaponAnimationStyle i) => { this.m_ListViewVM.Value.TrySelectEntity(SelectFromSettings()); }));
            base.AddDisposable(this.ViewModel.hand.Subscribe((bool i) =>
            {
                var viewVM = this.ViewModel.SelectFromSettings();
                if (viewVM != null)
                {
                    this.ViewModel.m_ListViewVM?.Value?.TrySelectEntity(viewVM);
                    //Main.Logger.Log($"Selected {viewVM.DisplayName}");
                }
                else this.ViewModel.m_ListViewVM?.Value?.TryUnselectEntity(this.ViewModel.m_ListViewVM.Value.SelectedEntity.Value);
            }));
            m_DropDown.value = 0;
            m_SlotButtons.OnClickPage(Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.Body.CurrentHandEquipmentSetIndex);
            // this.ViewModel.slot.Value = ;
            m_ToggleGroup.PrimOrSec.Value = true;
            m_ListPCView.Bind(ViewModel.m_ListViewVM.Value);
            this.AddDisposable(ViewModel.m_ListViewVM.Subscribe(this.m_ListPCView.Bind));
            if (Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.HandsEquipment?.m_ActiveSet?.MainHand?.VisibleItem != null) Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.HandsEquipment?.m_ActiveSet?.MainHand?.UpdateWeaponEnchantmentFx(true);
            if (Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.HandsEquipment?.m_ActiveSet?.OffHand?.VisibleItem != null) Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.HandsEquipment?.m_ActiveSet?.OffHand?.UpdateWeaponEnchantmentFx(true);
            if (Game.Instance.UI.Common.DollRoom.m_AvatarHands?.m_ActiveSet?.MainHand?.VisibleItem != null) Game.Instance.UI.Common.DollRoom.m_AvatarHands?.m_ActiveSet?.MainHand?.UpdateWeaponEnchantmentFx(true);
            if(Game.Instance.UI.Common.DollRoom.m_AvatarHands?.m_ActiveSet?.OffHand?.VisibleItem != null) Game.Instance.UI.Common.DollRoom.m_AvatarHands?.m_ActiveSet?.OffHand?.UpdateWeaponEnchantmentFx(true);
            //  this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            // this.m_VisualSettings.Dispose();
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            //  this.m_dollCharacterController.Unbind();
        }
        // public Dictionary<int, WeaponAnimationStyle> m_IntToAnim;
        // public OwlcatButton m_ApplyButton;
        public ClickablePageNavigation m_SlotButtons;
        public ListPCView m_ListPCView;
        public TMP_DropdownWorkaround m_DropDown;
        public ToggleGroupHandler m_ToggleGroup;
        // public CharacterVisualSettingsView m_VisualSettings;
        //public DollCharacterController m_dollCharacterController;
    }
}
