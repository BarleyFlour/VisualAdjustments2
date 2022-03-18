using Kingmaker;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM;
using Kingmaker.UI.MVVM._VM.ServiceWindows;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Menu;
using Kingmaker.UnitLogic;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.SelectionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using VisualAdjustments2.UI;

namespace VisualAdjustments2
{
    public enum VisualWindowType
    {
        Doll,
        Equipment,
        EEPicker,
        FXViewer,
        None
    }
    public class ServiceWindowsMenuVMModified : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public ServiceWindowsMenuVMModified(Action<VisualWindowType> onSelect)
        {
            this.m_OnSelect = onSelect;
            this.UnitDescriptor = Game.Instance.SelectionCharacter.SelectedUnit;
            this.CreateEntities();
            this.m_SelectedEntity = new ReactiveProperty<VisualWindowsMenuEntityVM>(this.m_EntitiesList.First());
            base.AddDisposable(this.SelectionGroup = new SelectionGroupRadioVM<VisualWindowsMenuEntityVM>(this.m_EntitiesList, this.m_SelectedEntity));
            base.AddDisposable(this.m_SelectedEntity.Skip(1).Subscribe(new Action<VisualWindowsMenuEntityVM>(this.OnEntitySelected)));
            base.AddDisposable(this.UnitDescriptor.Subscribe(new Action<UnitDescriptor>(this.OnUnitChanged)));
        }
        public void SelectWindow(VisualWindowType type)
        {
            try
            {
                this.m_SelectedEntity.SetValueAndForceNotify(this.m_EntitiesList.FirstOrDefault((VisualWindowsMenuEntityVM e) => e.VisualWindowType == type));
            }
            catch(Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        private void CreateEntities()
        {
            this.m_EntitiesList = new List<VisualWindowsMenuEntityVM>();
            foreach (object obj in Enum.GetValues(typeof(VisualWindowType)))
            {
                VisualWindowType type = (VisualWindowType)obj;
                if ((!RootUIContext.Instance.IsGlobalMap) && type != VisualWindowType.None)
                {
                    VisualWindowsMenuEntityVM windowsMenuEntityVm = new VisualWindowsMenuEntityVM(type);
                    base.AddDisposable(windowsMenuEntityVm);
                    this.m_EntitiesList.Add(windowsMenuEntityVm);
                }
            }
        }
        private void OnEntitySelected(VisualWindowsMenuEntityVM entity)
        {
            try
            {
                Action<VisualWindowType> onSelect = this.m_OnSelect;
                if (onSelect == null)
                {
                    return;
                }
                onSelect((entity != null) ? entity.VisualWindowType : throw (new Exception("")));
            }
            catch(Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        private void OnUnitChanged(UnitDescriptor unitDescriptor)
        {
            VisualWindowsMenuEntityVM serviceWindowsMenuEntityVM = this.m_EntitiesList.FirstOrDefault((VisualWindowsMenuEntityVM e) => e.VisualWindowType == VisualWindowType.None);
            if (serviceWindowsMenuEntityVM == null)
            {
                return;
            }
            serviceWindowsMenuEntityVM.SetAvailable(UIUtilityUnit.HasMythic(unitDescriptor));
        }
        public void Close()
        {
            Action<VisualWindowType> onSelect = this.m_OnSelect;
            if (onSelect == null)
            {
                return;
            }
            onSelect(VisualWindowType.None);
        }
        public override void DisposeImplementation()
        {
        }
        public SelectionGroupRadioVM<VisualWindowsMenuEntityVM> SelectionGroup;
        public List<VisualWindowsMenuEntityVM> m_EntitiesList;
        public ReactiveProperty<VisualWindowsMenuEntityVM> m_SelectedEntity;
        public readonly Action<VisualWindowType> m_OnSelect;
        public IReactiveProperty<UnitDescriptor> UnitDescriptor;
    }
}
