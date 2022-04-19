using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kingmaker.DLC;
using Kingmaker.Visual.CharacterSystem;
using Owlcat.Runtime.Core.Logging;
using UniRx;

namespace VisualAdjustments2.Infrastructure
{
    public static class Utilities
    {
        /* public class EEPoller : UnityEngine.MonoBehaviour, IDisposable
         {
             public Character character;
             Action<List<EquipmentEntity>> UpdateCurrentEEs;
             public EEPoller(Kingmaker.Visual.CharacterSystem.Character characterIn,Action<List<EquipmentEntity>> action)
             {
                 character = characterIn;
                 UpdateCurrentEEs = action;
             }
             int prevlength;
             public void Update()
             {
                 var count = character.EquipmentEntities.Count;
                 prevlength = count;
                 if(count != prevlength)
                 {
                     UpdateCurrentEEs(character.EquipmentEntities);
                 }
             }
             public void Dispose()
             {
                 UnityEngine.Component.Destroy(this);
             }
         }*/
#if DEBUG
        
        [HarmonyPatch(typeof(LogChannel))]
        public static class LOG_BE_QUIET
        {

            public static HashSet<string> Quiet = new HashSet<string>()
            {
                "Await input module",
                "Await event system",
            };

            [HarmonyPatch("Log"), HarmonyPatch(new Type[] { typeof(string), typeof(object[]) }), HarmonyPrefix]
            public static bool Log(string messageFormat)
            {
                if (Quiet.Contains(messageFormat))
                    return false;
                return true;
            }

        }
#endif
#if DEBUG

        [HarmonyPatch(typeof(DlcStore),nameof(DlcStore.IsPurchased))]
        public static class thingie
        {
            //[HarmonyPatch(typeof(DlcStoreSteam),nameof(DlcStoreSteam.IsPurchased)), HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                __result = true;
                return false;
            }

        }
#endif
    }
}
