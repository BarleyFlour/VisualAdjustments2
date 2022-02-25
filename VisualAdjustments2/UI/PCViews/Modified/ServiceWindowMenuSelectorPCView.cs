using Kingmaker.UI.MVVM._PCView.ServiceWindows.Menu;
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
    public class ServiceWindowMenuSelectorPCViewModified : ViewBase<SelectionGroupRadioVM<VisualWindowsMenuEntityVM>>
    {
        public void Initialize()
        {
            if (this.m_MenuEntities.Empty<VisualWindowsMenuEntityPCView>())
            {
                this.m_MenuEntities = base.GetComponentsInChildren<VisualWindowsMenuEntityPCView>().ToList<VisualWindowsMenuEntityPCView>();
            }
        }
        public override void BindViewImplementation()
        {
            try
            {
                this.m_MenuEntities.ForEach(delegate (VisualWindowsMenuEntityPCView e)
                {
                    e.gameObject.SetActive(false);
                });
                int index = 0;
                foreach (VisualWindowsMenuEntityVM entities in base.ViewModel.EntitiesCollection)
                {
                    this.m_MenuEntities[index].Bind(entities);
                    index++;
                }
            }
            catch(Exception e) { Main.Logger.Error(e.ToString()); }
        }
        public override void DestroyViewImplementation()
        {
        }
        public List<VisualWindowsMenuEntityPCView> m_MenuEntities;
    }
}
