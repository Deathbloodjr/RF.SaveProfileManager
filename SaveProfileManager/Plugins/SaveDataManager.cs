using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SaveProfileManager.Plugins
{
    internal static class SaveDataManager
    {
        public static List<SaveData> SaveData = new List<SaveData>();

        public static void Initialize()
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
                    SaveData data = new SaveData(list[i]);
                    Logger.Log("SaveData loaded: " + data.ProfileName);
                    SaveData.Add(data);
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message, LogType.Error);
                    continue;
                }
            }
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
    }
}
