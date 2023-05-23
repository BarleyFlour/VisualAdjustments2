using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.View.Equipment;
using Kingmaker.Visual.Particles;
using Owlcat.Runtime.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Visual.Particles.FxSpawnSystem;
using UnityEngine;

namespace VisualAdjustments2.Infrastructure
{
    //Fx stuff causes sheath fuckery
    // public class WeaponInfrastucture
    //  {
    //[HarmonyPatch(typeof(UnitViewHandSlotData), "VisibleItemBlueprint", MethodType.Getter)]
    [HarmonyPatch(typeof(UnitViewHandSlotData), nameof(UnitViewHandSlotData.ReattachSheath))]
    public static class UnitViewHandsSlotData_ReattachSheath_Patch
    {
        private static bool Prefix(UnitViewHandSlotData __instance, UnitViewHandsEquipment ___m_Equipment)
        {
            try
            {
                if (!__instance.Owner.IsPlayerFaction) return true;
                var characterSettings = __instance.Owner.GetSettings();
                if (characterSettings.HideEquipmentDict[ItemsFilter.ItemType.Weapon])
                {
                    __instance.DestroySheathModel();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UnitViewHandSlotData), nameof(UnitViewHandSlotData.VisibleItemVisualParameters),
        MethodType.Getter)]
    public static class UnitViewHandsSlotData_VisibleItemBlueprint_Patch
    {
        public static void Postfix(UnitViewHandSlotData __instance, ref WeaponVisualParameters __result)
        {
            try
            {
                if (!__instance.Owner.IsPlayerFaction) return;
                var characterSettings = __instance.Owner.GetSettings();
                if (characterSettings == null) return;
                if (__instance.VisibleItem == null) return;
                {
                    var bp = ((BlueprintItemEquipmentHand)__instance.VisibleItem.Blueprint)?.VisualParameters?.AnimStyle
                        .ToString();
                    var WeaponOverride = characterSettings.WeaponOverrides.FirstOrDefault(b =>
                        b.Slot == __instance.Owner.View.HandsEquipment?.Sets?.Keys?.ToList()
                            .IndexOf(__instance?.Slot?.HandsEquipmentSet) && b?.AnimStyle == bp &&
                        __instance?.m_IsMainHand == b?.MainOrOffHand);
                    if (WeaponOverride != null)
                    {
                        __result = ResourcesLibrary.TryGetBlueprint<BlueprintItemEquipmentHand>(WeaponOverride.GUID)
                            .VisualParameters;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(UnitViewHandSlotData), nameof(UnitViewHandSlotData.UpdateWeaponEnchantmentFx))]
    public static class UnitViewHandSlotData_UpdateWeaponEnchantmentFX_Patch
    {
        public static IFxHandle RespawnFx(GameObject prefab, ItemEntity item)
        {
            WeaponSlot weaponSlot = item?.HoldingSlot as WeaponSlot;
            var weaponSnap = weaponSlot?.FxSnapMap;
            var unit = item?.Wielder?.Unit?.View;
            if (prefab != null && weaponSnap != null)
                return FxHelper.SpawnFxOnWeapon(prefab, unit, weaponSnap);
            else return null;
        }

        public static void Postfix(UnitViewHandSlotData __instance, bool isVisible)
        {
            try
            {
                if (!Main.IsEnabled) return;
                if (!__instance.Owner.IsPlayerFaction) return;
                var characterSettings = __instance.Owner.GetSettings();
                if (characterSettings == null) return;
                if (__instance.Slot.MaybeItem == null) return;
                if (characterSettings.CurrentFXs.ContainsKey(__instance.Slot.MaybeItem))
                {
                    foreach (var fxObject in characterSettings.CurrentFXs[__instance.Slot.MaybeItem])
                    {
                        FxHelper.Destroy(fxObject);
                    }

                    characterSettings.CurrentFXs[__instance.Slot.MaybeItem].Clear();
                }

                if (__instance.IsInHand && isVisible)
                {
                    if (!characterSettings.CurrentFXs.ContainsKey(__instance.Slot.MaybeItem))
                        characterSettings.CurrentFXs[__instance.Slot.MaybeItem] = new List<IFxHandle>();
                    foreach (var enchantOverride in characterSettings.EnchantOverrides)
                    {
                        if ((__instance.Owner.View.HandsEquipment?.Sets?.Keys?.ToList()
                                .IndexOf(__instance?.Slot?.HandsEquipmentSet) == enchantOverride.Slot) &&
                            __instance.Slot.IsPrimaryHand == enchantOverride.MainOrOffHand)
                        {
                            foreach (var enchant in __instance.m_EnchantmentFxObjects.ToTempList())
                            {
                                FxHelper.Destroy(enchant, true);
                                // enchant?.SpawnedObject?.SetActive(false);
                                //  GameObject.DestroyImmediate(enchant?.SpawnedObject);
                                //  enchant?.HandleDestroy();
                                // GameObject.Destroy(enchant);
                                // __instance.m_EnchantmentFxObjects.Remove(enchant);
                            }

                            if (enchantOverride.GUID == "Hide")
                            {
#if DEBUG
                                Main.Logger.Log("Tried Remove Enchant");
#endif
                                continue;
                            }

                            var blueprint =
                                ResourcesLibrary.TryGetBlueprint<BlueprintWeaponEnchantment>(enchantOverride.GUID);
                            if (blueprint == null || blueprint.WeaponFxPrefab == null) continue;
                            var fxObject = RespawnFx(blueprint.WeaponFxPrefab.Load(), __instance.Slot.MaybeItem);
                            characterSettings.CurrentFXs[__instance.Slot.MaybeItem].Add(fxObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(DollRoom), nameof(DollRoom.UpdateAvatarRenderers))]
    public static class DollRoom_UpdateAvatarRenderers_Patchf
    {
        //static FastInvoker<DollRoom, GameObject, object> UnscaleFxTimes;
        /*static bool Prepare()
        {
           // UnscaleFxTimes = Accessors.CreateInvoker<DollRoom, GameObject, object>("UnscaleFxTimes");
            return true;
        }*/

        private static void Postfix(DollRoom __instance, UnitViewHandsEquipment ___m_AvatarHands,
            UnitEntityData ___m_Unit)
        {
            try
            {
                if (___m_Unit == null) return;
                if (!___m_Unit.IsPlayerFaction) return;
                var characterSettings = ___m_Unit.GetSettings();
                if (characterSettings == null) return;
                foreach (var isOffhand in new bool[] { true, false })
                {
                    WeaponParticlesSnapMap weaponParticlesSnapMap = ___m_AvatarHands?.GetWeaponModel(isOffhand)
                        ?.GetComponent<WeaponParticlesSnapMap>();
                    if (weaponParticlesSnapMap)
                    {
                        UnityEngine.Object x = weaponParticlesSnapMap;
                        UnityEngine.Object y = isOffhand
                            ? ___m_Unit?.Body?.SecondaryHand.FxSnapMap
                            : ___m_Unit?.Body?.PrimaryHand.FxSnapMap;
                        if (x == y)
                        {
                            var weapon = isOffhand
                                ? ___m_Unit?.Body?.SecondaryHand?.MaybeItem
                                : ___m_Unit?.Body?.PrimaryHand?.MaybeItem;
                            characterSettings.CurrentFXs.TryGetValue(weapon, out List<IFxHandle> fxObjects);
                            if (fxObjects != null)
                            {
                                foreach (var fxObject in fxObjects)
                                {
                                    DollRoom.UnscaleFxTimes(fxObject.SpawnedObject);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(UnitViewHandsEquipment), nameof(UnitViewHandsEquipment.UpdateBeltPrefabs))]
    static class UnitViewHandsEquipment_UpdateBeltPrefabs_Patch
    {
        private static void Postfix(UnitViewHandsEquipment __instance, GameObject[] ___m_ConsumableSlots)
        {
            try
            {
#if DEBUG
                Main.Logger.Log($"Updating {__instance.Owner.CharacterName}'s Belt items");
#endif
                if (!__instance.Owner.IsPlayerFaction) return;
#if DEBUG
                Main.Logger.Log($"{__instance.Owner.CharacterName} is of PlayerFaction");
#endif
                // Main.logger.Log("UpdateBeltPrefabs");
                var characterSettings = __instance.Owner.GetSettings();
                if (characterSettings.HideEquipmentDict[(ItemsFilter.ItemType)11])
                {
                    #if DEBUG
                    Main.Logger.Log($"tried to hide {__instance.Owner.CharacterName}'s belt items");
                    #endif
                    foreach (var go in ___m_ConsumableSlots) go?.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
    }
}