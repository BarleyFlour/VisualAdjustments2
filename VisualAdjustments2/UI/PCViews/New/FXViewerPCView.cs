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
using UniRx;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class FXViewerPCView : VisualAdjustments2ServiceWindowVM<FXViewerVM>
    {
        public BuffListPCView m_CurrentFX;
        public BuffListPCView m_AllFX;
        public void OnCharacterChanged()
        {
            m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit);
            var settings = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings();
            m_WhiteOrBlacklist.PrimOrSec.Value = settings.Fx_Settings.WhiteOrBlackList;
            this.m_VisualSettings.Dispose();
        }
        public void Initialize()
        {

        }
        public override void BindViewImplementation()
        {
            base.ViewModel.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(delegate (UnitDescriptor _)
            {
                this.OnCharacterChanged();
            }));
            m_WhiteOrBlacklist.PrimOrSec.Value = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings().Fx_Settings.WhiteOrBlackList;
            base.AddDisposable(m_WhiteOrBlacklist.PrimOrSec.Subscribe((a) => { var settings = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings(); settings.Fx_Settings.WhiteOrBlackList = a; }));
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
        public ToggleWithTextHandler m_WhiteOrBlacklist;
    }
}
