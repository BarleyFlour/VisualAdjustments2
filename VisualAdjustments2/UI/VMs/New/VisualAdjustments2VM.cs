using Kingmaker.UI.MVVM._PCView.ServiceWindows.Menu;
using Kingmaker.UI.MVVM._VM.ServiceWindows;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Menu;
using Kingmaker.Utility;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.SelectionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.UI
{
    public class VisualAdjustments2VM : ViewBase<SelectionGroupRadioVM<ServiceWindowsMenuEntityVM>>
    {
        public void Initialize()
        {
            if (this.m_MenuEntities.Empty<ServiceWindowsMenuEntityPCView>())
            {
                this.m_MenuEntities = base.GetComponentsInChildren<ServiceWindowsMenuEntityPCView>().ToList<ServiceWindowsMenuEntityPCView>();
            }
        }
        public override void BindViewImplementation()
        {
            this.m_MenuEntities.ForEach(delegate (ServiceWindowsMenuEntityPCView e)
            {
                e.gameObject.SetActive(false);
            });
            int index = 0;
            foreach (ServiceWindowsMenuEntityVM entities in base.ViewModel.EntitiesCollection)
            {
                this.m_MenuEntities[index].Bind(entities);
                index++;
            }
        }

        public override void DestroyViewImplementation()
        {
        }
        private List<ServiceWindowsMenuEntityPCView> m_MenuEntities;
    }
}
