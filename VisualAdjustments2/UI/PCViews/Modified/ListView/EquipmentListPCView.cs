using Kingmaker.UI;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class EquipmentListPCView : ListPCView
    {
        public override void SetupFromChargenList(CharGenFeatureSelectorPCView oldcomp, bool LeftOrRight, string LabelText)
        {
            var newpcview = oldcomp.gameObject.AddComponent<ListSearchPCView>();
            newpcview.SetupFromChargenFeatureSearchPCView(oldcomp.m_CharGenFeatureSearchView);
            this.m_CharGenFeatureSearchView = newpcview;
            this.m_SearchRequestEntitiesNotFound = oldcomp.m_SearchRequestEntitiesNotFound;
            if (m_Template == null)
            {
                var instantiated = UnityEngine.GameObject.Instantiate(oldcomp.SlotPrefabs.First());
                instantiated.ConvertToEquipmentPCView();
              //  var LE = instantiated.gameObject.AddComponent<LayoutElement>();
               // LE.minWidth = 530;
                /*if (LeftOrRight)
                {
                    instantiated.transform.Find("TextContainer").SetAsLastSibling();
                    instantiated.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>().padding.left = 20;
                }
                else
                {
                    instantiated.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>().padding.right = 12;
                }*/
                m_Template = instantiated.GetComponent<ListViewItemPCView>();
            }
            this.m_SelectorHeader = this.transform.Find("HeaderH2/Label").GetComponent<TextMeshProUGUI>();
            this.m_SelectorHeader.text = LabelText;
            this.SlotPrefab = m_Template;
            this.VirtualList = oldcomp.VirtualList;
           // this.transform.Find("StandardScrollView/Viewport/Content").GetComponent<VerticalLayoutGroup>().enabled = true;
        }
    }
}
