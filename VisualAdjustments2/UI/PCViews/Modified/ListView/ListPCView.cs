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

namespace VisualAdjustments2.UI
{
    //TODO: Serialize the array and check version to see if we need to grab again
    public static class ListPCViewExtensions
    {
        public static ListViewItemPCView ConvertToListPCView(this CharGenFeatureSelectorItemPCView oldview)
        {
            var newcomp = oldview.gameObject.AddComponent<ListViewItemPCView>();
            newcomp.m_Button = oldview.m_Button;
            newcomp.m_DisplayName = oldview.m_FeatureNameText;
            UnityEngine.Component.Destroy(oldview);
            return newcomp;
        }
    }
    public class ListPCView : SelectionGroupViewWithFilterPCView<ListViewVM, ListViewItemVM, ListViewItemPCView>
    {
        public ListViewItemPCView m_Template;
        public void SetupFromChargenList(CharGenFeatureSelectorPCView oldcomp)
        {
            this.m_CharGenFeatureSearchView = oldcomp.m_CharGenFeatureSearchView;
            this.m_SearchRequestEntitiesNotFound = oldcomp.m_SearchRequestEntitiesNotFound;
            if (m_Template == null)
            {
                var instantiated = UnityEngine.GameObject.Instantiate(oldcomp.SlotPrefabs.First());
                instantiated.ConvertToListPCView();
                m_Template = instantiated.GetComponent<ListViewItemPCView>();
            }
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
            base.BindViewImplementation();
            this.Initialize();
            if (this.m_CharGenFeatureSearchView != null)
            {
              //  base.AddDisposable(this.m_CharGenFeatureSearchView.SearchRequest.Subscribe(new Action<string>(this.OnSearchRequestChanged)));
            }
            base.AddDisposable(this.HasVisibleElements.Subscribe(new Action<bool>(this.UpdateNotFoundText)));
            base.AddDisposable(base.ViewModel.EntitiesCollection.ObserveCountChanged(false).Subscribe(delegate (int _)
            {
                this.OnCollectionChanged();
            }));
            this.OnCollectionChanged();
            base.AddDisposable(this.VirtualList.Subscribe<ListViewItemVM>(this.VisibleCollection));
            this.TryScrollToSelectedElement();
        }
        public override bool IsVisible(ListViewItemVM entity)
        {
            // TODO: Search logic here i think.
            return true;
        }
        public override int EntityComparer(ListViewItemVM a, ListViewItemVM b)
        {
            if (a.DisplayName == b.DisplayName) return 1; else return 0;
        }
        public CharGenFeatureSearchPCView m_CharGenFeatureSearchView;
        public TextMeshProUGUI m_SearchRequestEntitiesNotFound;
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
