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
    public class CreateDollPCView : ViewBase<CreateDollVM>
    {
        public TMPro.TextMeshProUGUI Label;
        public OwlcatButton Button;
        public CreateDollPCView()
        {
            
        }
        public override void BindViewImplementation()
        {
            base.gameObject.SetActive(true);
            this.ViewModel?.AddDisposable(() => { this.ViewModel?.ToDisplay?.Subscribe((string s) => { Label.text = s; }); });
            Label.text = this.ViewModel?.ToDisplay?.Value;
        }

        public override void DestroyViewImplementation()
        {
            base.gameObject.SetActive(false);
        }
    }
}
