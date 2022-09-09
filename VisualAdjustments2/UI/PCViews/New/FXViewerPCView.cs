using Kingmaker;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.Other.NestedSelectionGroup;
using Kingmaker.UI.MVVM._VM.Other.NestedSelectionGroup;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Inventory.VisualSettings;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Inventory;
using UniRx;
using VisualAdjustments2.Infrastructure;
using Owlcat.Runtime.UniRx;

namespace VisualAdjustments2.UI
{
    public class FXViewerPCView : VisualAdjustments2ServiceWindowVM<FXViewerVM>
    {
        public BuffListPCView m_CurrentFX;
        public BuffListPCView m_AllFX;
        public CharacterVisualSettingsEntityPCView m_FixScaleButton;
        public void SetFixScale()
        {
            //if (Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value == null) return;
            //this.m_FixScaleButton.ViewModel.IsOn.Value = !this.m_FixScaleButton.ViewModel.IsOn.Value;
            Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().BuffSettings.FixSize = !this.m_FixScaleButton.ViewModel.IsOn.Value;
        }
        public void OnCharacterChanged()
        {
            m_dollCharacterController?.Bind(Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value);
            var settings = Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value?.GetSettings();
            if (m_WhiteOrBlacklist?.PrimOrSec?.Value != null && settings?.BuffSettings?.WhiteOrBlackList != null) m_WhiteOrBlacklist.PrimOrSec.Value = settings.BuffSettings.WhiteOrBlackList;
            this.m_VisualSettings.gameObject.SetActive(false);
            this.m_FixScaleButton.Bind(new CharacterVisualSettingsEntityVM(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().BuffSettings.FixSize,SetFixScale));
            this.m_FixScaleButton.ViewModel.IsOn.Value = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().BuffSettings.FixSize;
        }
        public void Initialize()
        {

        }
        public override void BindViewImplementation()
        {
            base.ViewModel.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(delegate (UnitReference _)
            {
                this.OnCharacterChanged();
            }));
            m_WhiteOrBlacklist.PrimOrSec.Value = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().BuffSettings.WhiteOrBlackList;
            base.AddDisposable(m_WhiteOrBlacklist.PrimOrSec.Subscribe((a) => { var settings = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings(); settings.BuffSettings.WhiteOrBlackList = a; }));
            base.BindViewImplementation();
            this.m_CurrentFX.Bind(this.ViewModel.m_CurrentFX.Value);
            this.m_AllFX.Bind(this.ViewModel.m_AllFX.Value);
            this.ViewModel.AddDisposable(this.ViewModel.m_CurrentFX.Subscribe(this.m_CurrentFX.Bind));
            //this.ViewModel.AddDisposable(this.ViewModel.m_AllFX.Subscribe(this.m_AllFX.Bind));
            this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            this.m_VisualSettings.gameObject.SetActive(false);
            this.m_FixScaleButton.Bind(new CharacterVisualSettingsEntityVM(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().BuffSettings.FixSize,SetFixScale));
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            this.m_dollCharacterController.Unbind();
            this.m_CurrentFX.Unbind();
            this.m_AllFX.Unbind();
        }
        public DollCharacterController m_dollCharacterController;
        public CharacterVisualSettingsPCView m_VisualSettings;
        public ToggleWithTextHandler m_WhiteOrBlacklist;
    }
}
