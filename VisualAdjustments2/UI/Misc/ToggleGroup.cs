using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Owlcat.Runtime.UniRx;

namespace VisualAdjustments2.UI
{
    public class ToggleGroupHandler : MonoBehaviour
    {
        public void EnsureButtonsSelected()
        {
            m_PrimarySelected.SetActive((PrimOrSec.Value && ShowSelected.Value));
            m_SecondarySelected.SetActive((!PrimOrSec.Value && ShowSelected.Value));
        }
        public void Setup(OwlcatButton prim, OwlcatButton sec)
        {

            m_Primary_Button = prim;
            // m_PrimarySelected = prim.transform.Find("Selected").gameObject;

            m_Secondary_Button = sec;
            //m_PrimarySelected = sec.transform.Find("Selected").gameObject;

            // return;
            PrimOrSec = new ReactiveProperty<bool>(true);
            PrimOrSec.Subscribe((bool state) => { EnsureButtonsSelected(); });

            ShowSelected.Subscribe((bool state) => { EnsureButtonsSelected(); });

            m_Primary_Button.OnLeftClick.AddListener(() => { this.PrimOrSec.Value = true; });
            m_Secondary_Button.OnLeftClick.AddListener(() => { this.PrimOrSec.Value = false; });

        }
        public ReactiveProperty<bool> ShowSelected = new(true);
        public ReactiveProperty<bool> PrimOrSec;
        public OwlcatButton m_Primary_Button;
        public GameObject m_PrimarySelected;
        public OwlcatButton m_Secondary_Button;
        public GameObject m_SecondarySelected;
    }
}
