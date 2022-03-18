using Kingmaker;
using Kingmaker.BundlesLoading;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.UnitLogic;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using Owlcat.Runtime.UI.Controls.Button;
using Kingmaker.UI.ServiceWindow;

namespace VisualAdjustments2.UI
{
    public class EEPickerPCView : VisualAdjustments2ServiceWindowVM<EEPickerVM>
    {
        public void Initialize()
        {

        }
        private void OnCharacterChanged()
        {
            m_CurrentEEs.Bind(base.ViewModel.CurrentEEs.Value);
            this.AddDisposable(base.ViewModel.CurrentEEs.Value);
            m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            m_EEColorPicker.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            this.m_VisualSettings.Dispose();
        }
        public override void BindViewImplementation()
        {
            base.ViewModel.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(delegate (UnitDescriptor _)
            {
                this.OnCharacterChanged();
            }));
            
            base.BindViewImplementation();
            Initialize();
            m_EEColorPicker.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            m_EEColorPicker.m_ConfirmButton.OnLeftClick.AddListener(() => { this.ViewModel?.ApplyColor(m_EEColorPicker.m_Color.m_ToColor.color, m_EEColorPicker.PrimaryOrSecondary); });
            m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            this.AddDisposable(base.ViewModel.AllEEs.Value);
            this.m_AllEEs.Bind(base.ViewModel.AllEEs.Value);
            this.AddDisposable(base.ViewModel.CurrentEEs.Value);
            this.m_CurrentEEs.Bind(base.ViewModel.CurrentEEs.Value);
            this.m_VisualSettings.Dispose();
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            m_EEColorPicker.Dispose();
            this.m_dollCharacterController.Unbind();
        }
        public CharacterVisualSettingsView m_VisualSettings;
        public DollCharacterController m_dollCharacterController;
        public ListPCView m_AllEEs;
        public ListPCView m_CurrentEEs;
        public ListViewItemPCView template;
        public OwlcatButton m_ApplyButton;
        public EEColorPickerPCView m_EEColorPicker;
    }
}
