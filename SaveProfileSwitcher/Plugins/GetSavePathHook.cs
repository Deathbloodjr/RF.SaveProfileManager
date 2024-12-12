using HarmonyLib;
using Platform.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveProfileSwitcher.Plugins
{
    internal class GetSavePathHook
    {
        [HarmonyPatch(typeof(SteamSave))]
        [HarmonyPatch(nameof(SteamSave.GetSavePath))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void SteamSave_GetSavePath_Postfix(SteamSave __instance, ref string __result)
        {
            //Plugin.Log.LogInfo(__result);

            var split = __result.Split("\\");
            //split[split.Length - 2] = "DEBUG";
            split[split.Length - 2] = Plugin.Instance.ConfigSaveFileName.Value;
            __result = Path.Combine(split);

            Plugin.Log.LogInfo(__result);
        }
    }
}
