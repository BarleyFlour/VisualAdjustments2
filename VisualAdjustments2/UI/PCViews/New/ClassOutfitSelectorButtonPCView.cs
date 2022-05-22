using Kingmaker.Blueprints.Classes;
using Owlcat.Runtime.UI.Controls.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace VisualAdjustments2.UI
{
    public class ClassOutfitSelectorButtonPCView : MonoBehaviour
    {
        public void SetSelected(bool selected)
        {
            int layer = selected ? 1 : 0;
            this.Button.SetActiveLayer(layer);
        }
        public void Setup(TextMeshProUGUI label, BlueprintCharacterClass Class, OwlcatMultiButton button, Action onClick)
        {
            this.Label = label;
            this.ClassName = Class.LocalizedName;
            this.GUID = Class.AssetGuidThreadSafe;
            this.Button = button;
            this.Button.OnLeftClick.AddListener(new UnityAction(onClick));
            this.Label.text = ClassName;
        }
        public TextMeshProUGUI Label;
        public string ClassName;
        public string GUID;
        public OwlcatMultiButton Button;
        public GameObject Selected;

    }
}
