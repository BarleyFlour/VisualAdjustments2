using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using Owlcat.Runtime.UI.SelectionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace VisualAdjustments2.UI
{
    public class BuffContainer : IFeatureSelectionItem
    {
		public BuffContainer(BlueprintBuff buff)
        {
			//this.Feature = buff;
        }
		public BlueprintFeature Feature;
        BlueprintFeature IFeatureSelectionItem.Feature => ((IFeatureSelectionItem)Feature).Feature;

        FeatureParam IFeatureSelectionItem.Param => ((IFeatureSelectionItem)Feature).Param;

        string IUIDataProvider.Name => ((IUIDataProvider)Feature).Name;

        string IUIDataProvider.Description => ((IUIDataProvider)Feature).Description;

        Sprite IUIDataProvider.Icon => ((IUIDataProvider)Feature).Icon;

        string IUIDataProvider.NameForAcronym => ((IUIDataProvider)Feature).NameForAcronym;
    }
    public class BuffButtonVM : SelectionGroupEntityVM
    {
		public BuffButtonVM(BlueprintUnitFact feature,bool AddOrRemove, Action<BuffButtonVM> act) : base(true)
		{
			this.action = act;
			this.AddOrRemove = AddOrRemove;
			this.Feature = feature;
			this.FeatureSprite = feature.Icon;
			this.FeatureName = feature.Name != "" ? feature.Name : feature.name;

			this.FeatureAcronymName = feature.NameForAcronym;
			FeatureTagsComponent component = feature.GetComponent<FeatureTagsComponent>();
			this.FeatureTags = (UIUtilityTexts.GetFeatureTags((component != null) ? new FeatureTag?(component.FeatureTags) : null) ?? string.Empty);
			this.FeatureDescription = feature.NameForAcronym;
			if (this.FeatureSprite == null)
			{
				this.FeatureSprite = UIUtility.GetIconByText(this.FeatureName);
				this.FeatureSpriteColor = UIUtility.GetColorByText(this.FeatureName);
				this.FeatureAcronym = UIUtility.GetAbilityAcronym(feature.NameForAcronym);
			}
			else
			{
				this.FeatureSpriteColor = Color.white;
				this.FeatureAcronym = "";
			}
			//this.HasFeatureParam = (feature.Param != null);
		}
		public void UpdateViewState(FeatureSelectionState featureSelectionState, bool updateAvailible = true, bool updateRecommendation = true)
		{
			//FeatureSelectionViewState viewState = featureSelectionState.GetViewState(this.Feature);
			//viewState.RefreshCanSelectState(featureSelectionState, featureSelectionState.Selection, this.Feature);
			//if (updateAvailible)
			{
			//	this.UpdateAvailibleState(featureSelectionState, viewState);
			}
			this.UpdateValues();
		}
		private void UpdateAvailibleState(FeatureSelectionState featureSelectionState, FeatureSelectionViewState viewState)
		{
			this.SelectState = viewState.CanSelectState;
			IFeatureSelectionItem selectedItem = featureSelectionState.SelectedItem;
			//if (selectedItem != null && selectedItem.IsSame(this.Feature) && viewState.CanSelectState != FeatureSelectionViewState.SelectState.Forbidden)
			{
			//	this.SelectState = FeatureSelectionViewState.SelectState.CanSelect;
			}
			switch (this.SelectState)
			{
				case FeatureSelectionViewState.SelectState.Forbidden:
					this.m_NotAvailableLabelValue = UIStrings.Instance.Tooltips.Prerequisites;
					this.m_IsAvailableFeatureValue = false;
					return;
				case FeatureSelectionViewState.SelectState.AlreadyHas:
					this.m_NotAvailableLabelValue = UIStrings.Instance.Tooltips.AlreadyHasFeature;
					this.m_IsAvailableFeatureValue = false;
					return;
				case FeatureSelectionViewState.SelectState.CanSelect:
					this.m_NotAvailableLabelValue = "";
					this.m_IsAvailableFeatureValue = true;
					return;
				default:
					return;
			}
		}
		private void UpdateValues()
		{
			if (this.IsSelected.Value)
			{
				this.NotAvailableLabel.Value = "";
				base.SetAvailableState(true);
			}
			else
			{
				this.NotAvailableLabel.Value = this.m_NotAvailableLabelValue;
				base.SetAvailableState(this.m_IsAvailableFeatureValue);
			}
		}
		public override void DoSelectMe()
		{
		}
		public ReactiveProperty<BuffButtonVM> GetSelectedReactiveProperty()
		{
			return this.GetSelectedReactiveProperty();
		}
		public bool HasText(string searchRequest)
		{
			return (this.FeatureName.IndexOf(searchRequest, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.FeatureDescription.IndexOf(searchRequest, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.FeatureTags.IndexOf(searchRequest, StringComparison.InvariantCultureIgnoreCase) >= 0);
		}
		public readonly BlueprintUnitFact Feature;
		public readonly Sprite FeatureSprite;
		public Color FeatureSpriteColor;
		public readonly string FeatureName;
        public string FeatureAcronymName;
        public readonly string FeatureDescription;
		public readonly string FeatureAcronym;
		public readonly string FeatureTags;
		public readonly StringReactiveProperty NotAvailableLabel = new StringReactiveProperty();
		public FeatureSelectionViewState.SelectState SelectState;
		private bool m_IsAvailableFeatureValue = true;
		private string m_NotAvailableLabelValue = "";
		public bool HasFeatureParam;
		public Action<BuffButtonVM> action;
		public bool AddOrRemove;
	}
}
