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

namespace VisualAdjustments2.UI
{
    //TODO: Serialize the array and check version to see if we need to grab again
    public static class ListPCViewExtensions
    {
        public static T AddComponent<T>(this GameObject game, T duplicate) where T : Component
        {
            T target = game.AddComponent<T>();
            foreach (PropertyInfo x in typeof(T).GetProperties())
                if (x.CanWrite)
                    x.SetValue(target, x.GetValue(duplicate));
            return target;
        }
        public static ListViewItemPCView ConvertToListPCView(this CharGenFeatureSelectorItemPCView oldview)
        {
            try
            {
                var newcomp = oldview.gameObject.AddComponent<ListViewItemPCView>();
                newcomp.m_Button = oldview.m_Button;
                newcomp.m_DisplayName = oldview.m_FeatureNameText;
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("CollapseButton"));
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("RecommendationPlace"));
                /*foreach(var comp in oldview.transform.Find("IconPlace").GetComponents<Component>().Concat(oldview.transform.Find("IconPlace").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }
                foreach (var comp in oldview.transform.Find("TextContainer/Description").GetComponents<Component>().Concat(oldview.transform.Find("TextContainer/Description").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }

                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("IconPlace"), true);
                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("TextContainer/Description"), true);*/
                var iconplace = oldview.transform.Find("IconPlace");
                
                //iconplace.localScale = new Vector3((float)0.75, (float)0.75, (float)0.75);
                var bg = iconplace.Find("BG");
                bg.Find("Icon").gameObject.SetActive(false);
                var acronym = bg.Find("Acronym");
                acronym.gameObject.SetActive(true);
                var acronymTMP = acronym.GetComponent<TextMeshProUGUI>();
                acronymTMP.fontSizeMax = 46;
                acronymTMP.fontSize = 46;
                acronymTMP.text = "&";
                acronymTMP.verticalAlignment = VerticalAlignmentOptions.Geometry;
                acronymTMP.horizontalAlignment = HorizontalAlignmentOptions.Flush;

                
              //  newcomp.m_IconText = acronymTMP;
                oldview.transform.Find("TextContainer/Description").gameObject.SetActive(false);
                UnityEngine.Component.Destroy(oldview);

                
                var InstantiatedButton = UnityEngine.GameObject.Instantiate(StaticCanvas.Instance.transform.Find("ServiceWindowsPCView/InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/FilterPCView/SwitchBar/NonUsable"),newcomp.transform);
                var icon = InstantiatedButton.Find("Icon");
                icon.gameObject.SetActive(true);
                UnityEngine.Component.DestroyImmediate(icon.gameObject.GetComponent<Image>(),true);
                var newtxt = icon.gameObject.AddComponent<TextMeshProUGUI>(acronymTMP);
                newtxt.text = ">>";
                newtxt.fontSizeMin = 45;
                newcomp.m_IconText = newtxt;
                iconplace.gameObject.SetActive(false);
                /*newtxt.font = acronymTMP.font;
                newtxt.alignment = TextAlignmentOptions.CenterGeoAligned;
                newtxt.horizontalAlignment = HorizontalAlignmentOptions.Geometry;
                newtxt.verticalAlignment = VerticalAlignmentOptions.Geometry;
                newtxt.fontSizeMax = 46;
                newtxt.material = acronymTMP.material;
                newtxt.fontStyle = FontStyles.Bold;
                newtxt.color = acronymTMP.color;
                newcomp.m_IconText = newtxt;*/

                return newcomp;
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw e;
            }
        }
    }
    public class ListPCView : SelectionGroupViewWithFilterPCView<ListViewVM, ListViewItemVM, ListViewItemPCView>
    {
        private const string currentconst = "Current EEs";
        private const string allconst = "All EEs";
        public readonly string[] SelectorString = { allconst, currentconst };
        public bool AllOrCurrent;
        public ListViewItemPCView m_Template;
        public void SetupFromChargenList(CharGenFeatureSelectorPCView oldcomp, bool AllOrCurrent)
        {
            this.m_CharGenFeatureSearchView = oldcomp.m_CharGenFeatureSearchView;
            this.m_SearchRequestEntitiesNotFound = oldcomp.m_SearchRequestEntitiesNotFound;
            this.AllOrCurrent = AllOrCurrent;
            if (m_Template == null)
            {
                var instantiated = UnityEngine.GameObject.Instantiate(oldcomp.SlotPrefabs.First());
                instantiated.ConvertToListPCView();
                if (AllOrCurrent)
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

            this.m_SelectorHeader.text = AllOrCurrent ? SelectorString[0] : SelectorString[1];
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
            var searchVM = new CharGenFeatureSearchVM();
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
            if (a.DisplayName == b.DisplayName) return 1; else return 0;
        }
        public CharGenFeatureSearchPCView m_CharGenFeatureSearchView;
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
