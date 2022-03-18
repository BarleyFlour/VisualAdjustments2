using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic;
using Kingmaker.View;

namespace VisualAdjustments2.Infrastructure
{
    public static class ClassEquipment
    {
        [HarmonyPatch(typeof(UnitProgressionData),nameof(UnitProgressionData.GetEquipmentClass)),HarmonyPostfix]
        public static void ReturnSetClassOutfit(UnitProgressionData __instance,ref BlueprintCharacterClass __result)
        {
            if (!__instance.Owner.IsPlayerFaction) return;
            var settings = __instance.Owner.Unit.GetSettings();
            if (settings.ClassGUID == "") return;
            __result = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(settings.ClassGUID);
        }
    }
}
