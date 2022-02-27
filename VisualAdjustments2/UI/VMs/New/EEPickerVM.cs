using Kingmaker.Blueprints;
using Kingmaker.BundlesLoading;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Visual.CharacterSystem;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace VisualAdjustments2.UI
{
    public class EEPickerVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public ReactiveProperty<ListViewVM> AllEEs = new ReactiveProperty<ListViewVM>();
        public ReactiveProperty<ListViewVM> CurrentEEs = new ReactiveProperty<ListViewVM>();
        public EEPickerVM(UnitEntityData data)
        {
            base.AddDisposable(this);
            // data.View.CharacterAvatar.EquipmentEntities.ObserveEveryValueChanged(a => a).Subscribe(a => { CurrentEEs.EntitiesCollection.Clear(); CurrentEEs.EntitiesCollection.Add(a); });
            //base.AddDisposable();
            ReactiveCollection<ListViewItemVM> reactive = new ReactiveCollection<ListViewItemVM>();
            foreach(var kv in ResourceLoader.AllEEs)
            {
                reactive.Add(new ListViewItemVM(kv));
            }
            base.AddDisposable(AllEEs.Value = new ListViewVM(reactive, new ReactiveProperty<ListViewItemVM>(reactive.First())));
            base.AddDisposable(CurrentEEs.Value = new ListViewVM(reactive, new ReactiveProperty<ListViewItemVM>(reactive.First())));
            //CurrentEEs = new ListViewVM();
        }
        public override void DisposeImplementation()
        {

        }
    }
}
