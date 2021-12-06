using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.Functions
{
    public class MemoryCleaner
    {
        RAMcs RAMcs = new RAMcs();
        private void wait(int milliseconds)
        {
            Timer timer_wait = new Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer_wait.Interval = milliseconds;
            timer_wait.Enabled = true;
            timer_wait.Start();
            timer_wait.Tick += (s, e) =>
            {
                timer_wait.Enabled = false;
                timer_wait.Stop();
            };
            while (timer_wait.Enabled)
            {
                Application.DoEvents();
            }
        }
        public void CleanMem()
        {
            try
            {
                List<Process> processes = new List<Process>();
                processes.AddRange(Process.GetProcessesByName("BNSR").ToList());
                processes.AddRange(Process.GetProcessesByName("Client64").ToList());
                processes.AddRange(Process.GetProcessesByName(Form1.CurrentForm.materialTextBox5.Text.Replace(".exe", "")).ToList());
                if (processes.Count() > 0)
                {
                    Form1.CurrentForm.AddLauncherLog("Cleaning Memory...");
                    if (RAMcs.IsCounters)
                    {
                        var curr_ram = new RAMcs().Current_Usage();
                        var rounded_curr = Math.Round((Decimal)curr_ram, 2, MidpointRounding.AwayFromZero);
                        Form1.CurrentForm.AddLauncherLog("Before: " + rounded_curr + "%");
                    }
                    else
                    {
                        var curr_ram = new RAMcs().No_Counters_Curr_Usage();
                        var rounded_curr = Math.Round((Decimal)curr_ram, 2, MidpointRounding.AwayFromZero);
                        Form1.CurrentForm.AddLauncherLog("Before: " + rounded_curr + "%");
                    }
                    using (var Dispo = new DispoData())
                    {
                        foreach (Process prs_ss in processes)
                        {
                            if (prs_ss != null)
                                try
                                {
                                    prs_ss.MinWorkingSet = (IntPtr)(300000);
                                }
                                catch (Exception exception)
                                {
                                    throw new Exception(exception.Message);
                                }
                        }
                        GCSettings.LatencyMode = GCLatencyMode.Interactive;
                        GC.Collect();
                        GC.Collect(1, GCCollectionMode.Forced, blocking: false);
                        GC.Collect(2, GCCollectionMode.Forced, blocking: false);
                        GC.Collect(3, GCCollectionMode.Forced, blocking: false);
                        GC.WaitForPendingFinalizers();
                    }
                    wait(2500);
                    if (RAMcs.IsCounters)
                    {
                        var af_ram = new RAMcs().Current_Usage();
                        var rounded_af = Math.Round((Decimal)af_ram, 2, MidpointRounding.AwayFromZero);
                        Form1.CurrentForm.AddLauncherLog("After: " + rounded_af + "%");
                    }
                    else
                    {
                        var af_ram = new RAMcs().No_Counters_Curr_Usage();
                        var rounded_af = Math.Round((Decimal)af_ram, 2, MidpointRounding.AwayFromZero);
                        Form1.CurrentForm.AddLauncherLog("After: " + rounded_af + "%");
                    }
                    Form1.CurrentForm.AddLauncherLog("Memory Cleaned");
                }
            } 
            catch
            {
                Form1.CurrentForm.AddLauncherLog("[Error] Failed Cleaning Memory!");
            }
        }

        // Dispose
        class DispoData : IDisposable
        {
            public void Dispose(){}
        }
    }
}
