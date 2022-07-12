using HarmonyLib;
using Kingmaker.Enums;
using Kingmaker.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.Infrastructure
{
    public class CharacterScale
    {
        [HarmonyPatch(typeof(UnitEntityView), nameof(UnitEntityView.GetSizeScale))]
        private static class UnitEntityView_GetSizeScale_Patch
        {
            private static void Postfix(UnitEntityView __instance, ref float __result)
            {
                try
                {
                    if (__instance.EntityData == null) return;
                    if (!__instance.EntityData.IsPlayerFaction) return;
                    var characterSettings = __instance.EntityData.GetSettings();
                    if (characterSettings.BuffSettings.FixSize) return;
                    //Size originalSize = __instance.EntityData.Descriptor.OriginalSize;
                   // Size size = __instance.EntityData.Descriptor.State.Size;
                    __result = 1;
                    /*if (__instance.DisableSizeScaling) //Used when polymorphed
                    {
                        originalSize = size;
                    }
                    float sizeDiff = characterSettings.overrideScaleAdditive ?
                        ((int)size + characterSettings.additiveScaleFactor - (int)originalSize) :
                       (characterSettings.overrideScaleFactor - (int)originalSize);
                    float sizeScale = Mathf.Pow(1 / 0.66f, sizeDiff);
                    __result = sizeScale;*/
                }
                catch (Exception ex)
                {
                    Main.Logger.Error(ex.ToString());
                }
            }
        }
    }
}
