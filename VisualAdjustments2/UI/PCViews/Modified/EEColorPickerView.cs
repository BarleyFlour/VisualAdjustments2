using JetBrains.Annotations;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.ServiceWindow;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using Owlcat.Runtime.UniRx;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using UnityEngine.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Common;
using VisualAdjustments2.Infrastructure;
using Kingmaker.Visual.CharacterSystem;

namespace VisualAdjustments2.UI
{
    public class EEColorPickerView : MonoBehaviour, IDisposable
    {
        public void Intialize()
        {
            this.InitWindowAndControlls(false);
            this.ResetPlayerSeenWindow();
        }

        public void ResetPlayerSeenWindow()
        {
            this.HasPlayerSeenWindow = false;
        }

        public void Bind(UnitEntityData unit)
        {
            var colorlistR = new List<StringSequentialEntity>();
            for (int i = 0; i < 256; i++)
            {
                var SSE = new StringSequentialEntity();
                int x = i;
                SSE.Setter = () =>
                {
                    if (this.m_R_Slider.ViewModel != null) this.m_R = this.m_R_Slider.ViewModel.CurrentIndex.Value;
                    this.UpdateColor();
                    this.CustomColor = true;
                };
                colorlistR.Add(SSE);
            }

            var colorlistG = new List<StringSequentialEntity>();
            for (int i = 0; i < 256; i++)
            {
                var SSE = new StringSequentialEntity();
                int x = i;
                SSE.Setter = () =>
                {
                    if (this.m_G_Slider.ViewModel != null) this.m_G = this.m_G_Slider.ViewModel.CurrentIndex.Value;
                    this.UpdateColor();
                    this.CustomColor = true;
                };
                colorlistG.Add(SSE);
            }

            var colorlistB = new List<StringSequentialEntity>();
            for (int i = 0; i < 256; i++)
            {
                var SSE = new StringSequentialEntity();
                int x = i;
                SSE.Setter = () =>
                {
                    if (this.m_B_Slider.ViewModel != null) this.m_B = this.m_B_Slider.ViewModel.CurrentIndex.Value;
                    this.UpdateColor();
                    this.CustomColor = true;
                };
                colorlistB.Add(SSE);
            }

            /*var seqlist = new List<StringSequentialEntity>();
            /*for (int i = 0; i < 81; i++)
            {
                int x = i;
                var entity = new StringSequentialEntity();
                entity.Title = i.ToString();
                entity.Setter = () => {
                    this.CustomColor = false; this.Index.Value = x;
                };
                seqlist.Add(entity);
            }
            var StringSequentialVM = new StringSequentialSelectorVM(seqlist);
            this.m_Ramp_Slider.Bind(StringSequentialVM);*/
            this.InitWindowAndControlls(unit != null);
            this.m_R_Slider.Bind(
                new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(colorlistR));
            this.m_G_Slider.Bind(
                new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(colorlistG));
            this.m_B_Slider.Bind(
                new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(colorlistB));


            /*if (unit != null)
            {
                this.m_HelmEntityView.Bind(UIStrings.Instance.CharacterSheet.VisualSettingsShowHelmet, delegate
                {
                    unit.UISettings.ShowHelm = this.m_HelmEntityView.IsOn;
                }, () => unit.UISettings.ShowHelm);
                this.m_BackpackEntityView.Bind(UIStrings.Instance.CharacterSheet.VisualSettingsShowBackpack, delegate
                {
                    unit.UISettings.ShowBackpack = this.m_BackpackEntityView.IsOn;
                }, () => unit.UISettings.ShowBackpack);
                this.m_EquipmentEntityView.Bind(UIStrings.Instance.CharacterSheet.VisualSettingsShowEquipment, delegate
                {
                    unit.UISettings.ShowClassEquipment = this.m_EquipmentEntityView.IsOn;
                }, () => unit.UISettings.ShowClassEquipment);
                return;
            }
            CharacterVisualSettingsEntityView characterVisualSettingsEntityView = this.m_ClothEntityView.Or(null);
            if (characterVisualSettingsEntityView != null)
            {
                characterVisualSettingsEntityView.Bind(string.Empty, null, null);
            }
            this.m_HelmEntityView.Bind(string.Empty, null, null);
            this.m_BackpackEntityView.Bind(string.Empty, null, null);
            this.m_EquipmentEntityView.Bind(string.Empty, null, null);*/
        }

