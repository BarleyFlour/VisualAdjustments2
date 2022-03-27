using Owlcat.Runtime.UI.Controls.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

namespace VisualAdjustments2.UI
{
    public class ToggleWithTextHandler : MonoBehaviour
    {
        public const string txtIfTrue = "Whitelist";
        public const string txtIfFalse = "Blacklist";
        public void Setup(OwlcatButton button, TextMeshProUGUI txt)
        {

            m_Button = button;
            m_Text = txt;
            PrimOrSec = new ReactiveProperty<bool>(true);
            PrimOrSec.Subscribe((a) => { m_Text.text = this.PrimOrSec.Value == true ? txtIfTrue : txtIfFalse; });
            //PrimOrSec.Subscribe((bool state) => { EnsureButtonsSelected(); });
            m_Button.OnLeftClick.AddListener(() => { this.PrimOrSec.Value = !this.PrimOrSec.Value; });
            // m_Secondary_Button.OnLeftClick.AddListener(() => { this.PrimOrSec.Value = false; });

        }
        public ReactiveProperty<bool> PrimOrSec;
        public OwlcatButton m_Button;
        public TextMeshProUGUI m_Text;
        public GameObject m_PrimarySelected;
    }
}
