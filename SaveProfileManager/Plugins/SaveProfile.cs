using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveProfileManager.Plugins
{
    internal class SaveProfile
    {
        public string ProfileName { get; private set; }
        //public string ProfilePicturePath { get; set; }
        public Color ProfileColor { get; private set; } = Color.clear;
        //public string UserName { get; set; }
        public bool ModsEnabledByDefault { get; private set; } = false;

        public List<SaveProfileMod> Mods { get; private set; } = new List<SaveProfileMod>();

        public SaveProfile(JsonNode? jsonNode)
        {
            Initialize(jsonNode);
        }

        void Initialize(JsonNode? jsonNode)
        {
            if (jsonNode == null)
            {
                throw new ArgumentNullException(nameof(jsonNode));
            }

            // ProfileName is required
            ProfileName = jsonNode["ProfileName"]!.GetValue<string>();

            // ProfileColor is optional
            if (jsonNode["ProfileColor"] is not null)
            {
                if (ColorUtility.DoTryParseHtmlColor(jsonNode["ProfileColor"]!.GetValue<string>(), out var color))
                {
                    ProfileColor = color;
                }
            }

            // ModsEnabledByDefault is optional
            if (jsonNode["ModsEnabledByDefault"] is not null)
            {
                ModsEnabledByDefault = jsonNode["ModsEnabledByDefault"]!.GetValue<bool>();
            }

            // Mods is optional
            if (jsonNode["Mods"] is not null)
            {
                var modArray = jsonNode["Mods"].AsArray();
                for (int i = 0; i < modArray.Count; i++)
                {
                    try
                    {
                        var modName = modArray[i]["ModGuid"].GetValue<string>();
                        var modEnabled = modArray[i]["Enabled"].GetValue<bool>();
                        if (Mods.FindIndex((x) => x.PluginGuid == modName) != -1)
                        {
                            List<string> output = new List<string>()
                            {
                                "Error parsing mod in SaveProfile " + ProfileName,
                                "Mod declared multiple times",
                            };
                            Logger.Log(output);
                        }
                        Mods.Add(new SaveProfileMod(modName, modEnabled));
                    }
                    catch
                    {
                        Logger.Log("Error parsing mod in SaveProfile " + ProfileName);
                    }
                }
            }
        }
        
        public bool GetModEnabledStatus(PluginSaveDataInterface plugin)
        {
            for (int i = 0; i < Mods.Count; i++)
            {
                if (Mods[i].PluginGuid == plugin.Name)
                {
                    return Mods[i].IsEnabled;
                }
            }
            return ModsEnabledByDefault;
        }





    }
}
