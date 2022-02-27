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
    public class EEPickerPCView : ViewBase<EEPickerVM>
    {
        public void Initialize()
        {
            {
                var alleelistview = GameObject.Instantiate(gameObject.transform.parent.parent.parent.Find("ChargenPCView/ContentWrapper/DetailedViewZone/ChargenFeaturesDetailedPCView/FeatureSelectorPlace/FeatureSelectorView").gameObject, this.transform);
                alleelistview.transform.localPosition = new Vector3(650, -50, 0);
                var oldcomp = alleelistview.GetComponent<CharGenFeatureSelectorPCView>();
                var newcomp = alleelistview.AddComponent<ListPCView>();
                newcomp.SetupFromChargenList(oldcomp);
                UnityEngine.Component.Destroy(oldcomp);
                this.m_AllEEs = newcomp;
            }
            {
                var alleelistview = GameObject.Instantiate(gameObject.transform.parent.parent.parent.Find("ChargenPCView/ContentWrapper/DetailedViewZone/ChargenFeaturesDetailedPCView/FeatureSelectorPlace/FeatureSelectorView").gameObject, this.transform);
                alleelistview.transform.localPosition = new Vector3(-650, -50, 0);
                var oldcomp = alleelistview.GetComponent<CharGenFeatureSelectorPCView>();
                var newcomp = alleelistview.AddComponent<ListPCView>();
                newcomp.SetupFromChargenList(oldcomp);
                UnityEngine.Component.Destroy(oldcomp);
                this.m_CurrentEEs = newcomp;
            }

        }
        public override void BindViewImplementation()
        {
            Initialize();
            this.m_AllEEs.Bind(base.ViewModel.AllEEs.Value);
            this.m_CurrentEEs.Bind(base.ViewModel.CurrentEEs.Value);
        }

        public override void DestroyViewImplementation()
        {
        }
        public ListPCView m_AllEEs;
        public ListPCView m_CurrentEEs;
        public ListViewItemPCView template;
    }
}
