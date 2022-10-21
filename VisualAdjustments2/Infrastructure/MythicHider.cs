using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Kingmaker.Visual.CharacterSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.Infrastructure
{
    /*[HarmonyPatch(typeof(Kingmaker.UI.ServiceWindow.DollRoom), nameof(DollRoom.SetupInfo))]
    public static class SetupInfoDollRoom_Patch
    {
        private static void Postfix(Kingmaker.UI.ServiceWindow.DollRoom __instance, UnitEntityData player, bool force = false, BlueprintClassAdditionalVisualSettings additionalVisualSettings = null)
        {
            try
            {
                var settings = player.GetSettings();
                if (settings.HideEquipmentDict.TryGetValue((ItemsFilter.ItemType)100, out var value) && value == true)
                {
                    __instance.m_Avatar.SetAdditionalVisualSettings(null);
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(DollCharacterController), nameof(DollCharacterController.UpdateDollRoomLevel))]
    public static class DollCharacterController_Patch
    {
        public static void Postfix()
        {
            var dollroom = Game.Instance.UI.Common.DollRoom;
            var settings = dollroom.Unit.GetSettings();
            if (settings != null)
            {
                if (settings.HideEquipmentDict.TryGetValue((ItemsFilter.ItemType)100, out var value) && value == true)
                {
                    dollroom.GetAvatar().SetAdditionalVisualSettings(null);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.ApplyAdditionalVisualSettings))]
    public static class ApplyVisualSettings_Patch
    {
        public static void Prefix(Character __instance)
        {
            try
            {
                var unit = Game.Instance.Player.PartyAndPets.FirstOrDefault(a => a?.View?.CharacterAvatar == __instance);
                if (unit == null) return;
                var settings = unit.GetSettings();
                if (settings == null) return;
                if (settings.HideEquipmentDict.TryGetValue((ItemsFilter.ItemType)100, out var value) && value == true)
                {
                    __instance.m_AdditionalVisualSettings = null;
                    unit.View.CharacterAvatar.SetAdditionalVisualSettings(null);
                    Game.Instance.UI.Common.DollRoom.m_Avatar.SetAdditionalVisualSettings(null);

                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
    [HarmonyPatch(typeof(UnitProgressionData), nameof(UnitProgressionData.AdditionalVisualSettings), methodType: MethodType.Getter)]
    public static class UpdateVisualSettings_Patch
    {
        public static bool Prefix(UnitProgressionData __instance, ref BlueprintClassAdditionalVisualSettings __result)
        {
            try
            {
                var unit = __instance.Owner;
                if (!unit.IsPlayerFaction) return true;
                if (unit == null) return true;
                var settings = unit.Unit.GetSettings();
                if (settings == null) return true;
                if (settings.HideEquipmentDict.TryGetValue((ItemsFilter.ItemType)100, out var value) && value == true)
                {
                    __result = null;
                    return false;
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
            return true;
        }
    }*/
    [HarmonyPatch(typeof(UnitProgressionData), nameof(UnitProgressionData.GetVisualSettingsProvider))]
    public static class GetVisualSettingsProvider_Patch
    {
        public static bool Prefix(UnitProgressionData __instance, ref ClassData __result)
        {
            try
            {
                var unit = __instance.Owner;
                if (unit?.IsPlayerFaction != true) return true;
                var settings = unit.Unit.GetSettings();
                if (settings == null) return true;
                if (settings.HideEquipmentDict.TryGetValue((ItemsFilter.ItemType)100, out var value) && value == true)
                {
                    __result = null;
                    unit.Progression.m_AdditionalVisualSettingsDisabled = 1;
                    return false;
                }
                else
                {
                    unit.Progression.m_AdditionalVisualSettingsDisabled = 0;
                    return true;
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
            return true;
        }
    }
}
