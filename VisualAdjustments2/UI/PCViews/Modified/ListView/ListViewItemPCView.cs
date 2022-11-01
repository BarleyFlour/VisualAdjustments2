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
using Owlcat.Runtime.UI.Controls.Button;
using Kingmaker.Utility;

namespace VisualAdjustments2.UI
{
    public class ListViewItemPCView : SelectionGroupEntityView<ListViewItemVM>, IEventSystemHandler
    {
        public override void BindViewImplementation()
        {
            base.gameObject.SetActive(true);
            base.BindViewImplementation();
            m_AddButton?.OnLeftClick?.AddListener(OnClickAddRemove);
            this.m_DisplayName.text = base.ViewModel.DisplayName.IsNullOrEmpty() ? base.ViewModel.InternalName : base.ViewModel.DisplayName;
            if(this.m_IconText != null && !hasImage) this.m_IconText.text = base.ViewModel.AddOrRemove ? ListViewItemVM.Add : ListViewItemVM.Remove;
        }
        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            m_AddButton?.OnLeftClick?.RemoveListener(OnClickAddRemove);
            //  base.gameObject.SetActive(false);
        }
        public void OnClickAddRemove()
        {
            this?.ViewModel?.action(this?.ViewModel);
        }
        public override void OnClick()
        {
            if (base.ViewModel.IsAvailable.Value && (!base.ViewModel.IsSelected.Value || base.ViewModel.AllowSwitchOff))
            {
                base.ViewModel.SetSelectedFromView(!base.ViewModel.IsSelected.Value);
            }
        }
        public TextMeshProUGUI m_DisplayName;
        public TextMeshProUGUI m_IconText;
        public OwlcatMultiButton m_AddButton;
        public bool hasImage = false;
    }
}
