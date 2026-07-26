using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace BnSBuddy2.Functions
{
    public static class FasterToggle
    {
        private static string PathToMovies = "";
        private static string PathToLoadingScreenLang = "";
        private static string PathToLoadingScreenStatic = "";

        public static void CheckState(object region, object language)
        {
            if (region == null || language == null)
                return;
            region = RegionConvert.Convert(region.ToString());
            PathToMovies = RecursiveFindMovies(region.ToString());
            PathToLoadingScreenLang = RecursiveFindLoading(region.ToString(), language.ToString());
            PathToLoadingScreenStatic = RecursiveFindStaticLoading(region.ToString());
            bool Movies = CheckMovieState(PathToMovies);
            bool Loading1 = CheckLoadingScreenState(PathToLoadingScreenLang);
            bool Loading2 = (PathToLoadingScreenStatic != "" ? CheckLoadingScreenState(PathToLoadingScreenStatic) : true);
            if (Movies && Loading1 && Loading2)
                Form1.CurrentForm.materialSwitch1.Checked = true;
        }

        private static string RecursiveFindStaticLoading(string region)
        {
            if (Directory.Exists(Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Content\\local\\ZNCS\\Public"))
                return Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Content\\local\\ZNCS\\Public";
            else
                return "";
        }

        private static string RecursiveFindMovies(string region)
        {
            return Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Content\\Movies";
        }

        private static string RecursiveFindLoading(string region, string language)
        {

            string ClientPath = Form1.CurrentForm.ClientPaths.Installs[region];
            if (Directory.Exists(ClientPath + "BNSR\\Content\\LoadingImage") || Directory.Exists(ClientPath + "BNSR\\Content\\_LoadingImage")) // KR Support
                return ClientPath + "BNSR\\Content";
            else if (Directory.Exists(ClientPath + "BNSR\\Content\\local"))
                foreach (DirectoryInfo dir in new DirectoryInfo(ClientPath + "BNSR\\Content\\local").GetDirectories())
                    if (Directory.Exists(ClientPath + "BNSR\\Content\\local\\" + Path.GetFileName(dir.ToString()) + "\\data"))
                        if (Directory.Exists(ClientPath + "BNSR\\Content\\local\\" + Path.GetFileName(dir.ToString()) + "\\" + language))
                            return ClientPath + "BNSR\\Content\\local\\" + Path.GetFileName(dir.ToString()) + "\\" + language;
            return "";
        }

        // Toggle Both
        public static void Toggle(bool enableboost, string region, string language)
        {
            ToggleMovie(enableboost, PathToMovies);
            ToggleLoading(enableboost, PathToLoadingScreenLang);
            if (PathToLoadingScreenStatic != "")
                ToggleLoading(enableboost, PathToLoadingScreenStatic);
        }

        // Toggle Movie
        private static void ToggleMovie(bool enableboost, string Path2Movies)
        {
            if (enableboost)
            {
                if (File.Exists(Path2Movies + "\\BNSR_CI.mp4"))
                {
                    File.Copy(Path2Movies + "\\BNSR_CI.mp4", Path2Movies + "\\_BNSR_CI.mp4", true);
                    File.Delete(Path2Movies + "\\BNSR_CI.mp4");
                }
                if (File.Exists(Path2Movies + "\\UE4_Branding.mp4"))
                {
                    File.Copy(Path2Movies + "\\UE4_Branding.mp4", Path2Movies + "\\_UE4_Branding.mp4", true);
                    File.Delete(Path2Movies + "\\UE4_Branding.mp4");
                }
            }
            else
            {
                if (File.Exists(Path2Movies + "\\_BNSR_CI.mp4"))
                {
                    File.Copy(Path2Movies + "\\_BNSR_CI.mp4", Path2Movies + "\\BNSR_CI.mp4", true);
                    File.Delete(Path2Movies + "\\_BNSR_CI.mp4");
                }
                if (File.Exists(Path2Movies + "\\_UE4_Branding.mp4"))
                {
                    File.Copy(Path2Movies + "\\_UE4_Branding.mp4", Path2Movies + "\\UE4_Branding.mp4", true);
                    File.Delete(Path2Movies + "\\_UE4_Branding.mp4");
                }
            }
        }

        // Toggle Loading
        private static void ToggleLoading(bool enableboost, string Path2LoadingScreenLang)
        {
            if (enableboost)
            {
                // Check if old Exists
                if (Directory.Exists(Path2LoadingScreenLang + "\\LoadingImage") && Directory.Exists(Path2LoadingScreenLang + "\\_LoadingImage"))
                    Directory.Delete(Path2LoadingScreenLang + "\\_LoadingImage", true);
                // Move new Folder
                if (Directory.Exists(Path2LoadingScreenLang + "\\LoadingImage"))
                {
                    FileSystem.CopyDirectory(Path2LoadingScreenLang + "\\LoadingImage", Path2LoadingScreenLang + "\\_LoadingImage");
                    Directory.Delete(Path2LoadingScreenLang + "\\LoadingImage", true);
                }
            }
            else
            {
                // Check if new Exists
                if (Directory.Exists(Path2LoadingScreenLang + "\\_LoadingImage") && Directory.Exists(Path2LoadingScreenLang + "\\LoadingImage"))
                    Directory.Delete(Path2LoadingScreenLang + "\\_LoadingImage", true);
                // Move old Folder
                if (Directory.Exists(Path2LoadingScreenLang + "\\_LoadingImage"))
                {
                    FileSystem.CopyDirectory(Path2LoadingScreenLang + "\\_LoadingImage", Path2LoadingScreenLang + "\\LoadingImage");
                    Directory.Delete(Path2LoadingScreenLang + "\\_LoadingImage", true);
                }
            }
        }

        // Check Movie
        private static bool CheckMovieState(string Path2Movies)
        {
            if (File.Exists(Path2Movies + "\\_BNSR_CI.mp4") && File.Exists(Path2Movies + "\\_UE4_Branding.mp4"))
            {
                // Copy any new files to replace backup
                if (File.Exists(Path2Movies + "\\BNSR_CI.mp4") && File.Exists(Path2Movies + "\\UE4_Branding.mp4"))
                {
                    File.Copy(Path2Movies + "\\BNSR_CI.mp4", Path2Movies + "\\_BNSR_CI.mp4", true);
                    File.Copy(Path2Movies + "\\UE4_Branding.mp4", Path2Movies + "\\_UE4_Branding.mp4", true);
                    File.Delete(Path2Movies + "\\BNSR_CI.mp4");
                    File.Delete(Path2Movies + "\\UE4_Branding.mp4");
                }
                // return result
                return true;
            }
            return false;
        }

        // Check Loading
        private static bool CheckLoadingScreenState(string Path2LoadingScreenLang)
        {
            if (Directory.Exists(Path2LoadingScreenLang + "\\_LoadingImage"))
            {
                // Copy any new files to replace backup
                if (Directory.Exists(Path2LoadingScreenLang + "\\LoadingImage"))
                {
                    Directory.Delete(Path2LoadingScreenLang + "\\_LoadingImage", true);
                    FileSystem.CopyDirectory(Path2LoadingScreenLang + "\\LoadingImage", Path2LoadingScreenLang + "\\_LoadingImage", UIOption.OnlyErrorDialogs, UICancelOption.DoNothing);
                    Directory.Delete(Path2LoadingScreenLang + "\\LoadingImage", true);
                }
                // return result
                return true;
            }
            return false;
        }
    }
}
