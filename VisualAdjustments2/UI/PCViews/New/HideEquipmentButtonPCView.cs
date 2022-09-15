using Kingmaker.UI.Common;
using Kingmaker.UI.ServiceWindow;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Inventory.VisualSettings;
using TMPro;
using UniRx;
using UnityEngine;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class HideEquipmentButtonPCView : MonoBehaviour, IDisposable
    {
        public void SetOption(bool state)
        {
            var settings = Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value?.GetSettings();
            if (settings == null) return;
            switch (type)
            {
                case HideButtonType.Class_Equipment:
                {
                    settings.HideEquipmentDict[(ItemsFilter.ItemType)type] = state;
                    Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value?.View?.CharacterAvatar
                        .UpdateBackpackVisibility(state);
                    break;
                }
                case HideButtonType.Mythic_Things:
                {
                    settings.HideEquipmentDict[(ItemsFilter.ItemType)type] = state;
                    if (state == true)
                    {
                        Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value?.View?.CharacterAvatar
                            ?.SetAdditionalVisualSettings(null);
                        Kingmaker.Game.Instance.UI?.Common?.DollRoom?.m_Avatar?.SetAdditionalVisualSettings(null);
                        // Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value?.Unit?.View?.CharacterAvatar.ApplyAdditionalVisualSettings();
                        // Kingmaker.Game.Instance.UI?.Common?.DollRoom?.m_Avatar?.ApplyAdditionalVisualSettings();
                    }
                    else
                    {
                        var charclass = Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value
                            .Progression?.GetVisualSettingsProvider()?.GetAdditionalVisualSettings();
                        Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value?.View?.CharacterAvatar
                            ?.SetAdditionalVisualSettings(charclass);
                        Kingmaker.Game.Instance.UI?.Common?.DollRoom?.m_Avatar?.SetAdditionalVisualSettings(charclass);
                        // Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value?.Unit?.View?.CharacterAvatar.ApplyAdditionalVisualSettings();
                        // Kingmaker.Game.Instance.UI?.Common?.DollRoom?.m_Avatar?.ApplyAdditionalVisualSettings();
                    }

                    break;
                }
                case HideButtonType.Usable:
                {
                    settings.HideEquipmentDict[(ItemsFilter.ItemType)type] = state;
                    Kingmaker.Game.Instance?.SelectionCharacter?.CurrentSelectedCharacter?.View?.HandsEquipment?.UpdateBeltPrefabs();
                    Kingmaker.Game.Instance.UI?.Common?.DollRoom?.AvatarHands?.UpdateBeltPrefabs();
                    break;
                }
                default:
                {
                    settings.HideEquipmentDict[(ItemsFilter.ItemType)type] = state;
                    break;
                }
            }

            Kingmaker.Game.Instance?.SelectionCharacter?.CurrentSelectedCharacter?.View?.VAUpdate();
        }

        public void SetupFromVisualSettingsView(CharacterVisualSettingsEntityPCView old)
        {
            this.m_Toggle = old.m_Button;
            this.m_Label = old.m_Label;
            UnityEngine.Component.DestroyImmediate(old);
        }

        bool GetCheckState()
        {
            var settings = Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value?.GetSettings();
            if (settings == null) return false;
            if (settings.HideEquipmentDict.TryGetValue((ItemsFilter.ItemType)type, out var value))
            {
                return value;
            }
            else
            {
                settings.HideEquipmentDict.Add((ItemsFilter.ItemType)type, false);
                return false;
            }
            // return settings.HideEquipmentDict[(ItemsFilter.ItemType)type];
        }

        public void Bind(string label, HideButtonType buttonType)
        {
            this.type = buttonType;
            this.m_Label.text = label;
            //this.m_OnChangeStateAction = action;
            //this.m_GetCheckState = getState;
            IDisposable onChangeStateDisposable = this.m_OnChangeStateDisposable;
            if (onChangeStateDisposable != null)
            {
                onChangeStateDisposable.Dispose();
            }

            IDisposable onChangeVisualDisposable = this.m_OnChangeVisualDisposable;
            if (onChangeVisualDisposable != null)
            {
                onChangeVisualDisposable.Dispose();
            }

            this.IsOnState.Value = this.GetCheckState();
            this.m_OnChangeVisualDisposable = this.IsOnState.Subscribe(delegate(bool state)
            {
                //Action onChangeStateAction = this.m_OnChangeStateAction;
                //if (onChangeStateAction != null)
                {
                    SetOption(state);
                }
                this.m_Toggle.SetActiveLayer(state ? 1 : 0);
            });
            this.m_OnChangeStateDisposable = this.m_Toggle.OnLeftClickAsObservable().Subscribe(delegate(Unit _)
            {
                this.IsOnState.Value = !this.IsOnState.Value;
            });
        }

        public void Dispose()
        {
            this.m_OnChangeStateDisposable?.Dispose();
            this.m_OnChangeVisualDisposable?.Dispose();
        }

        public HideButtonType type;
        public OwlcatMultiButton m_Toggle;
        public TextMeshProUGUI m_Label;
        public BoolReactiveProperty IsOnState = new BoolReactiveProperty();
        public IDisposable m_OnChangeStateDisposable;
        public IDisposable m_OnChangeVisualDisposable;
    }
}