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
using Kingmaker.EntitySystem.Entities;
using UnityEngine;
using Kingmaker.Items.Slots;
using Kingmaker.UI.Common;
using Kingmaker.Visual.CharacterSystem;
using Kingmaker.Visual.Particles;

namespace VisualAdjustments2.Infrastructure
{
    [HarmonyLib.HarmonyPatch(typeof(UnitEntityView), nameof(UnitEntityView.AddItemEquipment), typeof(ItemEntity))]
    public static class EquipmentInfrastructure
    {

        public static bool Prefix(UnitEntityView __instance, ItemEntity item)
        {
            if (__instance?.Data == null) return true;
            if (!__instance.Data.IsPlayerFaction && !Kingmaker.Game.Instance.Player.AllCharacters.Contains(__instance.Data) ) return true;
            if (item?.Blueprint == null) return true;
            var settings = __instance.Data.GetSettings();
            if (settings.HideEquipmentDict.TryGetValue(item.Blueprint.ItemType, out bool val))
            {
                return !val;
            }
            else return true;
        }
    }
}
