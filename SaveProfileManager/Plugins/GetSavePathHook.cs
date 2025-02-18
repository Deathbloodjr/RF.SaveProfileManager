using HarmonyLib;
using Platform.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveProfileManager.Plugins
{
    internal class GetSavePathHook
    {
        public static string ProfileName = "Default";
        public static string PreviousName = "Default";

        [HarmonyPatch(typeof(SteamSave))]
        [HarmonyPatch(nameof(SteamSave.GetSavePath))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SteamSave_GetSavePath_Postfix(SteamSave __instance, ref string __result)
        {
            //Plugin.Log.LogInfo(__result);

            if (ProfileName != "Default")
            {
                var split = __result.Split("\\");
                //split[split.Length - 2] = "DEBUG";
                split[split.Length - 2] = ProfileName;
                __result = Path.Combine(split);
            }

            PreviousName = ProfileName;
            Plugin.Log.LogInfo(__result);
        }

        public static bool ProfileChanged()
        {
            return PreviousName != ProfileName;
        }
    }
}
