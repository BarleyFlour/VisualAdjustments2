using Kingmaker.BundlesLoading;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VisualAdjustments2.UI
{
    public class EEPickerPCView : VisualAdjustments2ServiceWindowVM<EEPickerVM>
    {
        public void Initialize()
        {

        }
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            Initialize();
            this.AddDisposable(base.ViewModel.AllEEs.Value);
            this.m_AllEEs.Bind(base.ViewModel.AllEEs.Value);
            this.AddDisposable(base.ViewModel.CurrentEEs.Value);
            this.m_CurrentEEs.Bind(base.ViewModel.CurrentEEs.Value);
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
        }
        public ListPCView m_AllEEs;
        public ListPCView m_CurrentEEs;
        public ListViewItemPCView template;
    }
}
