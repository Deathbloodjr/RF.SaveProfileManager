using HarmonyLib;
using Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveProfileManager.Plugins
{
    internal class AddSongsToLibraryPatch
    {
        // This patch will add any currently downloaded songs to the library
        // This basically only occurs when one profile downloads songs, any other profile wouldn't have downloaded the song, but the song files will exist
        // This prevents the song from being able to be downloaded, thus leaving it out of the library

        [HarmonyPatch(typeof(UserData))]
        [HarmonyPatch(nameof(UserData.OnLoaded))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UserData_OnLoaded_Postfix(UserData __instance)
        {
            for (int i = 0; i < __instance.MusicsData.Datas.Length; i++)
            {
                var data = __instance.MusicsData.Datas[i];
                if (data is not null)
                {
                    if (!data.IsDownloaded && PackedSongUtility.CheckSongFileExists(i))
                    {
                        __instance.MusicsData.AddDownloadedSong(i);
                    }
                }
            }
        }
    }
}
