using Kingmaker.View.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2
{
    static public class HandEquipmentOverrides
    {
        
        public struct HandEquipmentOverrideInfo
        {
            /* public HandEquipmentOverrideInfo(int slot, bool mainhand,WeaponAnimationStyle animationStyle)
             {
                 Slot = slot;
                 Mainhand = mainhand;
                 AnimationStyle = animationStyle;
             }*/
            /// <summary>
            /// 0-3
            /// </summary>
            [Newtonsoft.Json.JsonProperty] public int Slot;
            /// <summary>
            /// true = mainhand, false = offhand
            /// </summary>
            [Newtonsoft.Json.JsonProperty] public bool Mainhand;
            [Newtonsoft.Json.JsonProperty] public WeaponAnimationStyle AnimationStyle;
            public override string ToString()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }
        }
    }
}
