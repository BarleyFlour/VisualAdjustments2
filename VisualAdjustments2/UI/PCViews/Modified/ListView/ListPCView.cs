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
    public class ListPCView : SelectionGroupViewWithFilterPCView<ListViewVM, ListViewItemVM, ListViewItemPCView>
    {
        public static string[] EESearchTerms = new string[]
        {
            "",
            "",
            ""
        };
        public ListViewItemPCView m_Template;
        public virtual void SetupFromChargenList(CharGenFeatureSelectorPCView oldcomp, bool LeftOrRight, string LabelText)
        {
            var newpcview = oldcomp.gameObject.AddComponent<ListSearchPCView>();
            newpcview.SetupFromChargenFeatureSearchPCView(oldcomp.m_CharGenFeatureSearchView);
            this.m_CharGenFeatureSearchView = newpcview;
            this.m_SearchRequestEntitiesNotFound = oldcomp.m_SearchRequestEntitiesNotFound;
            if (m_Template == null)
            {
                var instantiated = UnityEngine.GameObject.Instantiate(oldcomp.SlotPrefabs.First());
                instantiated.ConvertToListPCView();
                if (LeftOrRight)
                {
                    instantiated.transform.Find("TextContainer").SetAsLastSibling();
                    instantiated.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>().padding.left = 20;
                }
                else
                {
                    instantiated.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>().padding.right = 12;
                }
                m_Template = instantiated.GetComponent<ListViewItemPCView>();
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
            base.AddDisposable(this.VirtualList.Subscribe<ListViewItemVM>(this.VisibleCollection));
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
            this.HasVisibleElements.Value = this.VisibleCollection.Any<ListViewItemVM>();
        }
        public override bool IsVisible(ListViewItemVM entity)
        {
            string searchRequest = this.m_CharGenFeatureSearchView.SearchRequest.Value;
            return string.IsNullOrEmpty(searchRequest) || entity.HasText(searchRequest);
        }
        public override int EntityComparer(ListViewItemVM a, ListViewItemVM b)
        {
            if (b.Guid.IsNullOrEmpty()/* || (a.Guid == "Hide" || a.InternalName == "Hide" || a.DisplayName == "Hide")*/) return 0;
            if (a.Guid.IsNullOrEmpty()/* || (b.Guid == "Hide" || b.InternalName == "Hide" || b.DisplayName == "Hide")*/) return 0;
            //if (a.Guid == "Hide" || a.InternalName == "Hide" || a.DisplayName == "Hide") return 0;
            //if (b.Guid == "Hide" || b.InternalName == "Hide" || b.DisplayName == "Hide") return 0;

           // Main.Logger.Log($"{string.Compare(a.DisplayName, b.DisplayName)} + {a.DisplayName} + {b.DisplayName}");
            return string.Compare(a.DisplayName, b.DisplayName, StringComparison.CurrentCultureIgnoreCase);
            // return 0;
        }
        public ListSearchPCView m_CharGenFeatureSearchView;
        public TextMeshProUGUI m_SearchRequestEntitiesNotFound;
        public TextMeshProUGUI m_SelectorHeader;
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
