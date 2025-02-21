using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveProfileManager.Plugins
{
    internal class SaveProfileMod
    {
        public string PluginGuid { get; set; }
        public bool IsEnabled { get; set; }

        public SaveProfileMod(string pluginName, bool isEnabled)
        {
            PluginGuid = pluginName; 
            IsEnabled = isEnabled;
        }
    }
}
