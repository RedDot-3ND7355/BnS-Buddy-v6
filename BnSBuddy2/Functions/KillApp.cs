using _Process = System.Diagnostics.Process;

namespace BnSBuddy2.Functions
{
    class KillApp
    {
        public KillApp()
        {
            _Process.GetCurrentProcess().Kill();
        }
    }
}
