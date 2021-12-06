using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Functions
{
    public static class GetAffinityClass
    {
        public static IntPtr AffinityClass(string setting = "All")
        {
            int allCpu = (int)Math.Pow(2, Form1.CurrentForm.cpuCount) - 1;
            int firstHalf = (int)Math.Pow(2, Form1.CurrentForm.cpuCount / 2) - 1;
            int secondHalf = allCpu - firstHalf;
            int odd = 0;
            for (int i = 1; i < Form1.CurrentForm.cpuCount; i++) { if (i % 2 != 0) { odd += (int)Math.Pow(2, i - 1); } };
            int even = allCpu - odd;
            switch (setting)
            {
                case "All":
                    return (IntPtr)allCpu;
                case "Odd":
                    return (IntPtr)odd;
                case "Even":
                    return (IntPtr)even;
                case "First Half":
                    return (IntPtr)firstHalf;
                case "Second Half":
                    return (IntPtr)secondHalf;
                case "Custom":
                    return (IntPtr)Form1.CurrentForm.customValue;
            }
            return (IntPtr)allCpu;
        }
    }
}
