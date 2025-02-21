using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace SaveProfileManager.Plugins
{
    internal class BackupManager
    {
        static string BackupPath = Plugin.Instance.ConfigBackupFolderPath.Value;

        public static void CreateBackup()
        {
            var now = DateTime.Now;
            var latestBackup = GetLatestBackupDateTime();

            if (latestBackup.Date == now.Date)
            {
                return;
            }

            var directoryPath = Path.Combine(BackupPath, now.ToString("yyyy-MM-dd_HH-mm-ss"));
            // The directory should never exist ahead of time
            // Even if it does, creating the directory just wouldn't do anything then (I think)
            Directory.CreateDirectory(directoryPath);

            ZipDirectory("tkdat", Path.Combine(directoryPath, "tkdat.zip"));
            ZipDirectory(Plugin.Instance.ConfigModDataFolderPath.Value, Path.Combine(directoryPath, "ModData.zip"));
        }

        static DateTime GetLatestBackupDateTime()
        {
            DateTime latestDateTime = DateTime.MinValue;
            DirectoryInfo dirInfo = new DirectoryInfo(BackupPath);
            if (!dirInfo.Exists)
            {
                return latestDateTime;
            }
            var subDirs = dirInfo.GetDirectories();
            for (int i = 0; i < subDirs.Length; i++)
            {
                try
                {
                    var dirDateTime = DateTime.ParseExact(subDirs[i].Name, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
                    if (dirDateTime > latestDateTime)
                    {
                        latestDateTime = dirDateTime;
                    }
                }
                catch
                {

                }
                
            }
            return latestDateTime;
        }

        static void ZipDirectory(string sourceDirectory, string destinationZipFile)
        {
            try
            {
                using (FileStream zipFile = File.Create(destinationZipFile))
                using (ZipArchive archive = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    string[] files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
                    string directoryName = Path.GetFileName(sourceDirectory);

                    foreach (string file in files)
                    {
                        string relativePath = file.Substring(sourceDirectory.Length).TrimStart(Path.DirectorySeparatorChar);
                        string entryPath = Path.Combine(directoryName, relativePath);
                        archive.CreateEntryFromFile(file, entryPath);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($" Error creating zipping backup: {e.Message}");
            }
        }
    }
}
