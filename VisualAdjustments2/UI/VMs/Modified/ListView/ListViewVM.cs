using Kingmaker.UI.MVVM._VM.Other.NestedSelectionGroup;
using Owlcat.Runtime.UI.SelectionGroup;
using Owlcat.Runtime.UI.SelectionGroup.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class ListViewVM : SelectionGroupVM<ListViewItemVM>
    {
        public ListViewVM(ReactiveCollection<ListViewItemVM> entitiesCollection, ReactiveProperty<ListViewItemVM> entity) : base(entitiesCollection)
        {
            this.SelectedEntity = entity;
            base.AddDisposable(this.SelectedEntity.Subscribe(delegate (ListViewItemVM _)
            {
                this.SetupSelectedState();
            }));
        }
        public override void SetupSelectedState()
        {
            foreach (ListViewItemVM tviewModel in this.EntitiesCollection)
            {
                tviewModel.SetSelected(this.SelectedEntity.Value == tviewModel);
            }
        }

        public override bool TryDoSelect(ListViewItemVM viewModel)
        {
            if (this.SelectedEntity == null)
            {
                return false;
            }
            if (this.SelectedEntity.Value != viewModel)
            {
                this.SelectedEntity.Value = viewModel;
                return true;
            }
            return false;
        }

        public override bool TryDoUnselectSelect(ListViewItemVM viewModel)
        {
            if (this.SelectedEntity != null && this.SelectedEntity.Value == viewModel)
            {
                this.SelectedEntity.Value = default(ListViewItemVM);
                return true;
            }
            return false;
        }
        public ReactiveProperty<ListViewItemVM> SelectedEntity;
    }
}
