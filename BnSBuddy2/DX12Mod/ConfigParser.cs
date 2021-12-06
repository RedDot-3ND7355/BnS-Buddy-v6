using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revamped_BnS_Buddy.Functions;

namespace Revamped_BnS_Buddy.DX12Mod
{
    public class ConfigParser
    {
        private Dictionary<string, string> CurrSett = new Dictionary<string, string>();

        public void GetCurrentConfigurationFile(string FilePath = "")
        {
            if (FilePath != "")
            {
                if (File.Exists(FilePath))
                {
                    string[] tmp = File.ReadAllLines(FilePath);
                    foreach (string line in tmp)
                    {
                        if (line != null)
                        {
                            if (line.Contains("="))
                            {
                                string _tmp = line.Substring(0, line.IndexOf("="));
                                if (!CurrSett.ContainsKey(_tmp))
                                    CurrSett.Add(_tmp, line.Substring(line.IndexOf("=") + 1));
                            }
                        }
                    }
                }
            }
        }

        public void ReplaceConfigurationFile(string FilePath = "")
        {
            if (FilePath != "")
            {
                if (File.Exists(FilePath))
                {
                    string[] tmp = File.ReadAllLines(FilePath);
                    int POS = 0;
                    foreach(string line in tmp)
                    {
                        foreach (string setting in CurrSett.Keys)
                        {
                            if (line.Contains(setting))
                            {
                                tmp[POS] = setting + "=" + CurrSett[setting];
                            }
                        }
                        POS++;
                    }
                    File.WriteAllLines(FilePath, tmp);
                }
            }
        }

    }
}
