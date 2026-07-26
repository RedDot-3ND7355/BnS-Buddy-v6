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
        public void ReturnExecFileLocation(object region)
        {
            if (region == null)
                return;
            region = RegionConvert.Convert(region.ToString());
            ExecFolder = Form1.CurrentForm.ClientPaths.Installs[region.ToString()] + "BNSR\\Binaries\\Win64\\";
            ExecPath = Form1.CurrentForm.ClientPaths.Installs[region.ToString()] + "BNSR\\Binaries\\Win64\\BNSR.exe";
        }
    }
}
