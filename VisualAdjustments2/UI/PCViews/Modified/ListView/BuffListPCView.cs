using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.BundlesLoading;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.UI.MVVM._PCView.Other.NestedSelectionGroup;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.FeatureSelector;
using Kingmaker.UI.MVVM._VM.Other;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using Kingmaker.UI.MVVM._PCView.Other.SelectionGroup;
using Owlcat.Runtime.UI.SelectionGroup;
using Owlcat.Runtime.UI.MVVM;
using Kingmaker.Blueprints;
using Kingmaker.Visual.CharacterSystem;
using System.Diagnostics;
using UnityEngine;
using Owlcat.Runtime.Core.Utils;
using System.Collections.ObjectModel;
using Kingmaker.UI;
using UnityEngine.UI;
using System.Reflection;
using Owlcat.Runtime.UI.Controls.Button;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class BuffListPCView : SelectionGroupViewWithFilterPCView<BuffListViewVM, BuffButtonVM, BuffButtonPCView>
    {
        public static string[] EESearchTerms = new string[]
        {
            "",
            "",
            ""
        };
        public BuffButtonPCView m_Template;
        public void SetupFromChargenList(CharGenFeatureSelectorPCView oldcomp, bool LeftOrRight, string LabelText)
        {
            var newpcview = oldcomp.gameObject.AddComponent<ListSearchPCView>();
            newpcview.SetupFromChargenFeatureSearchPCView(oldcomp.m_CharGenFeatureSearchView);
            this.m_CharGenFeatureSearchView = newpcview;
            this.m_SearchRequestEntitiesNotFound = oldcomp.m_SearchRequestEntitiesNotFound;
            if (m_Template == null)
            {
                var instantiated = UnityEngine.GameObject.Instantiate(oldcomp.SlotPrefabs.First());
                instantiated.ConvertToBuffButtonPCView();
                if (LeftOrRight)
                {
                    instantiated.transform.Find("NonUsable(Clone)").SetAsFirstSibling();
                    instantiated.transform.Find("Background").SetAsFirstSibling();
                    instantiated.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>().padding.left = 24;
                }
                else
                {
                    instantiated.transform.Find("NonUsable(Clone)").SetAsLastSibling();
                    instantiated.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>().padding.right = 18;
                }
                instantiated.transform.Find("TextContainer/Description").GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Bottom;
                m_Template = instantiated.GetComponent<BuffButtonPCView>();
            }
            this.m_SelectorHeader = this.transform.Find("HeaderH2/Label").GetComponent<TextMeshProUGUI>();

            this.m_SelectorHeader.text = LabelText;
            this.SlotPrefab = m_Template;
            this.VirtualList = oldcomp.VirtualList;
        }
        public void UpdateNotFoundText(bool val)
        {
            this.m_SearchRequestEntitiesNotFound.gameObject.SetActive(!val);
            if (!val)
            {
                this.m_SearchRequestEntitiesNotFound.text = string.Format(UIStrings.Instance.CharGen.SearchRequestEntitiesNotFound, this.m_CharGenFeatureSearchView.SearchRequest);
            }
        }
        public override void BindViewImplementation()
        {
            //base.BindViewImplementation();
            this.Initialize();
            var searchVM = new EESearchVM(new string[] { "", "", "", "" });
            base.AddDisposable(searchVM);
            m_CharGenFeatureSearchView.Bind(searchVM);
            if (this.m_CharGenFeatureSearchView != null)
            {
                base.AddDisposable(this.m_CharGenFeatureSearchView.SearchRequest.Subscribe(new Action<string>(this.OnSearchRequestChanged)));
            }
            base.AddDisposable(this.HasVisibleElements.Subscribe(new Action<bool>(this.UpdateNotFoundText)));
            base.AddDisposable(base.ViewModel.EntitiesCollection.ObserveCountChanged(false).Subscribe(delegate (int _)
            {
                this.OnCollectionChanged();
            }));
            base.AddDisposable(base.ViewModel.EntitiesCollection.ObserveCountChanged(false).Subscribe(delegate (int _)
            {
                this.OnSearchRequestChanged(null);
            }));
            this.OnCollectionChanged();
            base.AddDisposable(this.VirtualList.Subscribe<BuffButtonVM>(this.VisibleCollection));
            this.TryScrollToSelectedElement();
        }
        public override void DestroyViewImplementation()
        {
            // base.gameObject.SetActive(false);
            base.DestroyViewImplementation();
            this.m_CharGenFeatureSearchView?.Unbind();
        }
        public void OnSearchRequestChanged(string _)
        {
            this.VisibleCollection.Clear();
            base.OnCollectionChanged();
            base.TryScrollToSelectedElement();
            this.HasVisibleElements.Value = this.VisibleCollection.Any<BuffButtonVM>();
        }
        public override bool IsVisible(BuffButtonVM entity)
        {
            string searchRequest = this.m_CharGenFeatureSearchView.SearchRequest.Value;
            return string.IsNullOrEmpty(searchRequest) || entity.HasText(searchRequest);
        }
        public override int EntityComparer(BuffButtonVM a, BuffButtonVM b)
        {
            return string.Compare(a.FeatureName, b.FeatureName, StringComparison.CurrentCultureIgnoreCase);
            //if (a.FeatureName == b.FeatureName) return 1; else return 0;
        }
        public ListSearchPCView m_CharGenFeatureSearchView;
        public TextMeshProUGUI m_SearchRequestEntitiesNotFound;
        private TextMeshProUGUI m_SelectorHeader;
        public BoolReactiveProperty HasVisibleElements = new BoolReactiveProperty(true);
        public override bool HasSorter
        {
            get
            {
                return true;
            }
        }
        public override bool HasFilter
        {
            get
            {
                return true;
            }
        }
    }
}
