using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Items;
using Kingmaker.View;
using Kingmaker.View.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Visual.CharacterSystem;

namespace VisualAdjustments2.Infrastructure
{
    [HarmonyPatch(typeof(UnitEntityView), nameof(UnitEntityView.AddItemEquipment), typeof(ItemEntity), typeof(UnitEntityData), typeof(Character))]
    public static class UnitEntityView_AddItemEquipment_Patch
    {
        public static bool Prefix(UnitEntityView __instance, ItemEntity item)
        {
            try
            {
                if (item == null) return true;
                if (__instance?.Data?.IsPlayerFaction != true) return true;
                var characterSettings = __instance.Data.GetSettings();
                if (characterSettings == null) return true;
                if (item.Blueprint != null && characterSettings.HideEquipmentDict[item.Blueprint.ItemType] == true) return false;
                else return true;   
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
                return true;
            }
        }
    }
}
