using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveProfileSwitcher.Plugins
{
    internal class SaveData
    {
        public string ProfileName { get; }
        //public string ProfilePicturePath { get; set; }
        public Color ProfileColor { get; }
        //public string UserName { get; set; }

        public SaveData(JsonNode? jsonNode)
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
        }
    }
}
