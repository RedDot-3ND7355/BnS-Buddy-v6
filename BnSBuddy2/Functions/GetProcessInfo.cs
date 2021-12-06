using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Functions
{
    public class GetProcessInfo
    {
        public string Args = "";
        public GetProcessInfo(Process process) 
        {
            Args = GetProcessArguments(process);
        }

        private string GetProcessArguments(Process process)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                    using (ManagementObjectCollection objects = searcher.Get())
                        return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            } 
            catch
            {
                return "";
            }
        }

    }
}
