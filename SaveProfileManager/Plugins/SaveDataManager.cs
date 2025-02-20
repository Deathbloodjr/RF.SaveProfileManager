﻿using System;
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
        internal static List<PluginSaveDataInterface> PluginSaveDataInterfaces = new List<PluginSaveDataInterface>();

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
                GetSavePathHook.ProfileName = switchToProfile.ProfileName;
                if (switchToProfile.ProfileName == "DEBUG")
                {
                    for (int i = 0; i < PluginSaveDataInterfaces.Count; i++)
                    {
                        PluginSaveDataInterfaces[i].UnloadFunction?.Invoke();
                    }
                }
                else
                {
                    for (int i = 0; i < PluginSaveDataInterfaces.Count; i++)
                    {
                        PluginSaveDataInterfaces[i].LoadFunction?.Invoke();
                    }
                }
                CurrentProfile = switchToProfile;
                return true;
            }
            return false;
        }

        public static void AddPluginSaveData(PluginSaveDataInterface plugin)
        {
            for (int i = 0; i < PluginSaveDataInterfaces.Count; i++)
            {
                if (PluginSaveDataInterfaces[i].Name == plugin.Name)
                {
                    return;
                }
            }

            PluginSaveDataInterfaces.Add(plugin);
            Logger.Log("Plugin added to SaveDataManager: " + plugin.Name);
        }

        static void CreateDefaultJson()
        {
            JsonArray node = new JsonArray();
            JsonObject obj = new JsonObject()
            {
                ["ProfileName"] = "Default",
                ["ProfileColor"] = "#FFFFFF",
            };
            node.Add(obj);

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
