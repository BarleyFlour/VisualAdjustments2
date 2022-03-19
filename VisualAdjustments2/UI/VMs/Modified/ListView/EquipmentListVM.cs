using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class EquipmentListVM : ListViewVM
    {
        public EquipmentListVM(ReactiveCollection<ListViewItemVM> entitiesCollection, ReactiveProperty<ListViewItemVM> entity) : base(entitiesCollection, entity)
        {

        }

    }
}
