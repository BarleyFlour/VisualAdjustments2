using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
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
        public static void SetColors(UnitEntityData data, Character characterAvatar)
        {
            Gender gender = data.Gender;
            var race = UnitEntityView.GetActualRace(data);
            var settings = data.GetSettings();
            var EqClass = data.Progression.GetEquipmentClass();
#if DEBUG
            Main.Logger.Log(
                $"Tried to update {data.CharacterName}'s Class equipment, ${EqClass.NameSafe()}");
#endif
            if (EqClass == null) return;
            var links = EqClass?.GetClothesLinks(gender, race).Select(b => b.Load());
            if (!data.IsStoryCompanion() || (data.IsStoryCompanion() &&
                                             data.Descriptor.ForceUseClassEquipment))
            {
                foreach (var clothesLink in links)
                {
                    EquipmentEntity ee = clothesLink; //.Load(false, false);
                    if (!characterAvatar.EquipmentEntities.Contains(ee) &&
                        !settings.EeSettings.EEs.Any(a =>
                            a.GUID == ResourceLoader.NameToEEInfo[clothesLink.name].GUID))
                    {
                        characterAvatar.AddEquipmentEntity(ee);
                    }

                    if (clothesLink.PrimaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                    {
                        if (settings.ClassOverride.PrimaryIndex is not null or >= 0)
                        {
                            characterAvatar.SetPrimaryRampIndex(ee,
                                (int)settings.ClassOverride.PrimaryIndex, false);
                        }
                        else if (settings.ClassOverride.PrimaryCustomCol != null)
                        {
                            var col = settings.ClassOverride.PrimaryCustomCol.Value.ToColor();
                            if (!EeInfraStructure.ColorToTex.TryGetValue(col, out var tex))
                            {
                                tex = EE_Applier.ColorInfo.CreateTexture(col);
#if DEBUG
                                Main.Logger.Log($"Couldn't find texture for {col.ToString()}, Creating.");
#endif
                                EeInfraStructure.ColorToTex[col] = tex;
                            }

                            var ramps = ee.PrimaryColorsProfile.Ramps;
                            if (!ramps.Contains(tex))
                                ramps.Add(tex);

                            var index = ramps.IndexOf(tex);
                            characterAvatar.SetPrimaryRampIndex(ee, index);
                        }
                    }

                    if (clothesLink.SecondaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                    {
                        if (settings.ClassOverride.SecondaryIndex is not null or >= 0)
                        {
                            characterAvatar.SetSecondaryRampIndex(ee,
                                (int)settings.ClassOverride.SecondaryIndex, false);
                        }
                        else if (settings.ClassOverride.SecondaryCustomCol != null)
                        {
                            var col = settings.ClassOverride.SecondaryCustomCol.Value.ToColor();
                            if (!EeInfraStructure.ColorToTex.TryGetValue(col, out var tex))
                            {
                                tex = EE_Applier.ColorInfo.CreateTexture(col);
#if DEBUG
                                Main.Logger.Log($"Couldn't find texture for {col.ToString()}, Creating.");
#endif
                                EeInfraStructure.ColorToTex[col] = tex;
                            }

                            var ramps = ee.SecondaryColorsProfile.Ramps;
                            if (!ramps.Contains(tex))
                                ramps.Add(tex);

                            var index = ramps.IndexOf(tex);
                            characterAvatar.SetSecondaryRampIndex(ee, index);
                        }
                    }
                }
            }
        }

        public static void Postfix(UnitEntityView __instance)
        {
            try
            {
                if (!__instance.Data.IsPlayerFaction || __instance.Data.IsPet) return;
                SetColors(__instance.Data, __instance.CharacterAvatar);
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
}