using BnSBuddy2.Functions;

namespace BnSBuddy2.Pathing
{
    public class ExecuteableFile
    {
        // Globals
        public string ExecPath = "";
        public string ExecFolder = "";
        // End Globlas

        // Return exec path
        public void ReturnExecFileLocation(string region)
        {
            region = RegionConvert.Convert(region);
            ExecFolder = Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Binaries\\Win64\\";
            ExecPath = Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Binaries\\Win64\\BNSR.exe";
        }
    }
}
