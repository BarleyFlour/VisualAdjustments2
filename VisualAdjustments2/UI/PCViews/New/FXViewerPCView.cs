using Kingmaker.UI.MVVM._PCView.Other.NestedSelectionGroup;
using Kingmaker.UI.MVVM._VM.Other.NestedSelectionGroup;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.UI
{
    public class FXViewerPCView : VisualAdjustments2ServiceWindowVM<FXViewerVM>
    {
        public ListPCView m_CurrentFX;
        public ListPCView m_AllFX;
        public OwlcatButton m_AddRemoveButton;
        public void Initialize()
        {

        }
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
        }
    }
}
