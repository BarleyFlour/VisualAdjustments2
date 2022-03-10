using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Owlcat.Runtime.UI.MVVM;
using UnityEngine.UI;

namespace VisualAdjustments2.UI
{
    public class ColorPreviewView : ViewBase<EEPickerVM>
    {
        public override void BindViewImplementation()
        {
        }

        public override void DestroyViewImplementation()
        {
        }
        public Image m_ToColor;
    }
}
