using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using SaveProfileManager.Plugins;
using UnityEngine;
using System.Collections;

namespace SaveProfileManager
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public const string ModName = "SaveProfileManager";

        public static Plugin Instance;
        private Harmony _harmony = null;
        public new static ManualLogSource Log;


        public ConfigEntry<bool> ConfigEnabled;
        //public ConfigEntry<string> ConfigSaveFileName;
        public ConfigEntry<string> ConfigSaveProfileDefinitionsPath;
        public ConfigEntry<string> ConfigModDataFolderPath;



        public override void Load()
        {
            Instance = this;

            Log = base.Log;

            SetupConfig();
            SetupHarmony();
        }

        private void SetupConfig()
        {
            var dataFolder = Path.Combine("BepInEx", "data", ModName);

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");

            //ConfigSaveFileName = Config.Bind("General",
            //    "SaveFileName",
            //    "",
            //    "Sets the save file name to use. Leave blank for your default save.");

            ConfigSaveProfileDefinitionsPath = Config.Bind("General",
                "SaveProfileDefinitionsPath",
                Path.Combine(dataFolder, "SaveProfileDefinitions.json"),
                "The path to the json file containing save profile definitions. ");

            ConfigModDataFolderPath = Config.Bind("General",
                "ModDataFolderPath",
                Path.Combine(dataFolder, "ModData"),
                "The path that will contain any mod data. This includes saves and configs. ");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            if (ConfigEnabled.Value)
            {
                bool result = true;
                // If any PatchFile fails, result will become false
                result &= PatchFile(typeof(GetSavePathHook));
                result &= PatchFile(typeof(AddSongsToLibraryPatch));
                result &= PatchFile(typeof(TestingHooks));
                if (result)
                {
                    SaveDataManager.Initialize();
                    Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
                }
                else
                {
                    Log.LogError($"Plugin {MyPluginInfo.PLUGIN_GUID} failed to load.");
                    // Unload this instance of Harmony
                    // I hope this works the way I think it does
                    _harmony.UnpatchSelf();
                }
            }
            else
            {
                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        private bool PatchFile(Type type)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            }
            try
            {
                _harmony.PatchAll(type);
#if DEBUG
                Log.LogInfo("File patched: " + type.FullName);
#endif
                return true;
            }
            catch (Exception e)
            {
                Log.LogInfo("Failed to patch file: " + type.FullName);
                Log.LogInfo(e.Message);
                return false;
            }
        }

        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;
        public void StartCoroutine(IEnumerator enumerator)
        {
            GetMonoBehaviour().StartCoroutine(enumerator);
        }
    }
}
