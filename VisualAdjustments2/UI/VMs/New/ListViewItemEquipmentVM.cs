using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.UI
{
    public class ListViewItemEquipmentVM : ListViewItemVM
    {
        public ListViewItemEquipmentVM(ResourceInfo ee, bool addorremove, Action<ListViewItemVM> OnSelectAction) : base(ee, addorremove, OnSelectAction)
        {
        }

        public ListViewItemEquipmentVM(ListViewItemVM ToClone, bool addorremove, Action<ListViewItemVM> OnSelectAction) : base(ToClone, addorremove, OnSelectAction)
        {
        }
    }
}
