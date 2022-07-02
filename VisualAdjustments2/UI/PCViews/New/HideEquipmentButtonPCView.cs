using Kingmaker.UI.Common;
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
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class HideEquipmentButtonPCView : MonoBehaviour, IDisposable
    {
        public void SetOption(bool state)
        {
            var settings = Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value?.Unit?.GetSettings();
            if (settings == null) return;
            switch (type)
            {
                case HideButtonType.Class_Equipment:
                    {
                        settings.HideEquipmentDict[(ItemsFilter.ItemType)type] = state;
                        Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value?.Unit?.View?.CharacterAvatar.UpdateBackpackVisibility(state);
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
        public void SetupFromVisualSettingsView(CharacterVisualSettingsEntityView old)
        {
            this.m_Toggle = old.m_Toggle;
            this.m_Label = old.m_Label;
            UnityEngine.Component.DestroyImmediate(old);
        }
        bool GetCheckState()
        {
            var settings = Kingmaker.Game.Instance?.SelectionCharacter?.SelectedUnit?.Value?.Unit?.GetSettings();
            if (settings == null) return false;
            return settings.HideEquipmentDict[(ItemsFilter.ItemType)type];
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
            this.m_OnChangeVisualDisposable = this.IsOnState.Subscribe(delegate (bool state)
            {

                //Action onChangeStateAction = this.m_OnChangeStateAction;
                //if (onChangeStateAction != null)
                {

                    SetOption(state);
                }
                this.m_Toggle.SetActiveLayer(state ? 1 : 0);
            });
            this.m_OnChangeStateDisposable = this.m_Toggle.OnLeftClickAsObservable().Subscribe(delegate (Unit _)
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
