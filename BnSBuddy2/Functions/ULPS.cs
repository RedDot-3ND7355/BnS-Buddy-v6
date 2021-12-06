﻿using Microsoft.Win32;

namespace BnSBuddy2.Functions
{
    public class ULPS
    {
        // Check if AMD is detected and ULPS is on or not
        public void CheckULPS()
        {
            string[] wheretocheck = new string[4] { "SYSTEM\\ControlSet001\\Control\\Class\\", "SYSTEM\\ControlSet001\\Control\\Video\\", "SYSTEM\\CurrentControlSet\\Control\\Video\\", "SYSTEM\\CurrentControlSet\\Control\\Class\\" };
            foreach (string reg_path_to in wheretocheck)
            {
                try
                {
                    RegistryKey localMachine = Registry.LocalMachine;
                    localMachine = localMachine.OpenSubKey(reg_path_to, writable: true);
                    if (localMachine != null)
                        foreach (string HWID in localMachine.GetSubKeyNames())
                        {
                            RegistryKey tmpKey = Registry.LocalMachine.OpenSubKey(reg_path_to + HWID + "\\", writable: true);
                            if (tmpKey != null)
                            {
                                foreach (string PROFILE in tmpKey.GetSubKeyNames())
                                {
                                    if (PROFILE.Length == 4 && IsAllDigits(PROFILE))
                                    {
                                        RegistryKey tmpKey2 = Registry.LocalMachine.OpenSubKey(reg_path_to + HWID + "\\" + PROFILE + "\\" + "UMD", writable: true);
                                        if (tmpKey2 != null)
                                            if (Registry.LocalMachine.OpenSubKey(reg_path_to + HWID + "\\" + PROFILE + "\\", writable: true).GetValue("EnableUlps") != null)
                                            {
                                                Form1.CurrentForm.materialLabel90.Visible = false;
                                                if (Registry.LocalMachine.OpenSubKey(reg_path_to + HWID + "\\" + PROFILE + "\\", writable: true).GetValue("EnableUlps").ToString() == "1")
                                                    Form1.CurrentForm.materialSwitch3.Checked = true;
                                            }
                                    }
                                }
                            }
                        }
                }
                catch { }
            }
        }

        // Verify Digits
        private static bool IsAllDigits(string s)
        {
            foreach (char c in s)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }

        // Enable & Disable Toggle Handler
        public void ULPSHandler(bool enable)
        {
            try
            {
                string[] wheretocheck = new string[4] { "SYSTEM\\ControlSet001\\Control\\Class\\", "SYSTEM\\ControlSet001\\Control\\Video\\", "SYSTEM\\CurrentControlSet\\Control\\Video\\", "SYSTEM\\CurrentControlSet\\Control\\Class\\" };
                // Find valid Class Path
                foreach (string reg_path_to in wheretocheck)
                {
                    RegistryKey localMachine = Registry.LocalMachine.OpenSubKey(reg_path_to, writable: true);
                    if (localMachine != null)
                        foreach (string HWID in localMachine.GetSubKeyNames())
                        {
                            RegistryKey tmpKey = Registry.LocalMachine.OpenSubKey(reg_path_to + HWID + "\\", writable: true);
                            if (tmpKey != null)
                                foreach (string PROFILE in tmpKey.GetSubKeyNames())
                                    if (PROFILE.Length == 4 && IsAllDigits(PROFILE))
                                    {
                                        RegistryKey tmpKey2 = Registry.LocalMachine.OpenSubKey(reg_path_to + HWID + "\\" + PROFILE + "\\" + "UMD", writable: true);
                                        if (tmpKey2 != null)
                                            Registry.LocalMachine.OpenSubKey(reg_path_to + HWID + "\\" + PROFILE + "\\", writable: true).SetValue("EnableUlps", enable ? "1" : "0", RegistryValueKind.DWord);
                                    }
                        }
                }
                Prompt.Popup("A reboot of your computer may be required for this to take effect.");
            }
            catch { Prompt.Popup($"Error: Could not toggle ULPS to {enable}"); }
        }
    }
}
