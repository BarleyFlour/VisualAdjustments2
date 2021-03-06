using HarmonyLib;
using Kingmaker.Items;
using Kingmaker.View;
using Kingmaker.View.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints;
using UnityEngine;
using Kingmaker.Items.Slots;
using Kingmaker.Visual.Particles;

namespace VisualAdjustments2.Infrastructure
{
    public static class EquipmentInfrastructure
    {
        [HarmonyLib.HarmonyPatch(typeof(UnitEntityView),nameof(UnitEntityView.AddItemEquipment))]
        public static bool Prefix(UnitEntityView __instance,ItemEntity item)
        {
            if (!__instance.Data.IsPlayerFaction && Kingmaker.Game.Instance.Player.AllCharacters.Contains(__instance.Data)) return true;
            var settings = __instance.Data.GetSettings();
            if (settings.HideEquipmentDict.TryGetValue(item.Blueprint.ItemType,out bool val))
            {
                return !val;
            }
            else return true;
        }
    }
}
