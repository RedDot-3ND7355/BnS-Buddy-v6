using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace BnSBuddy2.CoreHandlers
{
    public class Game
    {
        // Globals
        public int gamesessionid = 0;
        // End Globals

        private void StartCapturedClients()
        {
            foreach (ProcessStartInfo info in Form1.CurrentForm.ActiveSessions.CapturedClients)
            {
                Process GameToStart = new Process();
                GameToStart.StartInfo = info;
                GameToStart.StartInfo.UseShellExecute = false;
                GameToStart.StartInfo.RedirectStandardError = true;
                GameToStart.StartInfo.Arguments += (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                GameToStart.Start();
                GameToStart.PriorityBoostEnabled = Form1.CurrentForm.materialSwitch53.Checked;
                GameToStart.PriorityClass = Form1.CurrentForm.Priority;
                GameToStart.ProcessorAffinity = GetAffinityClass.AffinityClass(Form1.CurrentForm.materialLabel88.Text.ToString().Replace("Affinity: ", ""));
                Form1.CurrentForm.ActiveSessions.UnhandledClients.Add("[Caught] " + GameToStart.Id, GameToStart.Id);
            }
            Form1.CurrentForm.ActiveSessions.CapturedClients = new List<ProcessStartInfo>();
        }

        private void KillGame(int gamesessionid)
        {
            try
            {
                if (Form1.CurrentForm.materialLabel33.Text == "Active")
                {
                    Form1.CurrentForm.ActiveSessions.KillActiveProcesses(true);
                    Form1.CurrentForm.ActiveSessions.ClearInactiveClients();
                }
                else
                {
                    if (ProcessExists(gamesessionid))
                        try
                        {
                            Process.GetProcessById(gamesessionid).Kill();
                        }
                        catch { }
                    Form1.CurrentForm.AddLauncherLog("Killed Game Process.");
                    Form1.CurrentForm.ActiveSessions.ClearInactiveClients();
                }
            }
            catch { }
            Form1.CurrentForm.materialButton2.Enabled = false;
            Form1.CurrentForm.PlayButton.Text = "Play!";
            Form1.CurrentForm.PlayButton.Enabled = true;
        }

        public bool ProcessExists(int iProcessID)
        {
            try
            {
                var tmp = Process.GetProcessById(iProcessID);
                if (tmp.Id == iProcessID)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        // Check if addons are queud
        private bool CheckAddons()
        {
            foreach (TreeNode node in Form1.CurrentForm.treeView3.Nodes)
            {
                if (node != null && node.Checked)
                {
                    if (node.Checked)
                        return true;
                    if (node.Nodes.Count > 0)
                        foreach (TreeNode childnode in node.Nodes)
                            if (childnode != null && childnode.Checked)
                                return true;
                }
            }
            return false;
        }

        private void LaunchGame(string username, string password, string region)
        {
            Form1.CurrentForm.PlayButton.Enabled = false;
            string PathToExe = Form1.CurrentForm.ExecuteableFile.ExecPath;
            if (Form1.CurrentForm.materialTextBox5.Text.Length > 0)
                PathToExe = PathToExe.Replace("BNSR.exe", Form1.CurrentForm.materialTextBox5.Text);
            // Check if EXE exists then proceed
            if (File.Exists(PathToExe))
            {
                // Apply addons
                if (CheckAddons())
                {
                    Form1.CurrentForm.AddLauncherLog("Applying Addons...");
                    Addons.StartGameAddons();
                }
                bool Started = false;
                // Login & start by region
                if (region == "North America" || region == "Europe" || region == "Korean" || region == "Korean Test" || region == "Taiwan")
                {
                    Form1.CurrentForm.AddLauncherLog("Starting Auth...");
                    switch (Form1.CurrentForm.NCHandler.GetLogin(username, password, region)) // Check Maintenance & Get Login
                    {
                        case "Logged":
                            Form1.CurrentForm.Show();
                            break;
                        case "Maintenance":
                            Form1.CurrentForm.PlayButton.Enabled = true;
                            Form1.CurrentForm.AddLauncherLog("Cancelled");
                            break;
                        default:
                            Form1.CurrentForm.PlayButton.Enabled = true;
                            Form1.CurrentForm.AddLauncherLog("Login Failed" + Environment.NewLine + "Cancelled");
                            break;
                    }
                }
                // Start Japanese Client
                else if (region == "Japanese")
                {
                    Form1.CurrentForm.AddLauncherLog("Starting Client!");
                    Process process = new Process();
                    process.StartInfo.FileName = PathToExe;
                    process.StartInfo.Arguments = Form1.CurrentForm.NCHandler.StringBuilder(region) + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = false;
                    try
                    {
                        if (process.Start())
                        {
                            process.PriorityBoostEnabled = Form1.CurrentForm.materialSwitch53.Checked;
                            process.PriorityClass = Form1.CurrentForm.Priority;
                            process.ProcessorAffinity = GetAffinityClass.AffinityClass(Form1.CurrentForm.materialLabel88.Text.ToString().Replace("Affinity: ", ""));
                            Form1.CurrentForm.ActiveSessions.UnhandledClients.Add($"[Japanese] " + new Random().Next(10, 999), process.Id);
                            Form1.CurrentForm.AddLauncherLog("Started " + Path.GetFileName(PathToExe) + "!");
                            gamesessionid = process.Id;
                            Started = true;
                            //disableButtons(); todo
                            Form1.CurrentForm.AddLauncherLog("Notice: BnS Buddy does not directly support Multiclient for Japanese server!");
                            Form1.CurrentForm.WindowState = FormWindowState.Minimized;
                        }
                    }
                    catch
                    {
                        Form1.CurrentForm.Show();
                        Form1.CurrentForm.PlayButton.Enabled = true;
                        Form1.CurrentForm.AddLauncherLog($"[Error] Could Not Start {Path.GetFileName(PathToExe)}!");
                    }
                }
                // Other unsupported Clients
                else
                {
                    Form1.CurrentForm.AddLauncherLog("Starting Client!");
                    Process process2 = new Process();
                    process2.StartInfo.FileName = PathToExe;
                    process2.StartInfo.Arguments = Form1.CurrentForm.NCHandler.StringBuilder(region) + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    process2.StartInfo.UseShellExecute = false;
                    process2.StartInfo.RedirectStandardError = false;
                    try
                    {
                        if (process2.Start())
                        {
                            process2.PriorityBoostEnabled = Form1.CurrentForm.materialSwitch53.Checked;
                            process2.PriorityClass = Form1.CurrentForm.Priority;
                            process2.ProcessorAffinity = GetAffinityClass.AffinityClass(Form1.CurrentForm.materialLabel88.Text.ToString().Replace("Affinity: ", ""));
                            if (username == "")
                                username = new Random().Next(10, 999).ToString();
                            Form1.CurrentForm.ActiveSessions.Add(region, username, process2.Id);
                            Form1.CurrentForm.AddLauncherLog("Started " + Path.GetFileName(PathToExe) + "!");
                            gamesessionid = process2.Id;
                            Started = true;
                            Form1.CurrentForm.AddLauncherLog("Notice: BnS Buddy does directly support Multiclient for this server!");
                            Form1.CurrentForm.WindowState = FormWindowState.Minimized;
                        }
                    }
                    catch
                    {
                        Form1.CurrentForm.Show();
                        Form1.CurrentForm.PlayButton.Enabled = true;
                        Form1.CurrentForm.AddLauncherLog($"[Error] Could Not Start {Path.GetFileName(PathToExe)}!");
                    }
                }
                if (Started && Form1.CurrentForm.materialSwitch52.Checked)
                {
                    Form1.CurrentForm.PlayButton.Text = "Kill Game";
                    if (Form1.CurrentForm.materialLabel33.Text == "Active")
                        Form1.CurrentForm.materialButton2.Enabled = true;
                    Form1.CurrentForm.MemoryCleaner.CleanMem();
                }
            }
            else
                Form1.CurrentForm.AddLauncherLog($"[Error]: Path to {Path.GetFileName(PathToExe)} not found!");
        }

        public void PlayGame(bool pressed = false, string username = "", string password = "", string region = "")
        {
            // Start Captured Clients
            if (Form1.CurrentForm.ActiveSessions.CapturedClients.Count > 0)
                StartCapturedClients();
            else
            {
                int count = Form1.CurrentForm.ActiveSessions.GetActiveClientCount();
                foreach (string users in Form1.CurrentForm.ActiveSessions.DiedClients)
                    Form1.CurrentForm.AddLauncherLog(users + "'s Game Process Died");
                if (count != 0 && pressed)
                    KillGame(gamesessionid);
                else
                    LaunchGame(username, password, region);
            }
        }

    }
}
