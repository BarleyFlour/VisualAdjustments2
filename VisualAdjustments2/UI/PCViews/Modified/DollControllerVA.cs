﻿using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.View;
using Kingmaker.Visual.Animation;
using Kingmaker.Visual.Animation.Kingmaker;
using Kingmaker.Visual.CharacterSystem;
using Owlcat.Runtime.Core.Utils;
using Owlcat.Runtime.UI.ConsoleTools.GamepadInput;
using Owlcat.Runtime.UniRx;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class DollControllerVA : MonoBehaviour
    {
		public void Initialize()
		{
			this.Unbind();
			FadeAnimator dollFadeAnimator = this.m_DollFadeAnimator;
			if (dollFadeAnimator != null)
			{
				dollFadeAnimator.Initialize();
			}
			if (this.m_DollPlaceHolderAnimator != null)
			{
				this.m_DollPlaceHolderAnimator.Initialize();
				this.m_DollPlaceHolderAnimator.gameObject.SetActive(true);
				this.m_DollPlaceHolderAnimator.CanvasGroup.alpha = 1f;
			}
		}
		public void Bind(UnitEntityData unit)
		{
			base.gameObject.SetActive(true);
			this.m_DollFadeAnimator.gameObject.SetActive(true);
			try
			{
				Game.Instance.UI.Common.DollRoom.SetCharacterController(this.m_CharacterController, this.m_ZoomMinValue);
				Game.Instance.UI.Common.DollRoom.SetupInfoBar(unit, false, null);
				Game.Instance.UI.Common.DollRoom.Show(unit != null && unit.View != null);
			}
			catch (Exception ex)
			{
				PFLog.Default.Exception(ex, null, Array.Empty<object>());
			}
			if (this.m_DollPlaceHolderAnimator.gameObject.activeInHierarchy)
			{
				this.m_DollPlaceHolderAnimator.CanvasGroup.alpha = 1f;
			}
			this.m_DollFadeAnimator.CanvasGroup.alpha = 0.001f;
			DelayedInvoker.InvokeInFrames(delegate
			{
				this.m_DollFadeAnimator.AppearAnimation(delegate
				{
					this.m_DollPlaceHolderAnimator.gameObject.SetActive(true);
					this.m_DollPlaceHolderAnimator.CanvasGroup.alpha = 0f;
					this.m_TextureCopy = new RenderTexture(this.m_DollRoom.texture as RenderTexture);
					this.m_DollPlaceHolder.texture = this.m_TextureCopy;
					Graphics.Blit(this.m_DollRoom.texture as RenderTexture, this.m_TextureCopy);
				});
			}, 2);
		}
		private void CreateSmoothChanger()
		{
			this.m_DollPlaceHolderAnimator.gameObject.SetActive(true);
			this.m_TextureCopy = new RenderTexture(this.m_DollRoom.texture as RenderTexture);
			this.m_DollPlaceHolder.texture = this.m_TextureCopy;
			if (this.m_Disposables == null)
			{
				this.m_Disposables = new List<IDisposable>();
				this.m_Disposables.Add(Game.Instance.UI.Common.DollRoom.DollUpdateStarted.Subscribe(delegate (Unit _)
				{
					this.m_DollPlaceHolderAnimator.gameObject.SetActive(true);
					this.m_DollPlaceHolderAnimator.AppearAnimation(null);
					Graphics.Blit(this.m_DollRoom.texture as RenderTexture, this.m_TextureCopy);
					this.m_DollFadeAnimator.DisappearAnimation(null);
				}));
				this.m_Disposables.Add(Game.Instance.UI.Common.DollRoom.DollUpdateFinished.Subscribe(delegate (Unit _)
				{
					DelayedInvoker.InvokeInFrames(delegate
					{
						this.m_DollFadeAnimator.AppearAnimation(null);
						this.m_DollPlaceHolderAnimator.DisappearAnimation(null);
					}, 5);
				}));
			}
		}
		public void UpdateDollRoomLevel(int level)
		{
			DollRoom dollRoom = Game.Instance.UI.Common.DollRoom;
			UnitEntityData unit = dollRoom.Unit;
			ClassData visualSettingsProvider = unit.Progression.GetVisualSettingsProvider();
			if (visualSettingsProvider == null)
			{
				return;
			}
			dollRoom.SetupInfo(unit, true, null);
			BlueprintClassAdditionalVisualSettings additionalVisualSettings = visualSettingsProvider.CharacterClass.GetAdditionalVisualSettings(level);
			dollRoom.GetAvatar().SetAdditionalVisualSettings(additionalVisualSettings);
		}
		public void Unbind()
		{
			this.m_DollPlaceHolderAnimator.gameObject.SetActive(false);
			base.gameObject.SetActive(false);
			Game.Instance.UI.Common.DollRoom.Show(false);
			this.ClearDisposables();
			if (this.m_TextureCopy != null)
			{
				this.m_TextureCopy.Release();
				UnityEngine.Object.Destroy(this.m_TextureCopy);
			}
		}
		private void ClearDisposables()
		{
			List<IDisposable> disposables = this.m_Disposables;
			if (disposables != null)
			{
				disposables.ForEach(delegate (IDisposable d)
				{
					d.Dispose();
				});
			}
			this.m_Disposables = null;
		}
		public void SetTransform(Transform targetTransform)
		{
			Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(base.transform.parent, targetTransform);
			RectTransform rectTransform = (RectTransform)base.transform;
			rectTransform.sizeDelta = bounds.size;
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.localPosition = new Vector3(bounds.center.x, bounds.center.y, 0f);
		}
		public InputLayer GetInputLayer(InputLayer il)
		{
			InputLayer inputLayer;
			if ((inputLayer = il) == null)
			{
				(inputLayer = new InputLayer()).ContextName = "CharacterController";
			}
			il = inputLayer;
			il.AddAxis(delegate (InputActionEventData _, float vector)
			{
				this.m_CharacterController.Rotate(vector * this.m_RotateFactor, true);
			}, 2, false);
			return il;
		}
		private DollRoomCharacterController m_CharacterController;
		private RawImage m_DollRoom;
		private FadeAnimator m_DollFadeAnimator;
		private float m_ZoomMinValue = -5f;
		private RawImage m_DollPlaceHolder;
		private FadeAnimator m_DollPlaceHolderAnimator;
		private RenderTexture m_TextureCopy;
		private List<IDisposable> m_Disposables;
		private float m_RotateFactor = 20f;
	}
}
