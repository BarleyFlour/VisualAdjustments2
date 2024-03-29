﻿using Kingmaker.UI.MVVM._PCView.ServiceWindows.Menu;
using Owlcat.Runtime.UI.SelectionGroup.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace VisualAdjustments2.UI
{
    public class VisualWindowsMenuEntityPCView : SelectionGroupEntityView<VisualWindowsMenuEntityVM>
    {
        public static string GetVisualWindowLabel(VisualWindowType windowtype)
        {
            switch (windowtype)
            {
                case VisualWindowType.Doll:
                    return "Doll";
                case VisualWindowType.EEPicker:
                    return "EE Picker";
                case VisualWindowType.Equipment:
                    return "Equipment";
                case VisualWindowType.FXViewer:
                    return "Buff FX's";
                case VisualWindowType.Portrait:
                    return "Portrait";
                default:
                    return "";
            }
        }
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            base.gameObject.SetActive(true);
            this.m_Label.text = GetVisualWindowLabel(base.ViewModel.VisualWindowType);
        }
        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            base.gameObject.SetActive(false);
            this.m_Label.text = string.Empty;
        }
        public TextMeshProUGUI m_Label;
    }
}
