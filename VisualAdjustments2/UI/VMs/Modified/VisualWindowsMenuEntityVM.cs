using Owlcat.Runtime.UI.SelectionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.UI
{
    public class VisualWindowsMenuEntityVM : SelectionGroupEntityVM
    {
		public VisualWindowsMenuEntityVM(VisualWindowType type) : base(false)
		{
			this.VisualWindowType = type;
		}
		public void SetAvailable(bool available)
		{
			base.SetAvailableState(available);
		}
		public override void DoSelectMe()
		{
		}

		public VisualWindowType VisualWindowType;
	}
}
