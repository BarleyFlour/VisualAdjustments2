using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Owlcat.Runtime.Core.Logging;

namespace VisualAdjustments2.Infrastructure
{
    public static class Utilities
    {
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
    }
}
