using System;
using System.Management;
using System.Text;

namespace Security
{
    public class FingerPrint
    {
        private static string fingerPrint = string.Empty;

        public static string Value()
        {
            fingerPrint = string.Empty;
            if (string.IsNullOrEmpty(fingerPrint))
            {
                string text = cpuId();
                string text2 = biosId();
                string text3 = baseId();
                string text4 = text + text2 + text3;
                text4 += text4.Replace(" ", "");
                fingerPrint = text4.Trim();
            }
            return StringToHex(fingerPrint);
        }

        private static string StringToHex(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char value in s)
            {
                stringBuilder.Append(Convert.ToInt32(value).ToString("x"));
            }
            return stringBuilder.ToString();
        }

        private static string identifier(string wmiClass, string wmiProperty)
        {
            string text = "";
            foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
            {
                if (text == "")
                {
                    try
                    {
                        if (instance[wmiProperty] == null)
                        {
                            return text;
                        }
                        text = instance[wmiProperty].ToString();
                        return text;
                    }
                    catch
                    {
                    }
                }
            }
            return text;
        }

        private static string cpuId()
        {
            try
            {
                string text = identifier("Win32_Processor", "UniqueId");
                if (text == "")
                {
                    text = identifier("Win32_Processor", "ProcessorId");
                    if (text == "")
                    {
                        text = identifier("Win32_Processor", "Name");
                        if (text == "")
                        {
                            text = identifier("Win32_Processor", "Manufacturer");
                        }
                    }
                }
                return text;
            } catch { return ""; }
        }

        private static string biosId()
        {
            try
            {
                return identifier("Win32_BIOS", "Manufacturer") + identifier("Win32_BIOS", "SMBIOSBIOSVersion") + identifier("Win32_BIOS", "IdentificationCode") + identifier("Win32_BIOS", "SerialNumber") + identifier("Win32_BIOS", "ReleaseDate") + identifier("Win32_BIOS", "Version");
            } catch { return ""; }
        }

        private static string baseId()
        {
            try
            {
                return identifier("Win32_BaseBoard", "Model") + identifier("Win32_BaseBoard", "Manufacturer") + identifier("Win32_BaseBoard", "Name") + identifier("Win32_BaseBoard", "SerialNumber");
            } catch { return ""; }
        }
    }
}
