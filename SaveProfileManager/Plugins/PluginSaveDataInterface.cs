using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveProfileManager.Plugins
{
    public delegate void SaveManagerFunction();

    public class PluginSaveDataInterface
    {
        public string Name { get; set; }

        internal SaveManagerFunction? LoadFunction = null;
        internal SaveManagerFunction? UnloadFunction = null;
        internal SaveManagerFunction? ReloadSaveFunction = null;

        public PluginSaveDataInterface(string name)
        {
            Name = name;
        }

        public void AssignLoadFunction(SaveManagerFunction loadFunction)
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

        public void AddToManager()
        {
            if (LoadFunction is not null &&
                UnloadFunction is not null)
            {
                SaveDataManager.AddPluginSaveData(this);
            }
            else
            {
                List<string> output = new List<string>()
                {
                    "Error adding plugin to SaveProfileManager: " + this.Name,
                    "LoadFunction or UnloadFunction were null.",
                };
                Logger.Log(output, LogType.Error);
            }
        }

    }
}
