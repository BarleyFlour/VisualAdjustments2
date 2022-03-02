using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.SelectionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.UI
{
    public class VisualAdjustments2ServiceWindowVM<VMType> : ViewBase<VMType> where VMType : BaseDisposable, IViewModel
    {
        public void Show()
        {
            if (this.m_IsShowed)
            {
                return;
            }
            this.m_IsShowed = true;
            base.gameObject.SetActive(true);
        }
        public void Hide()
        {
            if (!this.m_IsShowed)
            {
                return;
            }
            this.m_IsShowed = false;
            base.gameObject.SetActive(false);
        }
        public bool m_IsShowed;
        public override void BindViewImplementation()
        {
            this.Show();
            base.AddDisposable(base.ViewModel);
        }

        public override void DestroyViewImplementation()
        {
            this.Hide();
        }
    }
}
