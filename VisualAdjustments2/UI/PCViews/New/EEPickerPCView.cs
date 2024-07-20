using Kingmaker;
using Kingmaker.BundlesLoading;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.UnitLogic;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using Owlcat.Runtime.UI.Controls.Button;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.Visual.CharacterSystem;
using Owlcat.Runtime.UniRx;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Inventory.VisualSettings;

namespace VisualAdjustments2.UI
{
    public class EEPickerPCView : VisualAdjustments2ServiceWindowVM<EEPickerVM>
    {
        public void Initialize()
        {

        }
        public void ResetChanges()
        {
            this.ViewModel.ResetChanges();
            m_CurrentEEs.Bind(base.ViewModel.CurrentEEs.Value);
            this.AddDisposable(base.ViewModel.CurrentEEs.Value);

            this.m_dollCharacterController.Unbind();

            this.m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);

            m_EEColorPicker.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            this.m_VisualSettings.gameObject.SetActive(false);
            base.AddDisposable(this.ViewModel?.CurrentEEs?.Value?.SelectedEntity?.Subscribe((ListViewItemVM vm) => { this.m_EEColorPicker.OnEEChanged(ResourcesLibrary.TryGetResource<EquipmentEntity>(vm.Guid)); }));
            
        }
        private void OnCharacterChanged()
        {
            m_CurrentEEs.Bind(base.ViewModel.CurrentEEs.Value);
            this.AddDisposable(base.ViewModel.CurrentEEs.Value);
            m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            m_EEColorPicker.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
            this.m_VisualSettings.gameObject.SetActive(false);
            base.AddDisposable(this.ViewModel?.CurrentEEs?.Value?.SelectedEntity?.Subscribe((ListViewItemVM vm) => { this.m_EEColorPicker.OnEEChanged(ResourcesLibrary.TryGetResource<EquipmentEntity>(vm.Guid)); }));
        }
        public override void BindViewImplementation()
        {
            try
            {
                base.ViewModel.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(delegate (UnitReference _)
                {
                    this.OnCharacterChanged();
                }));

                base.BindViewImplementation();
                Initialize();
                m_EEColorPicker.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
                this.m_EEColorPicker.m_ConfirmButton.OnLeftClick.RemoveAllListeners();
                m_EEColorPicker.m_ConfirmButton.OnLeftClick.AddListener(() =>
                {
                    if (m_EEColorPicker.CustomColor)
                        this.ViewModel?.ApplyColor(m_EEColorPicker.m_Color.m_ToColor.color, m_EEColorPicker.PrimaryOrSecondary);
                    else
                        this.ViewModel?.ApplyColor(m_EEColorPicker.Index.Value, m_EEColorPicker.PrimaryOrSecondary);

                });
                this.AddDisposable(this.m_EEColorPicker.Index.Subscribe((int index) => { this.m_EEColorPicker.UpdateColorFromIndex(ResourcesLibrary.TryGetResource<EquipmentEntity>(this.ViewModel.CurrentEEs.Value.SelectedEntity.Value.Guid), index); }));
                this.AddDisposable(this.m_EEColorPicker.m_ToggleGroupHandler.PrimOrSec.Subscribe((bool _) => { this.m_EEColorPicker.UpdateRampSlider(ResourcesLibrary.TryGetResource<EquipmentEntity>(this.ViewModel.CurrentEEs.Value.SelectedEntity.Value.Guid)); }));
                base.AddDisposable(this.ViewModel?.CurrentEEs?.Value?.SelectedEntity?.Subscribe((ListViewItemVM vm) => { this.m_EEColorPicker.OnEEChanged(ResourcesLibrary.TryGetResource<EquipmentEntity>(vm.Guid)); }));
                
                m_dollCharacterController.Bind(Game.Instance.SelectionCharacter.SelectedUnit.Value.Value);
                this.AddDisposable(base.ViewModel.AllEEs.Value);
                this.m_AllEEs.Bind(base.ViewModel.AllEEs.Value);
                this.AddDisposable(base.ViewModel.CurrentEEs.Value);
                this.m_CurrentEEs.Bind(base.ViewModel.CurrentEEs.Value);
                this.m_VisualSettings.gameObject.SetActive(false);
            }
            catch(Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }

        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            m_EEColorPicker.Dispose();
            this.m_dollCharacterController.Unbind();
        }
        public CharacterVisualSettingsPCView m_VisualSettings;
        public DollCharacterController m_dollCharacterController;
        public ListPCView m_AllEEs;
        public ListPCView m_CurrentEEs;
        public ListViewItemPCView template;
        public OwlcatButton m_ApplyButton;
        public EEColorPickerView m_EEColorPicker;
        public OwlcatButton m_ResetButton;
        public OwlcatButton m_ResetAllButton;
    }
}
