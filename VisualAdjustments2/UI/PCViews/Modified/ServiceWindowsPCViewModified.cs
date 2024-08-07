﻿using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.ServiceWindows;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Menu;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class ServiceWindowsPCViewModified : ViewBase<ServiceWindowsVMModified>
    {
        public ServiceWindowsPCViewModified(ServiceWindowsPCView oldview, ServiceWindowMenuPCViewModified SWMPCVM)
        {
            this.m_Background = oldview.m_Background;
            this.m_BindDisposable = oldview.m_BindDisposable;
            this.m_ServiceWindowMenuPcView = SWMPCVM;
        }

        public void Initialize()
        {
            if (this.m_IsInit)
            {
                return;
            }

            this.m_FXViewerPCView?.Initialize();
            this.m_EquipmentPCView?.Initialize();
            this.m_ServiceWindowMenuPcView?.Initialize();
            this.m_IsInit = true;
        }

        public override void BindViewImplementation()
        {
            base.AddDisposable(base.ViewModel.ServiceWindowsMenuVM.Subscribe(
                new Action<ServiceWindowsMenuVMModified>(this.m_ServiceWindowMenuPcView.Bind)));
            base.AddDisposable(base.ViewModel.EEPickerVM.Subscribe(new Action<EEPickerVM>(this.m_EEPickerPCView.Bind)));
            base.AddDisposable(base.ViewModel.FXViewerVM.Subscribe(new Action<FXViewerVM>(this.m_FXViewerPCView.Bind)));
            base.AddDisposable(
                base.ViewModel.EquipmentVM.Subscribe(new Action<EquipmentVM>(this.m_EquipmentPCView.Bind)));
            base.AddDisposable(base.ViewModel.DollVM.Subscribe(new Action<DollVM>(this.m_DollPCView.Bind)));
            base.AddDisposable(
                base.ViewModel.PortraitVM.Subscribe(new Action<PortraitPickerVM>(this.m_portraitPickerPCView.Bind)));
            base.AddDisposable(base.ViewModel.ServiceWindowsMenuVM.Subscribe(delegate(ServiceWindowsMenuVMModified vm)
            {
#if DEBUG
                var isnullstring = vm == null ? "Null" : "Not null";
                Main.Logger.Log($"ServiceWindowsPCViewModified BindViewImplementation, vm is {isnullstring}");
#endif
                if (vm != null)
                {
                    //this.m_Background?.AppearAnimation(null);
                    this.gameObject.SetActive(true);
                    return;
                }
                //this.m_Background?.DisappearAnimation(null);
            }));
        }

        public override void DestroyViewImplementation()
        {
            //this.gameObject.SetActive(false);
        }

        public PortraitPickerPCView m_portraitPickerPCView;
        public EEPickerPCView m_EEPickerPCView;
        public FXViewerPCView m_FXViewerPCView;
        public EquipmentPCView m_EquipmentPCView;
        public DollPCView m_DollPCView;
        public ServiceWindowMenuPCViewModified m_ServiceWindowMenuPcView;
        public DollCharacterController m_DollRoom;
        public FadeAnimator m_Background;
        public bool m_IsInit;
    }
}