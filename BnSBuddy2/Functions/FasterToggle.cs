using System.IO;

namespace BnSBuddy2.Functions
{
    public static class FasterToggle
    {
        private static string PathToMovies = "";
        private static string PathToLoadingScreenLang = "";

        public static void CheckState(string region, string language)
        {
            region = RegionConvert.Convert(region);
            PathToMovies = RecursiveFindMovies(region);
            PathToLoadingScreenLang = RecursiveFindLoading(region, language);
            if (CheckMovieState(PathToMovies) && CheckLoadingScreenState(PathToLoadingScreenLang))
                Form1.CurrentForm.materialSwitch1.Checked = true;
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
        }

        // Toggle Movie
        private static void ToggleMovie(bool enableboost, string Path2Movies)
        {
            if (enableboost)
            {
                File.Move(Path2Movies + "\\BNSR_CI.mp4", Path2Movies + "\\_BNSR_CI.mp4");
                File.Move(Path2Movies + "\\UE4_Branding.mp4", Path2Movies + "\\_UE4_Branding.mp4");
            }
            else
            {
                File.Move(Path2Movies + "\\_BNSR_CI.mp4", Path2Movies + "\\BNSR_CI.mp4");
                File.Move(Path2Movies + "\\_UE4_Branding.mp4", Path2Movies + "\\UE4_Branding.mp4");
            }
        }

        // Toggle Loading
        private static void ToggleLoading(bool enableboost, string Path2LoadingScreenLang)
        {
            if (enableboost)
                Directory.Move(Path2LoadingScreenLang + "\\LoadingImage", Path2LoadingScreenLang + "\\_LoadingImage");
            else
                Directory.Move(Path2LoadingScreenLang + "\\_LoadingImage", Path2LoadingScreenLang + "\\LoadingImage");
        }

        // Check Movie
        private static bool CheckMovieState(string Path2Movies)
        {
            if (File.Exists(Path2Movies + "\\_BNSR_CI.mp4") && File.Exists(Path2Movies + "\\_UE4_Branding.mp4"))
            {
                // Copy any new files to replace backup
                if (File.Exists(Path2Movies + "\\BNSR_CI.mp4") && File.Exists(Path2Movies + "\\UE4_Branding.mp4"))
                {
                    File.Delete(Path2Movies + "\\_BNSR_CI.mp4");
                    File.Delete(Path2Movies + "\\_UE4_Branding.mp4");
                    File.Move(Path2Movies + "\\BNSR_CI.mp4", Path2Movies + "\\_BNSR_CI.mp4");
                    File.Move(Path2Movies + "\\UE4_Branding.mp4", Path2Movies + "\\_UE4_Branding.mp4");
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
                    Directory.Move(Path2LoadingScreenLang + "\\LoadingImage", Path2LoadingScreenLang + "\\_LoadingImage");
                }
                // return result
                return true;
            }
            return false;
        }
    }
}
