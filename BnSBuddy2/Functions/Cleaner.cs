using System.IO;

namespace BnSBuddy2.Functions
{
    public class Cleaner
    {
        public void MessCleaner()
        {
            if (Directory.Exists(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local"))
                Directory.Delete(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local", true);
            if (Directory.Exists(Form1.CurrentForm.PakPaths.PakPath + "Mods\\Pak0-Local_p"))
                Directory.Delete(Form1.CurrentForm.PakPaths.PakPath + "Mods\\Pak0-Local_p", true);
            if (Directory.Exists(Path.GetTempPath() + "config64.dat"))
                Directory.Delete(Path.GetTempPath() + "config64.dat", true);
            if (Directory.Exists(Path.GetTempPath() + "xml64.dat"))
                Directory.Delete(Path.GetTempPath() + "xml64.dat", true);
            if (File.Exists(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.pak") && File.Exists(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local_p.pak"))
                File.Delete(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local_p.pak");
            if (File.Exists(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.sig") && File.Exists(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local_p.sig"))
                File.Delete(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local_p.sig");
            if (File.Exists(Path.GetTempPath() + "quickbms.zip"))
                File.Delete(Path.GetTempPath() + "quickbms.zip");
            Form1.CurrentForm.AddLauncherLog("Cleaned Mess.");
        }
    }
}
