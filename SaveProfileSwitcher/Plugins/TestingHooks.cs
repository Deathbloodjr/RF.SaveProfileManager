using HarmonyLib;
using Platform;
using Platform.Steam;
using Scripts.OutGame.Title;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveProfileSwitcher.Plugins
{
    internal class TestingHooks
    {
        //[HarmonyPatch(typeof(SaveSystem))]
        //[HarmonyPatch(nameof(SaveSystem.InitializeAsync))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPrefix]
        //public static void SaveSystem_InitializeAsync_Prefix(SaveSystem __instance)
        //{
        //    Logger.Log("SaveSystem_InitializeAsync_Prefix");

        //}

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

            obj.AddComponent<SaveProfileSwitcherObject>();
            
            return true;
        }
    }
}
