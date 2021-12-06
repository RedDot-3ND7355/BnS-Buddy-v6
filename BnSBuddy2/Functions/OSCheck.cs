using System;

namespace BnSBuddy2.Functions
{
    class OSCheck
    {
        public static OperatingSystem OSVERSION = Environment.OSVersion;

        public OSCheck()
        {
            if (int.Parse(OSVERSION.Version.Major.ToString()) < 6)
            {
                Prompt.Popup("You must have atleast Windows Vista SP2 or above for BnS Buddy to work.");
                new KillApp();
            }
        }
    }
}