        public void UpdateRampSlider(EquipmentEntity ee)
        {
#if DEBUG
            Main.Logger.Log($"UpdateRampSlider Beginning: CustomColor:{CustomColor}");
#endif
            var CustomColorAtStart = CustomColor;

            bool hasPrimRamp = ee.PrimaryColorsProfile?.Ramps?.Count is not null and > 0;
            bool hasSecondaryRamp = ee.SecondaryColorsProfile?.Ramps?.Count is not null and > 0;

            if (hasPrimRamp && this.PrimaryOrSecondary == true)
            {
                var seqlist = new List<StringSequentialEntity>();
                for (int i = 0; i < ee.PrimaryColorsProfile.Ramps.Where(t => t.name != "CustomTex").Count(); i++)
                {
                    int x = i;
                    var entity = new StringSequentialEntity();
                    entity.Title = i.ToString();
                    entity.Setter = () =>
                    {
                        this.CustomColor = false;
                        this.Index.Value = x;
                    };
                    seqlist.Add(entity);
                }

                var StringSequentialVM = new StringSequentialSelectorVM(seqlist);
                this.m_Ramp_Slider.Bind(StringSequentialVM);
            }

            if (hasSecondaryRamp && this.PrimaryOrSecondary == false)
            {
                var seqlist = new List<StringSequentialEntity>();
                for (int i = 0; i < ee.SecondaryColorsProfile.Ramps.Where(t => t.name != "CustomTex").Count(); i++)
                {
                    int x = i;
                    var entity = new StringSequentialEntity();
                    entity.Title = i.ToString();
                    entity.Setter = () =>
                    {
                        this.CustomColor = false;
                        this.Index.Value = x;
                    };
                    seqlist.Add(entity);
                }

                var StringSequentialVM = new StringSequentialSelectorVM(seqlist);
                this.m_Ramp_Slider.Bind(StringSequentialVM);
            }
#if DEBUG
            Main.Logger.Log($"UpdateRampSlider End: CustomColor:{CustomColor}");
#endif
            CustomColor =
                CustomColorAtStart; // the binding of the ramp sliders seems to call the setter, Which causes custom color to always turn false when switching.

            if (this.CustomColor)
            {
                this.UpdateColor();
            }
            else
            {
                this.UpdateColorFromIndex(ee, 0);
            }
        }

        public void OnEEChanged(EquipmentEntity ee)
        {
#if DEBUG
            Main.Logger.Log($"EEChanged to {ee.name}");
#endif
            bool hasPrimRamp = ee.PrimaryColorsProfile?.Ramps?.Count is not null and > 0;
            bool hasSecondaryRamp = ee.SecondaryColorsProfile?.Ramps?.Count is not null and > 0;

            if (hasPrimRamp && this.PrimaryOrSecondary == true)
            {
                var seqlist = new List<StringSequentialEntity>();
                for (int i = 0; i < ee.PrimaryColorsProfile.Ramps.Where(t => t.name != "CustomTex").Count(); i++)
                {
                    int x = i;
                    var entity = new StringSequentialEntity();
                    entity.Title = i.ToString();
                    entity.Setter = () =>
                    {
                        this.CustomColor = false;
                        this.Index.Value = x;
                    };
                    seqlist.Add(entity);
                }

                var StringSequentialVM = new StringSequentialSelectorVM(seqlist);
                this.m_Ramp_Slider.Bind(StringSequentialVM);
            }

            if (hasSecondaryRamp && this.PrimaryOrSecondary == false)
            {
                var seqlist = new List<StringSequentialEntity>();
                for (int i = 0; i < ee.SecondaryColorsProfile.Ramps.Where(t => t.name != "CustomTex").Count(); i++)
                {
                    int x = i;
                    var entity = new StringSequentialEntity();
                    entity.Title = i.ToString();
                    entity.Setter = () =>
                    {
                        this.CustomColor = false;
                        this.Index.Value = x;
                    };
                    seqlist.Add(entity);
                }

                var StringSequentialVM = new StringSequentialSelectorVM(seqlist);
                this.m_Ramp_Slider.Bind(StringSequentialVM);
            }
            //this.m_R_Slider.LockControls(false);
            //this.m_G_Slider.LockControls(false);
            //this.m_B_Slider.LockControls(false);

            if (!hasPrimRamp && !hasSecondaryRamp)
            {
                this.m_Ramp_Slider.LockControls(false);
                this.m_ConfirmButton.SetInteractable(false);
                this.m_ToggleGroupHandler.m_Primary_Button.SetInteractable(false);
                this.m_ToggleGroupHandler.m_Secondary_Button.SetInteractable(false);
                this.m_ToggleGroupHandler.PrimOrSec.Value = true;
                this.m_ToggleGroupHandler.ShowSelected.Value = false;
            }
            else if (hasPrimRamp && hasSecondaryRamp)
            {
                this.m_Ramp_Slider.LockControls(true);
                this.m_ConfirmButton.SetInteractable(true);
                this.m_ToggleGroupHandler.m_Primary_Button.SetInteractable(true);
                this.m_ToggleGroupHandler.m_Secondary_Button.SetInteractable(true);
                this.m_ToggleGroupHandler.PrimOrSec.Value = true;
                this.m_ToggleGroupHandler.ShowSelected.Value = true;
            }
            else if (hasPrimRamp && !hasSecondaryRamp)
            {
                this.m_Ramp_Slider.LockControls(true);
                this.m_ConfirmButton.SetInteractable(true);
                this.m_ToggleGroupHandler.m_Primary_Button.SetInteractable(true);
                this.m_ToggleGroupHandler.m_Secondary_Button.SetInteractable(false);
                this.m_ToggleGroupHandler.PrimOrSec.Value = true;
                this.m_ToggleGroupHandler.ShowSelected.Value = true;
            }
            else if (!hasPrimRamp && hasSecondaryRamp)
            {
                this.m_Ramp_Slider.LockControls(true);
                this.m_ConfirmButton.SetInteractable(true);
                this.m_ToggleGroupHandler.m_Primary_Button.SetInteractable(false);
                this.m_ToggleGroupHandler.m_Secondary_Button.SetInteractable(true);
                this.m_ToggleGroupHandler.PrimOrSec.Value = false;
                this.m_ToggleGroupHandler.ShowSelected.Value = true;
            }
            //Lock controls in relation to Prim/Sec ramps and select first available ramp in the Prim/Sec switcher

            if (this.CustomColor)
            {
                this.UpdateColor();
            }
            else
            {
                this.UpdateColorFromIndex(ee, 0);
            }
        }

