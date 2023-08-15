using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Kingmaker.Blueprints.Classes.Prerequisites.Prerequisite;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.ActivatableAbilities;

namespace VisualAdjustments2.Infrastructure
{
    public static class Bubble_Extension_Methods
    {
        public static bool GetBeneficialBuffs(this GameAction action)
        {
            if (action != null)
            {
                if (action is ContextActionApplyBuff applyBuff && applyBuff.Buff != null &&
                    ((applyBuff.Buff.FxOnStart != null && applyBuff.Buff.FxOnStart.AssetId != "") ||
                     (applyBuff.Buff.FxOnRemove != null && applyBuff.Buff.FxOnRemove.AssetId != "")))
                {
                    return true;
                }
                else if (action is ContextActionsOnPet enchantPet)
                {
                    if (enchantPet.Actions?.Actions?.Where(a => a != null).Where(a => a?.GetBeneficialBuffs() == true).Any() == true)
                        return true;
                }
                else if (action is ContextActionPartyMembers applyParty)
                {
                    if (applyParty.Action?.Actions?.Where(a => a != null).Any() == true)
                        return true;
                }
                else if (action is ContextActionSpawnAreaEffect spawnArea)
                {
                    //  LogVerbose(level, $"recursing into spawnArea");
                    if (spawnArea.AreaEffect?.TryGetComponent<AbilityAreaEffectBuff>(out var areaBuff) == true &&
                        areaBuff.Buff?.IsBeneficial() == true)
                    {
                        //  LogVerbose(level, $"FOUND: areaBuff {areaBuff.name}");
                        return true;
                    }
                }
                else if (action is Conditional maybe)
                {
                    bool takeYes = true;
                    bool takeNo = true;
                    var conditions = maybe.ConditionsChecker?.Conditions;
                    if (conditions != null)
                    {
                        foreach (var c in conditions)
                        {
                            if (c is ContextConditionIsAlly ally)
                            {
                                if (ally.Not)
                                    takeYes = false;
                                else
                                    takeNo = false;
                            }
                        }
                    }

                    if (takeNo)
                    {
                        var buffs = maybe.IfFalse?.Actions?.Where(a => a.GetBeneficialBuffs());
                        if (buffs != null)
                        {
                            foreach (var b in buffs)
                                if (b)
                                    return true;
                        }
                    }

                    if (takeYes)
                    {
                        var buffs = maybe.IfTrue?.Actions?.Where(a => a.GetBeneficialBuffs());
                        if (buffs != null)
                        {
                            foreach (var b in buffs)
                                if (b)
                                    return b;
                        }
                    }
                }
                else if (action is ContextActionCastSpell spellCast)
                {
                    if (spellCast?.Spell?.GetBeneficialBuffs() == true)
                        return true;
                }
            }

            return false;
        }

        public static bool GetBeneficialBuffs(this BlueprintAbility spell)
        {
            try
            {
                //LogVerbose(level, $"getting buffs for spell: {spell.Name}");
                // spell = spell.DeTouchify();
                // LogVerbose(level, $"detouchified-to: {spell.Name}");
                if (spell.TryGetComponent<AbilityEffectRunAction>(out var runAction))
                {
                    return runAction.Actions.Actions.Where(a => a.GetBeneficialBuffs()).Any();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw;
            }
        }

        public static bool GetBeneficialBuffs(this BlueprintActivatableAbility spell)
        {
            //LogVerbose(level, $"getting buffs for spell: {spell.Name}");
            // spell = spell.DeTouchify();
            // LogVerbose(level, $"detouchified-to: {spell.Name}");
            if (spell.TryGetComponent<AbilityEffectRunAction>(out var runAction))
            {
                return runAction.Actions.Actions.Where(a => a.GetBeneficialBuffs()).Any();
            }
            else
            {
                return false;
            }
        }

        public static bool TryGetComponent<T>(this BlueprintScriptableObject bp, out T component)
        {
            component = bp.GetComponent<T>();
            return component != null;
        }

        public static bool IsBeneficial(this BlueprintBuff buff)
        {
            var contextApply = buff.GetComponent<AddFactContextActions>();
            if (contextApply == null)
                return true;

            if (contextApply.Activated?.Actions?.Any(action => action is ContextActionSavingThrow) ?? false)
            {
                return true;
            }

            return true;
        }
    }
}