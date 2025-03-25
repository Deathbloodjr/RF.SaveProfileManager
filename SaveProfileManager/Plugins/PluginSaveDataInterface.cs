using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveProfileManager.Plugins
{
    public delegate void SaveManagerLoadFunction(bool enabled);
    public delegate void SaveManagerFunction();
    public delegate void SaveManagerConfigSetupFunction(ConfigFile config, string saveFolder, bool isSaveManager);

    public class PluginSaveDataInterface
    {
        /// <summary>
        /// Guid of plugin
        /// </summary>
        public string Name { get; set; }

        internal SaveManagerLoadFunction? LoadFunction = null;
        internal SaveManagerFunction? UnloadFunction = null;
        internal SaveManagerFunction? ReloadSaveFunction = null;
        internal SaveManagerConfigSetupFunction? ConfigSetupFunction = null;

        public PluginSaveDataInterface(string name)
        {
            Name = name;
        }

        public void AssignLoadFunction(SaveManagerLoadFunction loadFunction)
        {
            LoadFunction = loadFunction;
        }

        public void AssignUnloadFunction(SaveManagerFunction unloadFunction)
        {
            UnloadFunction = unloadFunction;
        }

        public void AssignReloadSaveFunction(SaveManagerFunction reloadSaveFunction)
        {
            ReloadSaveFunction = reloadSaveFunction;
        }

        public void AssignConfigSetupFunction(SaveManagerConfigSetupFunction configSetupFunction)
        {
            ConfigSetupFunction = configSetupFunction;
        }

        public void AddToManager(bool isEnabled)
        {
            if (LoadFunction is not null &&
                UnloadFunction is not null)
            {
                SaveDataManager.AddPluginSaveData(this, isEnabled);
            }
            else
            {
                List<string> output = new List<string>()
                {
                    "Error adding plugin to SaveProfileManager: " + this.Name,
                };
                if (LoadFunction is null)
                {
                    output.Add("LoadFunction is null");
                }
                if (UnloadFunction is null)
                {
                    output.Add("UnloadFunction is null");
                }
                Logger.Log(output, LogType.Error);
            }
        }

    }
}
