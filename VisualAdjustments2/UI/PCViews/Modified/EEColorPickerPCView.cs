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

namespace VisualAdjustments2.UI
{
	public static class EEColorPickerPCViewExtensions
    {
		public static void SetupFromVisualSettings(this EEColorPickerPCView newcomp, CharacterVisualSettingsView oldcomp)
		{
			newcomp.m_SettingsBtn = oldcomp.m_SettingsBtn;
			newcomp.m_Title = oldcomp.m_Title;
			newcomp.m_Window = oldcomp.m_Window;
			newcomp.m_WindowShow = oldcomp.m_WindowShow;
			newcomp.m_OnClickDispose = oldcomp.m_OnClickDispose;
			var windowcontainer = newcomp.transform.Find("WindowContainer");
			var layout = windowcontainer.gameObject.GetComponent<VerticalLayoutGroup>();
			layout.childControlWidth = true;
			layout.childScaleWidth = true;
			layout.spacing = -15;
			var sizefitter = windowcontainer.gameObject.GetComponent<ContentSizeFitterExtended>();
			sizefitter.m_VerticalFit = ContentSizeFitterExtended.FitMode.MinSize;
			windowcontainer.Find("ShowBackpackContainer").gameObject.SetActive(false);
			windowcontainer.Find("ShowHelmContainer").gameObject.SetActive(false);
			windowcontainer.Find("ShowClothContainer").gameObject.SetActive(false);
		}
	}
	public class EEColorPickerPCView : MonoBehaviour, IDisposable
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
			for(int i = 0; i < 256; i++)
            {
				var SSE = new StringSequentialEntity();
				int x = i;
				SSE.Setter = () =>
				{
					if(this.m_R_Slider.ViewModel != null) this.m_R = this.m_R_Slider.ViewModel.CurrentIndex.Value;
					this.UpdateColor();
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
				};
				colorlistB.Add(SSE);

			}
			this.InitWindowAndControlls(unit != null);
			this.m_R_Slider.Bind(new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(colorlistR));
			this.m_G_Slider.Bind(new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(colorlistG));
			this.m_B_Slider.Bind(new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(colorlistB));
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

        private void UpdateColor()
        {
			this.m_Color.m_ToColor.color = new Color( ((float)this.m_R)/256, ((float)this.m_G)/256, ((float)this.m_B)/256);
        }

        // Token: 0x06003C9C RID: 15516 RVA: 0x000ECDC4 File Offset: 0x000EAFC4
        private void InitWindowAndControlls(bool state)
		{
			this.m_Window.Initialize();
			this.m_Window.gameObject.SetActive(false);
			this.m_WindowShow = false;
			base.gameObject.SetActive(state);
			this.m_Title.text = "Colour";
			IDisposable onClickDispose = this.m_OnClickDispose;
			if (onClickDispose != null)
			{
				onClickDispose.Dispose();
			}
			this.m_OnClickDispose = this.m_SettingsBtn.OnLeftClickAsObservable().Subscribe(new Action(this.OnSettingsBtnClick));
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
			this.m_Window.DisappearAnimation(delegate
			{
				this.m_Window.gameObject.SetActive(false);
			});
		}
		private void ShowWindow()
		{
			this.m_WindowShow = true;
			this.m_Window.gameObject.SetActive(true);
			this.m_Window.AppearAnimation(null);
			this.HasPlayerSeenWindow = true;
		}
		public void Dispose()
		{
			this.InitWindowAndControlls(false);
			this.m_OnClickDispose.Dispose();
		}
		public bool PrimaryOrSecondary => m_ToggleGroupHandler.PrimOrSec.Value;
		[SerializeField]
		[UsedImplicitly]
		public OwlcatButton m_SettingsBtn;
		[SerializeField]
		[UsedImplicitly]
		public WindowAnimator m_Window;
		[SerializeField]
		[UsedImplicitly]
		public TextMeshProUGUI m_Title;
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

		public int m_R = 0;
		public int m_G = 0;
		public int m_B = 0;
	}
}
