using HarmonyLib;
using Kingmaker.UnitLogic.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.Infrastructure
{
    [HarmonyPatch(typeof(Buff), "TrySpawnParticleEffect")]
    public static class TrySpawnParticleEffect_Patch
    {
        public static bool Prefix(Buff __instance)
        {
            if (!__instance.Owner.IsPlayerFaction) return true;
            var settings = __instance.Owner.Unit.GetSettings();
            var sourceAbility = __instance?.Context?.SourceAbility?.AssetGuidThreadSafe;
            if (sourceAbility != null && ResourceLoader.AbilityGuidToFXGuids.TryGetValue(__instance.Context.SourceAbility.AssetGuidThreadSafe, out var fxs))
            {
                if(fxs.Any(x => settings.Fx_Settings.Xlist.Any(y => y == x)))
               // if (fxs.Any(settings.Fx_Settings.Xlist.Any(a => fxs.Contains(a)));
                //if (settings.Fx_Settings.Xlist.Contains(fxs))
                {
                    Main.Logger.Log(__instance.NameForAcronym + " Blocked");
                    if (settings.Fx_Settings.WhiteOrBlackList) return true;
                    else return false;
                }
                else
                {
                    Main.Logger.Log(__instance.NameForAcronym + " NotBlocked");
                    if (settings.Fx_Settings.WhiteOrBlackList) return false;
                    else return true;
                }
            }
            else return true;
        }
    }
}
