using System.Collections.Generic;
using System.Diagnostics;

namespace BnSBuddy2.ProfileManager
{
    public class ActiveSessions
    {
        // Add started clients to ActiveClients
        public Dictionary<string, Dictionary<string, int>> ActiveClients = new Dictionary<string, Dictionary<string, int>>();
        //
        // Dictionary
        //  L Region [Key]
        //     Dictionary
        //      L Username [Key]
        //          L Process ID [int]
        //

        // Add new clients to CapturedClients
        public List<ProcessStartInfo> CapturedClients = new List<ProcessStartInfo>();
        //
        // List
        //  L Process Start Info [int]
        //

        // Add existing clients to UnhandledClients
        public Dictionary<string, int> UnhandledClients = new Dictionary<string, int>();
        //
        // Dictionary
        //  L Uknown ID [Key]
        //     L Process ID [int]
        //

        // Add Specified ActiveClient
        public bool Add(string region, string username, int PID)
        {
            region = RegionConvert(region);
            Dictionary<string, int> ActiveUsername = new Dictionary<string, int>();
            if (!ActiveClients.ContainsKey(region))
                ActiveClients.Add(region, ActiveUsername);
            ActiveUsername = ActiveClients[region];
            if (!ActiveUsername.ContainsKey(username))
            {
                ActiveUsername.Add(username, PID);
                ActiveClients[region] = ActiveUsername;
                return true;
            }
            else
                return false;
        }

        // Region convert
        private string RegionConvert(string region)
        {
            if (region == "North America" || region == "Europe")
                return "NA/EU";
            if (region == "Korean" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                return "Korean Test";
            return region;
        }

        // Remove Specified ActiveClient
        public void Remove(string region, string username)
        {
            region = RegionConvert(region);
            if (ActiveClients.ContainsKey(region) && ActiveClients[region].ContainsKey(username))
                ActiveClients[region].Remove(username);
        }

        // Terminate Specified PID
        public void TerminatePID(int PID)
        {
            if (Verify(PID))
                Process.GetProcessById(PID).Kill();
        }

        // Remove from ActiveClients if PID not found
        public bool Verify(int PID)
        {
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
                if (processes[i].Id == PID)
                    return true;
            return false;
        }

        public void ClearInactiveClients()
        {
            Dictionary<string, List<string>> ToRemove = new Dictionary<string, List<string>>();
            List<string> ToRemove2 = new List<string>();
            foreach (string region in ActiveClients.Keys)
            {
                List<string> usernames = new List<string>();
                foreach (string username in ActiveClients[region].Keys)
                    if (!Verify(ActiveClients[region][username]))
                        usernames.Add(username);
                if (usernames.Count > 0)
                    ToRemove.Add(region, usernames);
            }
            foreach (string unkn in UnhandledClients.Keys)
            {
                if (!Verify(UnhandledClients[unkn]))
                    ToRemove2.Add(unkn);
            }
            foreach (string region in ToRemove.Keys)
                foreach (string username in ToRemove[region])
                    ActiveClients[region].Remove(username);
            foreach (string unkn in ToRemove2)
                UnhandledClients.Remove(unkn);
        }

        // Get count of ActiveClient
        public int GetActiveClientCount()
        {
            DiedClients = new List<string>();
            int count = 0;
            foreach (string region in ActiveClients.Keys)
                foreach (string username in ActiveClients[region].Keys)
                    if (Verify(ActiveClients[region][username]))
                        count++;
                    else
                        DiedClients.Add($"[{region}] {username}");
            foreach (string unkn in UnhandledClients.Keys)
                if (Verify(UnhandledClients[unkn]))
                    count++;
                else
                    DiedClients.Add($"[UNK] {unkn}");
            ClearInactiveClients();
            return count;
        }

        public List<string> DiedClients = new List<string>();

        // Get PID based from region and username
        public int GetPID(string region, string username)
        {
            region = RegionConvert(region);
            if (ActiveClients.ContainsKey(region) && ActiveClients[region].ContainsKey(username))
                return ActiveClients[region][username];
            else
                return 0;
        }

        public void KillActiveProcesses(bool _override = false)
        {
            try
            {
                string customclientname = Form1.CurrentForm.materialTextBox5.Text;
                if (_override || Form1.CurrentForm.materialSwitch40.Checked)
                {
                    int killcount = 0;
                    Process[] processesByName = Process.GetProcessesByName("Client");
                    for (int i = 0; i < processesByName.Length; i++)
                    {
                        processesByName[i].Kill();
                        killcount++;
                    }
                    processesByName = Process.GetProcessesByName("BNSR");
                    for (int i = 0; i < processesByName.Length; i++)
                    {
                        processesByName[i].Kill();
                        Form1.CurrentForm.AddLauncherLog("Killed Game Process.");
                    }
                    if (customclientname != "")
                    {
                        processesByName = Process.GetProcessesByName(customclientname.Replace(".exe", ""));
                        for (int j = 0; j < processesByName.Length; j++)
                        {
                            processesByName[j].Kill();
                            Form1.CurrentForm.AddLauncherLog("Killed Game Process.");
                        }
                    }
                    if (killcount > 1)
                        Form1.CurrentForm.AddLauncherLog("Killed Game Processes.");
                    else if (killcount == 1)
                        Form1.CurrentForm.AddLauncherLog("Killed Game Process.");
                    Form1.CurrentForm.CalledKiller = true;
                }
            }
            catch
            {
                Form1.CurrentForm.AddLauncherLog("Could Not Kill Game Process.");
            }
        }
    }
}