        private void UpdateColor()
        {
            this.m_Color.m_ToColor.sprite = null;
            this.m_Color.m_ToColor.color =
                new Color(((float)this.m_R) / 256, ((float)this.m_G) / 256, ((float)this.m_B) / 256);
        }

        public void UpdateColorFromIndex(EquipmentEntity ee, int index)
        {
            {
                this.m_Color.m_ToColor.color = new Color(1, 1, 1);
                var texture = this.PrimaryOrSecondary
                    ? ee?.PrimaryColorsProfile?.Ramps?[index]
                    : ee?.SecondaryColorsProfile?.Ramps?[index];
                if (texture == null) return;
                //Ramps are a gradient for some reason, darker on the left brighter on the right, 75 seems to be about right so that the preview matches the applied colour.
                Rect rect = new Rect(75f, 0f, 1f, 1f);
                this.m_Color.m_ToColor.sprite = Sprite.Create(texture, rect, Vector2.zero);
            }
        }

        // Token: 0x06003C9C RID: 15516 RVA: 0x000ECDC4 File Offset: 0x000EAFC4
        private void InitWindowAndControlls(bool state)
        {
            this.m_FadeAnimator.Initialize();
            this.m_FadeAnimator.gameObject.SetActive(false);
            this.m_WindowShow = false;
            base.gameObject.SetActive(state);
            this.m_Header.text = "Colour";
            IDisposable onClickDispose = this.m_OnClickDispose;
            if (onClickDispose != null)
            {
                onClickDispose.Dispose();
            }

            this.m_OnClickDispose = this.m_SettingsBtn.OnLeftClickAsObservable()
                .Subscribe(new Action(this.OnSettingsBtnClick));
        }

        private void OnSettingsBtnClick()
        {
            bool windowShow = this.m_WindowShow;
            if (windowShow)
            {
                this.HideWindow();
                return;
            }

            this.ShowWindow();
        }

        public void ShowIfNotSeenAndSwitchClothTo(bool state)
        {
            this.ShowIfNotSeen();
        }

        public void ShowIfNotSeen()
        {
            if (!this.HasPlayerSeenWindow)
            {
                this.ShowWindow();
                this.HasPlayerSeenWindow = true;
            }
        }

        private void HideWindow()
        {
            this.m_WindowShow = false;
            this.m_FadeAnimator.DisappearAnimation(delegate { this.m_FadeAnimator.gameObject.SetActive(false); });
        }

        private void ShowWindow()
        {
            this.m_WindowShow = true;
            this.m_FadeAnimator.gameObject.SetActive(true);
            this.m_FadeAnimator.AppearAnimation(null);
            this.HasPlayerSeenWindow = true;
        }

        public void Dispose()
        {
            this.InitWindowAndControlls(false);
            this.m_OnClickDispose.Dispose();
        }

        public bool PrimaryOrSecondary => m_ToggleGroupHandler.PrimOrSec.Value;
        [SerializeField] [UsedImplicitly] public OwlcatButton m_SettingsBtn;
        public FadeAnimator m_FadeAnimator;
        public TextMeshProUGUI m_Header;
        public bool m_WindowShow;
        public bool m_Init;
        public IDisposable m_OnClickDispose;
        public bool HasPlayerSeenWindow;
        public BarleySlideSelectorPCView m_R_Slider;
        public BarleySlideSelectorPCView m_G_Slider;
        public BarleySlideSelectorPCView m_B_Slider;
        public ToggleGroupHandler m_ToggleGroupHandler;
        public ColorPreviewView m_Color;
        public OwlcatButton m_ConfirmButton;
        public BarleySlideSelectorPCView m_Ramp_Slider;

        public int m_R = 0;
        public int m_G = 0;
        public int m_B = 0;
        public bool CustomColor = false;
        public ReactiveProperty<int> Index = new();
    }
}