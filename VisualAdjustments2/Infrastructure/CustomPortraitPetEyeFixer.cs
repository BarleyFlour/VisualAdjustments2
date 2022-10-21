using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.View.Equipment;
using UnityEngine;

namespace VisualAdjustments2.Infrastructure
{
    /*[HarmonyPatch(typeof(PortraitData),methodType:MethodType.Getter, methodName:nameof(PortraitData.PetEyePortrait))]
    public class CustomPortraitPetEyeFixer
    {
        public static void Postfix(PortraitData __instance, ref Sprite __result)
        {
            if (__result == null)
            {
                __result = __instance.SmallPortrait;
            }
        }
    }*/
}