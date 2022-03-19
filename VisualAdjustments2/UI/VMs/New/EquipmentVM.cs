using Kingmaker.EntitySystem.Entities;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.UI
{
    public class EquipmentVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public EquipmentVM()
        {
            base.AddDisposable(this);
        }
        public override void DisposeImplementation()
        {
        }
        public void OnCharacterChanged()
        {

        }
        public EquipmentListVM m_EquipmentListVM;
    }
}
