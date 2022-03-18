using Kingmaker.UI.MVVM._VM.CharGen.Phases.Common;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UniRx;
using Owlcat.Runtime.UniRx;
using UnityEngine;
using UnityEngine.UI;
using Kingmaker.Utility;
using JetBrains.Annotations;
using TMPro;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;

namespace VisualAdjustments2.UI

{
    public class BarleySlideSelectorPCView : ViewBase<StringSequentialSelectorVM>, IScrollHandler, IEventSystemHandler
    {
        public string m_Prefix;
        public bool IsActive
        {
            get
            {
                StringSequentialSelectorVM viewModel = base.ViewModel;
                return viewModel != null && viewModel.Active.Value;
            }
        }
        public void Initialize()
        {
            base.gameObject.SetActive(false);
            this.m_Slider.minValue = 1f;
            this.m_Slider.wholeNumbers = true;
            this.m_Slider.maxValue = 10f;
        }
        public void SetOnChangeCallback(Action callback)
        {
            this.m_Callback = callback;
        }
        public override void BindViewImplementation()
        {
            this.m_Slider.maxValue = (float)base.ViewModel.TotalCount;
            if (this.m_ButtonNext != null)
            {
                base.AddDisposable(this.m_ButtonNext.OnLeftClickAsObservable().Subscribe(delegate (Unit _)
                {
                    this.OnNextHandler();
                }));
            }
            if (this.m_ButtonPrevious != null)
            {
                base.AddDisposable(this.m_ButtonPrevious.OnLeftClickAsObservable().Subscribe(delegate (Unit _)
                {
                    this.OnPreviousHandler();
                }));
            }
            base.AddDisposable(base.ViewModel.CurrentIndex.Subscribe(delegate (int index)
            {
                this.SetCurrentIndex(index);
            }));
            base.AddDisposable(base.ViewModel.Active.Subscribe(delegate (bool value)
            {
                this.m_ButtonNext.SetInteractable(value);
                this.m_ButtonPrevious.SetInteractable(value);
                this.m_Slider.interactable = value;
                base.gameObject.SetActive(value);
            }));
            base.AddDisposable(this.m_Slider.OnValueChangedAsObservable().Subscribe(new Action<float>(this.OnSliderChangedValue)));
            DelayedInvoker.InvokeInFrames(new Action(this.CalculateHandleSize), 1);
        }
        public void CalculateHandleSize()
        {
            float num = 1f;
            if (this.m_Slider.maxValue >= 1f)
            {
                num = this.m_Slider.maxValue;
            }
            if (this.m_CalculateHandleSize)
            {
                this.m_Slider.handleRect.sizeDelta = new Vector2(this.m_SliderRect.sizeDelta.x / num, this.m_Slider.handleRect.sizeDelta.y);
                this.m_SliderSlideArea.offsetMin = new Vector2(this.m_Slider.handleRect.sizeDelta.x / 2f, this.m_SliderSlideArea.offsetMin.y);
                this.m_SliderSlideArea.offsetMax = new Vector2(-this.m_Slider.handleRect.sizeDelta.x / 2f, this.m_SliderSlideArea.offsetMin.y);
            }
        }
        public void OnSliderChangedValue(float value)
        {
            int currentIndex = Mathf.RoundToInt(value) - 1;
            base.ViewModel.SetCurrentIndex(currentIndex);
        }
        public void SetCurrentIndex(int index)
        {
            if (this.m_Value != null)
            {
                this.m_Value.text = base.ViewModel.Value.Value;
            }
            if (this.m_Counter != null)
            {
                this.m_Counter.text = string.Format("{1} {0}", index,m_Prefix);
            }
            this.m_Slider.value = (float)(index + 1);
        }
        public void SetTitleText(string title)
        {
            if (string.IsNullOrEmpty(title) || this.m_Label == null)
            {
                return;
            }
            this.m_Label.text = title;
        }
        public bool OnPreviousHandler()
        {
            Action callback = this.m_Callback;
            if (callback != null)
            {
                callback();
            }
            base.ViewModel.OnLeft();
            return true;
        }
        public bool OnNextHandler()
        {
            Action callback = this.m_Callback;
            if (callback != null)
            {
                callback();
            }
            base.ViewModel.OnRight();
            return true;
        }
        public override void DestroyViewImplementation()
        {
            this.m_Callback = null;
            base.gameObject.SetActive(false);
        }
        public void OnScroll(PointerEventData eventData)
        {
            if (eventData.scrollDelta.y > 0f)
            {
                this.OnPreviousHandler();
                return;
            }
            this.OnNextHandler();
        }
        public Slider m_Slider;
        public bool m_CalculateHandleSize = true;
        [SerializeField]
        [ConditionalShow("m_CalculateHandleSize")]
        public RectTransform m_SliderRect;
        [SerializeField]
        [ConditionalShow("m_CalculateHandleSize")]
        public RectTransform m_SliderSlideArea;
        [SerializeField]
        [CanBeNull]
        public OwlcatMultiButton m_ButtonNext;
        [SerializeField]
        [CanBeNull]
        public OwlcatMultiButton m_ButtonPrevious;
        [SerializeField]
        [CanBeNull]
        public TextMeshProUGUI m_Value;
        [SerializeField]
        [CanBeNull]
        public TextMeshProUGUI m_Counter;
        [SerializeField]
        [CanBeNull]
        public TextMeshProUGUI m_Label;
        public Action m_Callback;
    }
}
