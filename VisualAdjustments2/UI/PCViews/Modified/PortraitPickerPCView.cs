using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Owlcat.Runtime.UniRx;
using Kingmaker.UI.MVVM._VM.Utility;

namespace VisualAdjustments2.UI
{
    public class PortraitPickerPCView : VisualAdjustments2ServiceWindowVM<PortraitPickerVM>, IInitializable
    {
        public Kingmaker.UI.MVVM._PCView.CharGen.Phases.Portrait.CharGenPortraitPhaseDetailedPCView m_PortraitPicker;

        public Kingmaker.UI.MVVM._PCView.CharGen.Portrait.CharGenPortraitPCView m_PortraitPreview;

        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            this.m_PortraitPicker.Bind(this.ViewModel.PickerVM.Value);
            this.ViewModel.AddDisposable(this.ViewModel.PickerVM.Value);
            this.m_PortraitPreview.Bind(this.ViewModel.PreviewVM.Value);
            base.AddDisposable(this.ViewModel.PreviewVM.Value);
            base.AddDisposable(this.ViewModel.PreviewVM.Subscribe(b => this.m_PortraitPreview.Bind(b)));
            this.m_PortraitPicker.transform.Find("ContentWrapper/PortraitSelector/DefaultPortraitGroup/Scroll View/Viewport/Content/PortraitGroupView(Clone)/Expandable").gameObject.SetActive(false);
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            this.m_PortraitPicker.Unbind();
            this.m_PortraitPreview.Unbind();
        }

        public void Initialize()
        {
            this.m_PortraitPicker.Initialize();
            this.m_PortraitPreview.Initialize();
        }
    }
}
