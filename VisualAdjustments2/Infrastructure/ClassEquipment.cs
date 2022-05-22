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
    [HarmonyPatch(typeof(UnitProgressionData), nameof(UnitProgressionData.GetEquipmentClass))]
    public static class ClassEquipment
    {
        
        public static void Postfix(UnitProgressionData __instance,ref BlueprintCharacterClass __result)
        {
            Main.Logger.Log("dfgsdfgsgdfs");
            if (!__instance.Owner.IsPlayerFaction) return;
            var settings = __instance.Owner.Unit.GetSettings();
            Main.Logger.Log("cvdfbdf");
            if (settings.ClassGUID == "") return;
            __result = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(settings.ClassGUID);
        }
    }
}
