using Kingmaker.Blueprints.Root;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Root.Strings;

namespace VisualAdjustments2.UI
{
    public class ListSearchVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
		public ListSearchVM(IEnumerable<string> DropDownOptions)
		{
			this.LocalizedValues = DropDownOptions.GetType() == typeof(List<string>) ? (List<string>)DropDownOptions : DropDownOptions.ToList();
		}
		public override void DisposeImplementation()
		{
		}
		public List<string> LocalizedValues;
	}
}
