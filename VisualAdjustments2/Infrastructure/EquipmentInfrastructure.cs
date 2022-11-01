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

    [HarmonyPatch(typeof(UnitViewHandsEquipment), nameof(UnitViewHandsEquipment.UpdateBeltPrefabs))]
    public static class bro
    {
        private static void Postfix(UnitViewHandsEquipment __instance)
        {
            try
            {
                if (!__instance.Owner.IsPlayerFaction) return;
                var characterSettings = __instance.Owner.GetSettings();
#if  DEBUG
                Main.Logger.Log($"Updating {__instance.Owner.CharacterName}'s Belt items, Hide is set to {characterSettings.HideEquipmentDict[ItemsFilter.ItemType.Usable]}");
#endif
                if (characterSettings.HideEquipmentDict[ItemsFilter.ItemType.Usable])
                {
                    foreach (var go in __instance.m_ConsumableSlots)
                    {
#if  DEBUG
                        Main.Logger.Log($"Tried to hide belt item: {go.name}");
#endif
                        go.SetActive(false);
                    }
                }
                /*if (characterSettings.overrideScale && !__instance.Character.PeacefulMode)
                {
                    foreach (var go in ___m_ConsumableSlots)
                    {
                        if (go == null) continue;
                        go.transform.localScale *= ViewManager.GetRealSizeScale(__instance.Owner.View, characterSettings);
                    }
                }*/
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
    }
}