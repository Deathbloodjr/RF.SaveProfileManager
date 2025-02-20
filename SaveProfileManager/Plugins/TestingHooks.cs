using HarmonyLib;
using Platform;
using Platform.Steam;
using Scripts.OutGame.SongSelect;
using Scripts.OutGame.Title;
using Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveProfileManager.Plugins
{
    internal class TestingHooks
    {
        public static TitleSceneUiController titleSceneInstance;

        [HarmonyPatch(typeof(TitleSceneUiController))]
        [HarmonyPatch(nameof(TitleSceneUiController.StartAsync))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool TitleSceneUiController_StartAsync_Prefix(TitleSceneUiController __instance)
        {
            Plugin.Log.LogInfo("TitleSceneUiController_StartAsync_Prefix");

            GameObject obj = new GameObject("Thing");
            GameObject canvas = GameObject.Find("CanvasFg");
            obj.transform.SetParent(canvas.transform);

            obj.AddComponent<SaveProfileManagerObject>();

            titleSceneInstance = __instance;

            return true;
        }
    }
}
