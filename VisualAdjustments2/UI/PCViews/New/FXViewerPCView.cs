using Kingmaker;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.Other.NestedSelectionGroup;
using Kingmaker.UI.MVVM._VM.Other.NestedSelectionGroup;
using Kingmaker.UI.ServiceWindow;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class FXViewerPCView : VisualAdjustments2ServiceWindowVM<FXViewerVM>
    {
        public BuffListPCView m_CurrentFX;
        public BuffListPCView m_AllFX;
        public void Initialize()
        {

        }
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            this.m_CurrentFX.Bind(this.ViewModel.m_CurrentFX.Value);
            this.m_AllFX.Bind(this.ViewModel.m_AllFX.Value);
            this.ViewModel.AddDisposable(this.ViewModel.m_CurrentFX.Subscribe(this.m_CurrentFX.Bind));
            //this.ViewModel.AddDisposable(this.ViewModel.m_AllFX.Subscribe(this.m_AllFX.Bind));
            this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            this.m_VisualSettings.Dispose();
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            this.m_dollCharacterController.Unbind();
            this.m_CurrentFX.Unbind();
            this.m_AllFX.Unbind();
        }
        public DollCharacterController m_dollCharacterController;
        public CharacterVisualSettingsView m_VisualSettings;
        public ToggleGroupHandler m_WhiteOrBlacklist;
    }
}
