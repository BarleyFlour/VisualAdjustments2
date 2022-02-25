using Kingmaker.Blueprints.Root.Strings;
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

namespace VisualAdjustments2.UI
{
	public class BarleyListPCView : NestedSelectionGroupRadioPCView<ListViewItemVM, ListViewItemPCView>
	{
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
		public override void BindViewImplementation()
		{
			base.BindViewImplementation();
			if (this.m_CharGenFeatureSearchView != null)
			{
				base.AddDisposable(this.m_CharGenFeatureSearchView.SearchRequest.Subscribe(new Action<string>(this.OnSearchRequestChanged)));
			}
			base.AddDisposable(this.HasVisibleElements.Subscribe(new Action<bool>(this.UpdateNotFoundText)));
		}
		private void UpdateNotFoundText(bool val)
		{
			this.m_SearchRequestEntitiesNotFound.gameObject.SetActive(!val);
			if (!val)
			{
				this.m_SearchRequestEntitiesNotFound.text = string.Format(UIStrings.Instance.CharGen.SearchRequestEntitiesNotFound, this.m_CharGenFeatureSearchView.SearchRequest);
			}
		}
		public void OnSearchRequestChanged(string request)
		{
			base.ClearListView();
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
			bool flag = a.IsSelected.Value || a.IsAvailable.Value;
			int num = (b.IsSelected.Value || b.IsAvailable.Value).CompareTo(flag);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase);
		}
        private CharGenFeatureSearchPCView m_CharGenFeatureSearchView;
		private TextMeshProUGUI m_SearchRequestEntitiesNotFound;
		public BoolReactiveProperty HasVisibleElements = new BoolReactiveProperty(true);
	}
}
