using BnSBuddy2.Functions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace BnSBuddy2.ProfileManager
{
    public class functions
    {
        string SALT = Form1.CurrentForm.SALT;
        ConfirmDialog dialog;

        public bool Add(string username, string password, string region)
        {
            bool adding = true;
            string unc_username = username;
            string unc_region = region;
            username = Enc(username);
            password = Enc(password);
            region = Enc(region);
            string md5hash = "";
            using (MD5 md5 = MD5.Create())
                md5hash = StringToHex(Encoding.UTF8.GetString(md5.ComputeHash(Encoding.UTF8.GetBytes(unc_username + unc_region))));
            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\" + md5hash, writable: true) != null)
            {
                dialog = new ConfirmDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Registry.LocalMachine.DeleteSubKey("SOFTWARE\\BnS Buddy\\" + md5hash);
                    adding = true;
                }
                else
                    adding = false;
            }
            if (adding)
            {
                RegistryKey localMachine = Registry.LocalMachine.CreateSubKey("SOFTWARE\\BnS Buddy\\" + md5hash, true);
                localMachine.SetValue("username", username);
                localMachine.SetValue("password", password);
                localMachine.SetValue("region", region);
                return true;
            }
            return false;
        }

        public string GetMD5(string username, string region)
        {
            using (MD5 md5 = MD5.Create())
                return StringToHex(Encoding.UTF8.GetString(md5.ComputeHash(Encoding.UTF8.GetBytes(username + region))));
        }

        private string StringToHex(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char value in s)
                stringBuilder.Append(Convert.ToInt32(value).ToString("x"));
            return stringBuilder.ToString();
        }


        public void Remove(string md5)
        {
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\", writable: true);
            localMachine.DeleteSubKey(md5);
        }

        private void Clear()
        {
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\", writable: true);
            string[] subKeyNames = localMachine.GetSubKeyNames();
            foreach (string text in subKeyNames)
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\BnS Buddy", writable: true).DeleteSubKeyTree(text.ToString());
            if (localMachine.GetValue("lastused") != null)
                localMachine.DeleteValue("lastused");
        }

        private string Enc(string s)
        {
            s += SALT;
            s = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(s), "Basic Enc"));
            return s;
        }

        private string Dec(string s)
        {
            try
            {
                s = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(s), "Basic Enc"));
                if (s.Contains(SALT))
                    s = s.Replace(SALT, "");
                else
                {
                    s = "";
                    Clear();
                    Prompt.Popup("Did you changed hardware? Because I don't recognize you.");
                }
                return s;
            }
            catch (CryptographicException)
            {
                ConfirmReset confirmReset = new ConfirmReset();
                if (confirmReset.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    Clear();
                return "";
            }
        }

        // User formatted credentials from registry
        //
        // Dictionary
        //  L UserHash [Key]
        //      |- Username [List 0]
        //      |- Password [List 1]
        //      L- Region   [List 2]
        //

        public Dictionary<string, List<string>> GetInfo(string md5)
        {
            Dictionary<string, List<string>> User = new Dictionary<string, List<string>>();
            RegistryKey tmpkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\" + md5, writable: true);
            if (tmpkey != null)
            {
                List<string> info = new List<string>();
                if (tmpkey.GetValue("username") != null)
                    info.Add(Dec(tmpkey.GetValue("username").ToString()));
                if (tmpkey.GetValue("password") != null)
                    info.Add(Dec(tmpkey.GetValue("password").ToString()));
                if (tmpkey.GetValue("region") != null)
                    info.Add(Dec(tmpkey.GetValue("region").ToString()));
                if (info.Count == 3 || info.Count == 2)
                    User.Add(md5, info);
            }
            return User;
        }

        public Dictionary<string, List<string>> GetLegacyUsers()
        {
            Dictionary<string, List<string>> UserList = new Dictionary<string, List<string>>();
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\", writable: true);
            string[] AccountHashes = localMachine.GetSubKeyNames();
            foreach (string md5 in AccountHashes)
            {
                localMachine = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\" + md5, writable: true);
                if (localMachine != null && localMachine.GetValue("region") == null)
                {
                    List<string> info = new List<string>();
                    if (localMachine.GetValue("username") != null)
                        info.Add(Dec(localMachine.GetValue("username").ToString()));
                    if (localMachine.GetValue("password") != null)
                        info.Add(Dec(localMachine.GetValue("password").ToString()));
                    if (info.Count == 2)
                        UserList.Add(md5, info);
                }
            }
            return UserList;
        }

        public Dictionary<string, List<string>> GetUserListByRegion(string region)
        {
            Dictionary<string, List<string>> UserList = new Dictionary<string, List<string>>();
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\", writable: true);
            string[] AccountHashes = localMachine.GetSubKeyNames();
            foreach (string md5 in AccountHashes)
            {
                RegistryKey tmpkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\" + md5, writable: true);
                if (tmpkey != null && tmpkey.GetValue("region") != null)
                {
                    List<string> info = new List<string>();
                    if (tmpkey.GetValue("username") != null)
                        info.Add(Dec(tmpkey.GetValue("username").ToString()));
                    if (tmpkey.GetValue("password") != null)
                        info.Add(Dec(tmpkey.GetValue("password").ToString()));
                    if (tmpkey.GetValue("region") != null)
                        info.Add(Dec(tmpkey.GetValue("region").ToString()));
                    if (info.Count == 3 && info[2] == region)
                        UserList.Add(md5, info);
                }
            }
            return UserList;
        }
    }
}
