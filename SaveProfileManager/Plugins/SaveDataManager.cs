using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SaveProfileManager.Plugins
{
    public static class SaveDataManager
    {
        static SaveProfile CurrentProfile = null;
        internal static List<SaveProfile> SaveProfiles = new List<SaveProfile>();
        internal static List<PluginSaveDataInterface> Plugins = new List<PluginSaveDataInterface>();
        internal static Dictionary<PluginSaveDataInterface, bool> ActivePlugins = new Dictionary<PluginSaveDataInterface, bool>();

        internal static void Initialize()
        {
            string filePath = Plugin.Instance.ConfigSaveProfileDefinitionsPath.Value;

            if (!File.Exists(filePath))
            {
                CreateDefaultJson();
            }

            JsonArray list = JsonNode.Parse(File.ReadAllText(filePath)).AsArray();
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    SaveProfile data = new SaveProfile(list[i]);
                    Logger.Log("SaveData loaded: " + data.ProfileName);
                    SaveProfiles.Add(data);
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message, LogType.Error);
                    continue;
                }
            }

            if (SaveProfiles.Count > 0)
            {
                CurrentProfile = SaveProfiles[0];
            }
            else
            {
                // Surely there should be DEFAULT at least, no?
                Logger.Log("No SaveData profiles found", LogType.Error);
            }
        }

        internal static bool ChangeProfile(int index)
        {
            SaveProfile switchToProfile = SaveProfiles[index];
            if (switchToProfile != CurrentProfile)
            {
                // This has to be done first
                // Since ReloadMods will change saves ->
                // Mods will get next profile directory from SaveDataManager
                // next profile directory comes from CurrentProfile
                // If this is done last, it gets the directory for the previous profile
                CurrentProfile = switchToProfile;

                GetSavePathHook.ProfileName = switchToProfile.ProfileName;

                ReloadMods(switchToProfile);

                return true;
            }
            return false;
        }

        internal static void ReloadMods(SaveProfile profile)
        {
            // 3 cases can occur
            // Technically 4, but the 4th requires no action
            // The mod is on and needs to remain on
            //     Reload mod
            //     This won't unpatch and repatch. It should just reload any save/config data.
            // The mod is on and needs to turn off
            //     Unload mod
            // The mod is off and needs to turn on
            //     Load mod (Reloading shouldn't be necessary I would think)
            // The mod is off and needs to remain off
            //     Do nothing
            for (int i = 0; i < Plugins.Count; i++)
            {
                var isEnabledMod = ActivePlugins[Plugins[i]];
                var toEnableMod = profile.GetModEnabledStatus(Plugins[i]);
                if (isEnabledMod && toEnableMod)
                {
                    Logger.Log("Reloading plugin " + Plugins[i].Name);
                    Plugins[i].ReloadSaveFunction?.Invoke();
                }
                else if (isEnabledMod && !toEnableMod)
                {
                    Logger.Log("Unloading plugin " + Plugins[i].Name);
                    Plugins[i].UnloadFunction?.Invoke();
                }
                else if (!isEnabledMod && toEnableMod)
                {
                    Logger.Log("Loading plugin " + Plugins[i].Name);
                    Plugins[i].LoadFunction?.Invoke();
                }
                ActivePlugins[Plugins[i]] = toEnableMod;
            }
        }

        public static void AddPluginSaveData(PluginSaveDataInterface plugin)
        {
            for (int i = 0; i < Plugins.Count; i++)
            {
                if (Plugins[i].Name == plugin.Name)
                {
                    Logger.Log("Error. Attempted to add a duplicate plugin to the SaveDataManager");
                    return;
                }
            }

            Plugins.Add(plugin);
            // Every plugin is set active when it's first loaded
            // Only on switching profiles would a plugin potentially be disabled
            ActivePlugins.Add(plugin, true);
            Logger.Log("Plugin added to SaveDataManager: " + plugin.Name);
        }

        public static string GetProfileSaveDirectory(PluginSaveDataInterface plugin)
        {
            string dir = Path.Combine(Plugin.Instance.ConfigModDataFolderPath.Value, plugin.Name, CurrentProfile.ProfileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        static void CreateDefaultJson()
        {
            JsonArray node = new JsonArray()
            {
                new JsonObject()
                {
                    ["ProfileName"] = "Default",
                    ["ProfileColor"] = "#FFFFFF",
                    ["ModsEnabledByDefault"] = false,
                    ["Mods"] = new JsonArray() {
                        new JsonObject()
                        {
                            ["ModGuid"] = "com.DB.RF.SingleHitBigNotes",
                            ["Enabled"] = false
                        }
                    }
                }
            };

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            string filePath = Plugin.Instance.ConfigSaveProfileDefinitionsPath.Value;
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            File.WriteAllText(Plugin.Instance.ConfigSaveProfileDefinitionsPath.Value, node.ToJsonString(options));
        }

        internal static SaveProfile GetCurrentSaveProfile()
        {
            return CurrentProfile;
        }
        internal static int GetIndexOfCurrentProfile()
        {
            for (int i = 0; i < SaveProfiles.Count; i++)
            {
                if (SaveProfiles[i] == CurrentProfile)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
