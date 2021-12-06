using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BnSBuddy2.ProfileManager
{
    public static class ProfileHandler
    {
        private static functions pm = new functions();
        public static Dictionary<string, string> CurrentList = new Dictionary<string, string>();

        // Open ProfileManager
        public static void OpenProfileManager()
        {
            if (Form1.CurrentForm.Profiles == null)
                Form1.CurrentForm.Profiles = new Pages.Profiles();
            Form1.CurrentForm.Profiles.ShowDialog();
        }

        // Open AddGui
        public static void OpenAddGui()
        {
            AddGui AddGui = new AddGui();
            AddGui.ShowDialog();
            AddGui.Dispose();
        }

        // Save entered user
        public static bool SaveUser(string email, string pass, string region)
        {
            if (email.Length > 0 && pass.Length > 0)
                if (pm.Add(email, pass, region))
                    return true;
                else
                    return false;
            else
                return false;
        }

        // Populate ADDGui
        public static object[] PopulateAddGui() { 
            return Form1.CurrentForm.materialComboBox2.Items.Cast<Object>().ToArray();
        }

        // Ini reg
        public static void IniReg()
        {
            RegistryKey localMachine = Registry.LocalMachine;
            if (localMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\") == null)
                Registry.LocalMachine.CreateSubKey("SOFTWARE\\BnS Buddy");
            pm = new functions();
        }

        // Convert legacy user
        public static void Convert(string SelectedProfile)
        {
            string md5 = "";
            foreach (string key in CurrentList.Keys)
                if (key == SelectedProfile)
                    md5 = CurrentList[key];
            Dictionary<string, List<string>> UserInfo = pm.GetInfo(md5);
            string username = UserInfo[md5][0];
            string password = UserInfo[md5][1];
            pm.Remove(md5);
            pm.Add(username, password, SelectedProfile);
        }

        // Remove selected user
        public static void Remove(string SelectedProfile)
        {
            string tmp = SelectedProfile;
            pm.Remove(CurrentList[tmp]);
        }

        // Populate Tree
        public static List<string> Populate(string region)
        {
            List<string> ToAdd = new List<string>();
            CurrentList = new Dictionary<string, string>();
            ToAdd.AddRange(PopulateLegacy(pm.GetLegacyUsers()));
            ToAdd.AddRange(PopulateList(pm.GetUserListByRegion(region)));
            return ToAdd;
        }

        private static List<string> PopulateList(Dictionary<string, List<string>> UserList)
        {
            List<string> ToAdd = new List<string>();
            foreach (string key in UserList.Keys)
            {
                ToAdd.Add(UserList[key][0]);
                CurrentList.Add(UserList[key][0], key);
            }
            return ToAdd;
        }

        private static List<string> PopulateLegacy(Dictionary<string, List<string>> UserList)
        {
            List<string> ToAdd = new List<string>();
            foreach (string key in UserList.Keys)
            {
                ToAdd.Add(UserList[key][0] + " (UNMANAGED)");
                CurrentList.Add(UserList[key][0] + " (UNMANAGED)", key);
            }
            return ToAdd;
        }

    }
}
