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
        static string BackupPath => Plugin.Instance.ConfigBackupFolderPath.Value;

        const string DateTimeFormat = "yyyy-MM-dd_HH-mm-ss";

        public static void CreateBackup()
        {
            var now = DateTime.Now;
            var latestBackup = GetLatestBackupDateTime();

            // If you rewind your system time to a previous day
            // Should it backup?
            // I vote yes, for this hypothetical situation
            // Someone fastfowards their time to 1 year in the future for whatever reason
            // Backup is made
            // They return to current date
            // No backups are made for a full year - Bad
            if (latestBackup.Date == now.Date)
            {
                return;
            }

            Logger.Log("Creating Save Backup");

            // Make sure Backup path is not within ModData path
            // If it is, revert Backup path to default maybe?
            if (BackupPath.Contains(Plugin.Instance.ConfigModDataFolderPath.Value))
            {
                Logger.Log("Backup path cannot be within ModData path. Reverting to default backup path", LogType.Warning);
                Plugin.Instance.ConfigBackupFolderPath.Value = (string)Plugin.Instance.ConfigBackupFolderPath.DefaultValue;
            }


            var directoryPath = Path.Combine(BackupPath, now.ToString(DateTimeFormat));
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
                    var dirDateTime = DateTime.ParseExact(subDirs[i].Name, DateTimeFormat, CultureInfo.InvariantCulture);
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
