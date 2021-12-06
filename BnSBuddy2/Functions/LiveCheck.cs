using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace BnSBuddy2.Functions
{
    public class LiveCheck
    {
        private static bool BusyTimer = false;
        public static void Check()
        {
            if (BusyTimer)
                return;
            BusyTimer = true;
            if (Form1.CurrentForm.ActiveSessions.GetActiveClientCount() == 0)
            {
                foreach (string users in Form1.CurrentForm.ActiveSessions.DiedClients)
                    Form1.CurrentForm.AddLauncherLog(users + "'s Game Process Died");
                Form1.CurrentForm.PlayButton.Text = "Play!";
                Form1.CurrentForm.materialButton2.Enabled = false;
                if (Form1.CurrentForm.materialSwitch46.Checked && Form1.CurrentForm.AppStarted && !Form1.CurrentForm.CalledKiller)
                {
                    // Find any unsupported clients for relaunch!
                    Form1.CurrentForm.ActiveSessions.CapturedClients = new List<ProcessStartInfo>();
                    List<Process> processes = new List<Process>();
                    processes.AddRange(Process.GetProcessesByName("BNSR").ToList());
                    processes.AddRange(Process.GetProcessesByName("Client64").ToList());
                    processes.AddRange(Process.GetProcessesByName(Form1.CurrentForm.materialTextBox5.Text.Replace(".exe", "")).ToList());
                    //
                    if (processes.Count > 0)
                    {
                        int i = 0;
                        foreach (Process GameProcess in processes)
                        {
                            Form1.CurrentForm.ActiveSessions.GetActiveClientCount();
                            bool AlreadyCaptured = false;
                            // Check if process not already capture! Return if yes
                            foreach (string key in Form1.CurrentForm.ActiveSessions.UnhandledClients.Keys)
                                if (Form1.CurrentForm.ActiveSessions.UnhandledClients[key] == GameProcess.Id)
                                    AlreadyCaptured = true;
                            foreach (string region in Form1.CurrentForm.ActiveSessions.ActiveClients.Keys)
                                foreach (string username in Form1.CurrentForm.ActiveSessions.ActiveClients[region].Keys)
                                    if (Form1.CurrentForm.ActiveSessions.ActiveClients[region][username] == GameProcess.Id)
                                        AlreadyCaptured = true;
                            // Continue
                            try
                            {
                                if (Form1.CurrentForm.Game.ProcessExists(GameProcess.Id) && GameProcess.MainModule != null && !AlreadyCaptured)
                                {
                                    GameProcess.StartInfo.FileName = GameProcess.MainModule.FileName;
                                    GameProcess.StartInfo.Arguments = new GetProcessInfo(GameProcess).Args.Replace("\"" + GameProcess.StartInfo.FileName + "\" ", "");
                                    if (GameProcess.StartInfo.Arguments != "")
                                    {
                                        Form1.CurrentForm.ActiveSessions.CapturedClients.Add(GameProcess.StartInfo);
                                        GameProcess.Kill();
                                        i++;
                                    }
                                }
                            }
                            catch (Win32Exception) { GameProcess.Kill(); }
                        }
                        if (Form1.CurrentForm.ActiveSessions.CapturedClients.Count > 0)
                        {
                            Prompt.Popup("Captured " + Form1.CurrentForm.ActiveSessions.CapturedClients.Count + " game " + (Form1.CurrentForm.ActiveSessions.CapturedClients.Count == 1 ? "process" : "processes") + " and they are ready for launch!" + Environment.NewLine + "Feel free to apply addons and any other needed modifications.");
                            Form1.CurrentForm.PlayButton.Enabled = true;
                        }
                    }
                }
            }
            else
                foreach (string users in Form1.CurrentForm.ActiveSessions.DiedClients)
                    Form1.CurrentForm.AddLauncherLog(users + "'s Game Process Died");
            if (Form1.CurrentForm.CalledKiller)
                Form1.CurrentForm.CalledKiller = false;
            BusyTimer = false;
        }
    }
}
