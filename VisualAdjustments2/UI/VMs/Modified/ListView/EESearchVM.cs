using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.FeatureSelector;

namespace VisualAdjustments2.UI
{
    public class EESearchVM : ListSearchVM
    {
        // TODO: Populate this
        public static readonly string[] SearchOptions =
        {
            "",
            "",
            ""
        };
        public EESearchVM(IEnumerable<string> searchOptions) : base(searchOptions)
        {
            //this.LocalizedValues = SearchOptions.ToList<string>();
        }

        // Token: 0x06004EDD RID: 20189 RVA: 0x00003AE3 File Offset: 0x00001CE3
        public override void DisposeImplementation()
        {

        }

        // Token: 0x04003271 RID: 12913
        new public List<string> LocalizedValues;
    }
}
