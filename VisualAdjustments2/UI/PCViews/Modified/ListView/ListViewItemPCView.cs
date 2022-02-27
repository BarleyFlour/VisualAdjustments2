using Kingmaker.UI.MVVM._PCView.Other.NestedSelectionGroup;
using Owlcat.Runtime.UI.Controls.Other;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.VirtualListSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Owlcat.Runtime.UI.SelectionGroup;
using Owlcat.Runtime.UI.SelectionGroup.View;

namespace VisualAdjustments2.UI
{
    public class ListViewItemPCView : SelectionGroupEntityView<ListViewItemVM>, IEventSystemHandler
    {
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            this.m_DisplayName.text = base.ViewModel.DisplayName;
        }
        public override void DestroyViewImplementation()
        {
        }
        public override void OnClick()
        {
            if (base.ViewModel.IsAvailable.Value && (!base.ViewModel.IsSelected.Value || base.ViewModel.AllowSwitchOff))
            {
                base.ViewModel.SetSelectedFromView(!base.ViewModel.IsSelected.Value);
            }
        }
        [SerializeField]
        public TextMeshProUGUI m_DisplayName;
    }
}
