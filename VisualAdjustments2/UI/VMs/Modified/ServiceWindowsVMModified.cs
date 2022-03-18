using Kingmaker;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UI.MVVM._VM.ServiceWindows;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Parts;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class ServiceWindowsVMModified : BaseDisposable, IDisposable, IViewModel, IBaseDisposable, ISubscriber, IGlobalSubscriber
    {
        public ServiceWindowsVMModified()
        {
            base.AddDisposable(EventBus.Subscribe(this));
            ShowWindow(VisualWindowType.Doll);
        }
        public override void DisposeImplementation()
        {
            this.HideMenu();
            this.HideWindow(this.CurrentWindow);
            /*EventBus.RaiseEvent<IFullScreenUIHandler>(delegate (IFullScreenUIHandler h)
            {
                h.HandleFullScreenUiChanged(false, Kingmaker.UI.FullScreenUITypes.FullScreenUIType.Inventory);
            }, true);*/
        }
        /*public void HandleCloseAll()
        {
            this.HandleOpenWindow(this.CurrentWindow);
        }
        public void HandleOpenWindowOfType(VisualWindowType type)
        {
            this.HandleOpenWindow(type);
        }
        public void HandleOpenEquipment()
        {
            this.HandleOpenWindow(VisualWindowType.Equipment);
        }
        public void HandleOpenFXViewer()
        {
            this.HandleOpenWindow(VisualWindowType.FXViewer);
        }
        public void HandleOpenEEPicker()
        {
            this.HandleOpenWindow(VisualWindowType.EEPicker);
        }
        public void HandleOpenDoll()
        {
            this.HandleOpenWindow(VisualWindowType.Doll);
        }*/
        public void HandleOpenWindow(VisualWindowType type)
        {
            if (Game.Instance.Vendor.IsTrading)
            {
                return;
            }
            if (this.ServiceWindowsMenuVM.Value == null)
            {
                if (type == this.CurrentWindow || type == VisualWindowType.None)
                {
                    return;
                }
                this.ShowMenu();
            }
            ServiceWindowsMenuVMModified value = this.ServiceWindowsMenuVM.Value;
            if (value == null)
            {
                return;
            }
            //this.CurrentWindow = type;
            value.SelectWindow(type);
        }
        public void OnSelectWindow(VisualWindowType type)
        {
            //Main.Logger.Log("OnSelectWindow");
            this.HideWindow(this.CurrentWindow);
            if (type != this.CurrentWindow && type != VisualWindowType.None)
            {
                this.CurrentWindow = type;
                this.ShowWindow(type);
                this.PlayOpenSound(this.CurrentWindow);
                /*EventBus.RaiseEvent<IFullScreenUIHandler>(delegate (IFullScreenUIHandler h)
                {
                    h.HandleFullScreenUiChanged(true, Kingmaker.UI.FullScreenUITypes.FullScreenUIType.Inventory);
                }, true);*/
                return;
            }
            this.HideMenu();
            this.PlayCloseSound(this.CurrentWindow);
            EventBus.RaiseEvent<IFullScreenUIHandler>(delegate (IFullScreenUIHandler h)
            {
                h.HandleFullScreenUiChanged(false, Kingmaker.UI.FullScreenUITypes.FullScreenUIType.Inventory);
            }, true);
            this.CurrentWindow = VisualWindowType.None;
        }

        // Token: 0x0600444B RID: 17483 RVA: 0x001929EC File Offset: 0x00190BEC
        public void ShowWindow(VisualWindowType type)
        {
            //Main.Logger.Log("ShowWindow");
            try
            {
                switch (type)
                {
                    case VisualWindowType.Equipment:
                        if (this.EquipmentVM.Value == null)
                        {
                            base.AddDisposable(this.EquipmentVM.Value = new EquipmentVM(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit));
                            return;
                        }
                        return;
                    case VisualWindowType.FXViewer:
                        if (this.FXViewerVM.Value == null)
                        {
                            base.AddDisposable(this.FXViewerVM.Value = new FXViewerVM(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit));
                            return;
                        }
                        return;
                    case VisualWindowType.Doll:
                        if (this.DollVM.Value == null)
                        {
                            base.AddDisposable(this.DollVM.Value = new DollVM());
                            return;
                        }
                        return;
                    case VisualWindowType.EEPicker:
                        if (this.EEPickerVM.Value == null)
                        {
                            base.AddDisposable(this.EEPickerVM.Value = new EEPickerVM(Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit));
                            return;
                        }
                        return;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public void HideWindow(VisualWindowType type)
        {
            switch (type)
            {
                case VisualWindowType.Equipment:
                    {
                        EquipmentVM value = this.EquipmentVM.Value;
                        if (value != null)
                        {
                            value.Dispose();
                        }
                        this.EquipmentVM.Value = null;
                        return;
                    }
                case VisualWindowType.Doll:
                    {
                        DollVM value2 = this.DollVM.Value;
                        if (value2 != null)
                        {
                            value2.Dispose();
                        }
                        this.DollVM.Value = null;
                        return;
                    }
                case VisualWindowType.FXViewer:
                    {
                        FXViewerVM value3 = this.FXViewerVM.Value;
                        if (value3 != null)
                        {
                            value3.Dispose();
                        }
                        this.FXViewerVM.Value = null;
                        return;
                    }
                case VisualWindowType.EEPicker:
                    {
                        EEPickerVM value3 = this.EEPickerVM.Value;
                        if (value3 != null)
                        {
                            value3.Dispose();
                        }
                        this.EEPickerVM.Value = null;
                        return;
                    }
                default:
                    return;
            }
        }
        public void ShowMenu()
        {
            base.AddDisposable(this.ServiceWindowsMenuVM.Value = new ServiceWindowsMenuVMModified(new Action<VisualWindowType>(this.OnSelectWindow)));
        }
        public void HideMenu()
        {
            ServiceWindowsMenuVMModified value = this.ServiceWindowsMenuVM.Value;
            if (value != null)
            {
                value.Dispose();
            }
            this.ServiceWindowsMenuVM.Value = null;
        }
        public void PlayOpenSound(VisualWindowType type)
        {
            UISoundController.Instance.Play(UISoundType.InventoryOpen);
        }
        public void PlayCloseSound(VisualWindowType type)
        {
            UISoundController.Instance.Play(UISoundType.InventoryClose);
        }
        public ReactiveProperty<ServiceWindowsMenuVMModified> ServiceWindowsMenuVM = new ReactiveProperty<ServiceWindowsMenuVMModified>();

        public ReactiveProperty<EquipmentVM> EquipmentVM = new ReactiveProperty<EquipmentVM>();

        public ReactiveProperty<EEPickerVM> EEPickerVM = new ReactiveProperty<EEPickerVM>();

        public ReactiveProperty<FXViewerVM> FXViewerVM = new ReactiveProperty<FXViewerVM>();

        public ReactiveProperty<DollVM> DollVM = new ReactiveProperty<DollVM>();

        public VisualWindowType CurrentWindow;
    }
}
