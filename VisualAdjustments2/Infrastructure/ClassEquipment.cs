using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using Kingmaker.View;
using Kingmaker.Visual.CharacterSystem;
using UnityEngine;

namespace VisualAdjustments2.Infrastructure
{
    [HarmonyPatch(typeof(UnitProgressionData), nameof(UnitProgressionData.GetEquipmentClass))]
    public static class ClassEquipment
    {

        public static void Postfix(UnitProgressionData __instance, ref BlueprintCharacterClass __result)
        {
            if (!__instance.Owner.IsPlayerFaction) return;
            var settings = __instance.Owner.Unit.GetSettings();
            if (settings.ClassOverride.GUID.IsNullOrEmpty()) return;
            __result = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(settings.ClassOverride.GUID);
        }
    }
    [HarmonyPatch(typeof(UnitEntityView), nameof(UnitEntityView.UpdateClassEquipment))]
    public static class ClassEquipmentColors
    {
        public static void Postfix(UnitEntityView __instance)
        {
            try
            {
                if (!__instance.Data.IsPlayerFaction || __instance.Data.IsPet) return;
                Gender gender = __instance.EntityData.Gender;
                var race = UnitEntityView.GetActualRace(__instance.EntityData);
                var settings = __instance.Data.GetSettings();
                var EqClass = __instance.Data.Progression.GetEquipmentClass();
#if DEBUG
                Main.Logger.Log($"Tried to update {__instance.Data.CharacterName}'s Class equipment, ${EqClass.NameSafe()}");
#endif
                if (EqClass == null) return;
                var links = EqClass?.GetClothesLinks(gender, race).Select(b => b.Load());
                // var links = GetLinks();
                /*List<EquipmentEntity> GetLinks()
                {
                    var list = new List<EquipmentEntity>();
                    foreach(var eel in EqClass.EquipmentEntities)
                    {
                        var ee = eel.Load(gender,race);
                        if (ee != null && (ee.Count() > 0)) list.AddRange(ee);
                    }
                    return list;
                }*/
                if (!__instance.Data.IsStoryCompanion() || (__instance.Data.IsStoryCompanion() && __instance.Data.Descriptor.ForceUseClassEquipment))
                {
                    foreach (var clothesLink in links)
                    {
                        EquipmentEntity ee = clothesLink;//.Load(false, false);
                        if (!__instance.CharacterAvatar.EquipmentEntities.Contains(ee) &&
                            !settings.EeSettings.EEs.Any(a =>
                                a.GUID == ResourceLoader.NameToEEInfo[clothesLink.name].GUID))
                        {
                            __instance.CharacterAvatar.AddEquipmentEntity(ee);
                        }
                        else continue;
                        if (clothesLink.PrimaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                        {
                            if (settings.ClassOverride.PrimaryIndex is not null or >= 0)
                            {
                                __instance.CharacterAvatar.SetPrimaryRampIndex(ee, (int)settings.ClassOverride.PrimaryIndex, false);
                            }
                            else if (settings.ClassOverride.PrimaryCustomCol != null)
                            {
                                var col = settings.ClassOverride.PrimaryCustomCol.Value.ToColor();
                                var firstpixel = ee.PrimaryColorsProfile?.Ramps?.Where(t => t.isReadable).FirstOrDefault(t => t.GetPixel(1, 1) == col);
                                if (firstpixel == null)
                                {
                                    var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                                    {
                                        filterMode = FilterMode.Bilinear
                                    };
                                    tex.SetPixel(1, 1, col);
                                    tex.Apply();
                                    ee.PrimaryColorsProfile.Ramps.Add(tex);
                                    var index = ee.PrimaryColorsProfile.Ramps.IndexOf(tex);
                                    __instance.CharacterAvatar.SetPrimaryRampIndex(ee, index);
                                }
                                else
                                {
                                    var index = ee.PrimaryColorsProfile.Ramps.IndexOf(firstpixel);
                                    __instance.CharacterAvatar.SetPrimaryRampIndex(ee, index);
                                }
                            }
                        }
                        if (clothesLink.SecondaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                        {
                            if (settings.ClassOverride.SecondaryIndex is not null or >= 0)
                            {
                                __instance.CharacterAvatar.SetSecondaryRampIndex(ee, (int)settings.ClassOverride.SecondaryIndex, false);
                            }
                            else if (settings.ClassOverride.SecondaryCustomCol != null)
                            {
                                var col = settings.ClassOverride.SecondaryCustomCol.Value.ToColor();
                                var firstpixel = ee.SecondaryColorsProfile.Ramps.Where(t => t.isReadable).FirstOrDefault(t => t.GetPixel(1, 1) == col);
                                if (firstpixel == null)
                                {
                                    var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                                    {
                                        filterMode = FilterMode.Bilinear
                                    };
                                    tex.SetPixel(1, 1, col);
                                    tex.Apply();
                                    ee.SecondaryColorsProfile.Ramps.Add(tex);
                                    var index = ee.SecondaryColorsProfile.Ramps.IndexOf(tex);
                                    __instance.CharacterAvatar.SetSecondaryRampIndex(ee, index);
                                }
                                else
                                {
                                    var index = ee.SecondaryColorsProfile.Ramps.IndexOf(firstpixel);
                                    __instance.CharacterAvatar.SetSecondaryRampIndex(ee, index);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
}
