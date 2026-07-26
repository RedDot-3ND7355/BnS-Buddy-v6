using System;
using System.IO;
using System.Windows.Forms;

namespace BnSBuddy2.LegacyBin
{
    /// <summary>
    /// Remembers the last folder used by open/save/browse dialogs across sessions.
    /// </summary>
    public static class DialogPaths
    {
        private static string _lastDirectory;
        private static bool _loaded;

        private static string SettingsFile
        {
            get
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "LegacyBin");
                return Path.Combine(dir, "last-dialog-folder.txt");
            }
        }

        public static string LastDirectory
        {
            get
            {
                EnsureLoaded();
                return _lastDirectory;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                try
                {
                    string full = Path.GetFullPath(value);
                    if (!Directory.Exists(full))
                    {
                        return;
                    }
                    _lastDirectory = full;
                    _loaded = true;
                    Persist();
                }
                catch
                {
                    // ignore invalid paths
                }
            }
        }

        public static void Apply(FileDialog dialog)
        {
            if (dialog == null)
            {
                return;
            }
            dialog.RestoreDirectory = true;
            string dir = LastDirectory;
            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            {
                dialog.InitialDirectory = dir;
            }
        }

        public static void Apply(FolderBrowserDialog dialog)
        {
            if (dialog == null)
            {
                return;
            }
            string dir = LastDirectory;
            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            {
                dialog.SelectedPath = dir;
            }
        }

        /// <summary>Call after a successful Open/Save dialog (OK).</summary>
        public static void RememberFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }
            try
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(filePath));
                if (!string.IsNullOrEmpty(dir))
                {
                    LastDirectory = dir;
                }
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>Call after a successful folder browser (OK).</summary>
        public static void RememberFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return;
            }
            try
            {
                LastDirectory = Path.GetFullPath(folderPath);
            }
            catch
            {
                // ignore
            }
        }

        private static void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }
            _loaded = true;
            try
            {
                string file = SettingsFile;
                if (File.Exists(file))
                {
                    string line = File.ReadAllText(file).Trim();
                    if (!string.IsNullOrEmpty(line) && Directory.Exists(line))
                    {
                        _lastDirectory = line;
                    }
                }
            }
            catch
            {
                // ignore
            }
        }

        private static void Persist()
        {
            try
            {
                string file = SettingsFile;
                string dir = Path.GetDirectoryName(file);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(file, _lastDirectory ?? string.Empty);
            }
            catch
            {
                // ignore
            }
        }
    }
}
