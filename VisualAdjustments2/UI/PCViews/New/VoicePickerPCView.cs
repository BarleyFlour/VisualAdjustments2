using System;
using Kingmaker.UI.Common.Animations;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.VirtualListSystem;

namespace VisualAdjustments2.UI
{
    public class VoicePickerPCView : ViewBase<VoicePickerVM>
    {
        public override void BindViewImplementation()
        {
            m_VoiceList.Bind(base.ViewModel?.m_VoiceList);
            this.gameObject.SetActive(true);
        }

        public override void DestroyViewImplementation()
        {
            this.gameObject.SetActive(false);
        }
        public ListPCView m_VoiceList;
        public FadeAnimator fadeAnimator;
    }
}