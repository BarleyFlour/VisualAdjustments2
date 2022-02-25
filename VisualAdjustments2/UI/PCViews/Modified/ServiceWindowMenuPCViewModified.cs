using Kingmaker;
using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Menu;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Menu;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Owlcat.Runtime.UI.Controls.Other;

namespace VisualAdjustments2.UI
{
    public class ServiceWindowMenuPCViewModified : ViewBase<ServiceWindowsMenuVMModified>
    {
        public void Initialize()
        {
            base.gameObject.SetActive(false);
            this.m_Animator.Initialize();
            this.m_MenuSelector.Initialize();
        }
        public override void BindViewImplementation()
        {
            this.m_Animator.AppearAnimation(null);
            this.m_MenuSelector.Bind(base.ViewModel.SelectionGroup);
        }
        public override void DestroyViewImplementation()
        {
            this.m_Animator.DisappearAnimation(null);
        }
        public FadeAnimator m_Animator;
        public ServiceWindowMenuSelectorPCViewModified m_MenuSelector;
    }
}
