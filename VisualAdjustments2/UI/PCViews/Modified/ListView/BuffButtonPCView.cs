using System;
using Kingmaker.UI.MVVM._PCView.Other;
using Kingmaker.UI.MVVM._PCView.Other.NestedSelectionGroup;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.FeatureSelector;
using Kingmaker.UI.MVVM._VM.Other;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using Owlcat.Runtime.UI.SelectionGroup.View;
using Owlcat.Runtime.UniRx;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VisualAdjustments2.UI
{
    public class BuffButtonPCView : SelectionGroupEntityView<BuffButtonVM>, IEventSystemHandler, IPointerEnterHandler
    {
        // Token: 0x060063A0 RID: 25504 RVA: 0x00201F78 File Offset: 0x00200178
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            this.m_FeatureImage.sprite = base.ViewModel.FeatureSprite;
            this.m_FeatureImage.color = base.ViewModel.FeatureSpriteColor;
            string str = base.ViewModel.FeatureName;
            if (str.Length > this.m_MaxSymbols)
            {
                str = base.ViewModel.FeatureName.Substring(0, this.m_MaxSymbols - 2) + "...";
            }
            this.m_FeatureNameText.text = str;
            this.m_FeatureAcronymText.text = base.ViewModel.FeatureAcronym;
            this.m_FeatureAcronymText.gameObject.SetActive(!string.IsNullOrEmpty(base.ViewModel.FeatureAcronym));
            // this.m_FeatureDescription.text = base.ViewModel.FeatureAcronymName;
            this.m_FeatureDescription.text = base.ViewModel.FeatureDescription;
            this.m_FeatureDescription.gameObject.SetActive(true);
            m_AddButton.OnLeftClick.AddListener(OnClickAddRemove);
            this.m_IconText.text = base.ViewModel.AddOrRemove ? ListViewItemVM.Add : ListViewItemVM.Remove;
            //base.AddDisposable(base.ViewModel.NotAvailableLabel.Subscribe(new Action<string>(this.SetNotAvailableLabel)));
            //base.AddDisposable(this.m_Button.OnRightClickAsObservable().Subscribe(new Action(this.OnRightClick)));
            //base.AddDisposable(this.m_Button.OnRightClickNotInteractableAsObservable().Subscribe(new Action(this.OnRightClick)));
        }
        public void OnClickAddRemove()
        {
            this?.ViewModel?.action(this?.ViewModel);
        }

        public override void OnChangeSelectedState(bool value)
        {
            base.OnChangeSelectedState(value);
            this.m_FeatureImage.material = (base.ViewModel.IsAvailable.Value ? null : this.m_Material);
        }

        // Token: 0x060063A3 RID: 25507 RVA: 0x002020C4 File Offset: 0x002002C4
        public override void OnClick()
        {
            if (base.ViewModel.IsAvailable.Value && (!base.ViewModel.IsSelected.Value || base.ViewModel.AllowSwitchOff))
            {
                this.ApplyOnClick();
                return;
            }
        }

        // Token: 0x060063A4 RID: 25508 RVA: 0x0020211C File Offset: 0x0020031C
        public void ApplyOnClick()
        {
            base.ViewModel.SetSelectedFromView(!base.ViewModel.IsSelected.Value);
            if (!base.ViewModel.AllowSwitchOff)
            {
                this.OnChangeSelectedState(base.ViewModel.IsSelected.Value);
            }
        }

        // Token: 0x060063A5 RID: 25509 RVA: 0x000433CF File Offset: 0x000415CF
        public void SetNotAvailableLabel(string notAvailableLabel)
        {
            this.m_NotAvailableLabel.text = notAvailableLabel;
            this.m_NotAvailableLabel.gameObject.SetActive(!string.IsNullOrEmpty(notAvailableLabel));
        }

        // Token: 0x060063A6 RID: 25510 RVA: 0x000433F6 File Offset: 0x000415F6
        public void SetRecommendation(RecommendationMarkerVM recommendation)
        {
            this.m_RecommendationObject.Bind(recommendation);
        }

        // Token: 0x060063A7 RID: 25511 RVA: 0x00043404 File Offset: 0x00041604
        public void OnPointerEnter(PointerEventData eventData)
        {
          //  base.ViewModel.TryShowTooltip();
        }

        public TextMeshProUGUI m_FeatureDescription;
        // Token: 0x04004262 RID: 16994
        [Header("Feature description")]
        [SerializeField]
        public Image m_FeatureImage;

        // Token: 0x04004263 RID: 16995
        [SerializeField]
        public Material m_Material;

        // Token: 0x04004264 RID: 16996
        [SerializeField]
        public TextMeshProUGUI m_FeatureAcronymText;

        // Token: 0x04004265 RID: 16997
        [SerializeField]
        public TextMeshProUGUI m_FeatureNameText;

        // Token: 0x04004266 RID: 16998
        [SerializeField]
        public int m_MaxSymbols = 33;

        // Token: 0x04004267 RID: 16999
        [SerializeField]
        public TextMeshProUGUI m_NotAvailableLabel;

        // Token: 0x04004268 RID: 17000
        [Header("Recommendation")]
        [SerializeField]
        public RecommendationMarkerPCView m_RecommendationObject;
        public OwlcatMultiButton m_AddButton;
        public TextMeshProUGUI m_IconText;
    }
}
