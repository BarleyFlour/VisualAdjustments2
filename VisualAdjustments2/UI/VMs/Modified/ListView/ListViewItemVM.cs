using JetBrains.Annotations;
using Kingmaker.UI.MVVM._VM.Other.NestedSelectionGroup;
using Kingmaker.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class ListViewItemVM : NestedSelectionGroupEntityVM, INestedListSource
    {
        public override INestedListSource NextSource
        {
            get
            {
                return this;
            }
        }
        public ListViewItemVM([NotNull] INestedListSource source, bool allowSwitchOff,(string,string) nameandguid) : base(source, allowSwitchOff)
        {
            Name = nameandguid.Item1;
            GUID = nameandguid.Item2;

        }

        public string Name { get; set; }

        public ReactiveProperty<ListViewItemVM> SelectedItemVM = new ReactiveProperty<ListViewItemVM>();
        public string GUID { get; set; }

        public override bool HasNesting => false;

        public override int NestingLimit => 0;

        public override void DoSelectMe()
        {
        }
        public bool HasText(string searchRequest)
        {
            return (this != null && this.Name.IndexOf(searchRequest, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.GUID.IndexOf(searchRequest, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        public List<NestedSelectionGroupEntityVM> ExtractNestedEntities()
        {
            return new List<NestedSelectionGroupEntityVM>();
        }

        public ReactiveProperty<NestedSelectionGroupEntityVM> GetSelectedReactiveProperty()
        {
            ReactiveProperty<NestedSelectionGroupEntityVM> selectorProperty = new ReactiveProperty<NestedSelectionGroupEntityVM>();
            base.AddDisposable(this.SelectedItemVM.Subscribe(delegate (ListViewItemVM val)
            {
                selectorProperty.Value = val;
            }));
            base.AddDisposable(selectorProperty.Skip(1).Subscribe(delegate (NestedSelectionGroupEntityVM val)
            {
                this.SelectedItemVM.Value = (val as ListViewItemVM);
            }));
            return selectorProperty;
        }
    }
}
