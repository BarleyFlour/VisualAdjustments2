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

namespace VisualAdjustments2.UI
{
    public class ListViewItemPCView : NestedSelectionGroupEntityPCView<ListViewItemVM>, IEventSystemHandler
    {
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            string str = base.ViewModel.Name;
            if (str.Length > this.m_MaxSymbols)
            {
                str = base.ViewModel.Name.Substring(0, this.m_MaxSymbols - 2) + "...";
            }
            this.m_FeatureNameText.text = str;
            this.m_FeatureAcronymText.text = base.ViewModel.Name;
            this.m_FeatureAcronymText.gameObject.SetActive(!string.IsNullOrEmpty(base.ViewModel.Name));
        }
        public override void OnChangeSelectedState(bool value)
        {
            base.OnChangeSelectedState(value);
            this.m_FeatureImage.material = (base.ViewModel.IsAvailable.Value ? null : this.m_Material);
        }

        // Token: 0x060061FC RID: 25084 RVA: 0x001F975C File Offset: 0x001F795C
        public override void OnClick()
        {
            if (base.ViewModel.IsAvailable.Value && (!base.ViewModel.IsSelected.Value || base.ViewModel.AllowSwitchOff))
            {
                this.ApplyOnClick();
                return;
            }
        }
        private void ApplyOnClick()
        {
            base.ViewModel.SetSelectedFromView(!base.ViewModel.IsSelected.Value);
            if (!base.ViewModel.AllowSwitchOff)
            {
                this.OnChangeSelectedState(base.ViewModel.IsSelected.Value);
            }
        }
        public Image m_FeatureImage;
        public Material m_Material;
        public TextMeshProUGUI m_FeatureAcronymText;
        public TextMeshProUGUI m_FeatureNameText;

        private int m_MaxSymbols = 33;
    }
}
