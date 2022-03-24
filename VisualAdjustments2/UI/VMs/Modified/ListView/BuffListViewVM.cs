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
    public class BuffListViewVM : SelectionGroupVM<BuffButtonVM>
    {
        public BuffListViewVM(ReactiveCollection<BuffButtonVM> entitiesCollection, ReactiveProperty<BuffButtonVM> entity) : base(entitiesCollection)
        {
            this.SelectedEntity = entity;
            base.AddDisposable(this.SelectedEntity.Subscribe(delegate (BuffButtonVM _)
            {
                this.SetupSelectedState();
            }));
        }
        public override void SetupSelectedState()
        {
            foreach (BuffButtonVM tviewModel in this.EntitiesCollection)
            {
                tviewModel.SetSelected(this.SelectedEntity.Value == tviewModel);
            }
        }
        public override bool TryDoSelect(BuffButtonVM viewModel)
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

        public override bool TryDoUnselectSelect(BuffButtonVM viewModel)
        {
            if (this.SelectedEntity != null && this.SelectedEntity.Value == viewModel)
            {
                this.SelectedEntity.Value = default(BuffButtonVM);
                return true;
            }
            return false;
        }
        public override void DisposeImplementation()
        {
            base.DisposeImplementation();
        }
        public ReactiveProperty<BuffButtonVM> SelectedEntity;
    }
}
