using MetroFramework.Forms;
using Process.NET;
using Process.NET.Memory;
using Process.NET.Modules;
using Process.NET.Native.Types;
using Process.NET.Patterns;
using Process.NET.Threads;
using Process.NET.Windows;
using Revamped_BnS_Buddy.Functions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace Revamped_BnS_Buddy.QoLToggles
{
    public partial class QoLTable : MetroForm
    {
        public static ProcessSharp ProcessSharp { get; set; }
        Dictionary<string, Dictionary<string, int>> pairs = Form1.CurrentForm.ActiveSessions.ActiveClients;
        Dictionary<string, int> pairs2 = Form1.CurrentForm.ActiveSessions.UnhandledClients;
        Dictionary<string, int> Identify = new Dictionary<string, int>();
        private XmlDocument xmldoc = new XmlDocument();

        public interface IProcess : IDisposable
        {
            System.Diagnostics.Process Native { get; }
            SafeMemoryHandle Handle { get; }
            IMemory Memory { get; }
            IThreadFactory ThreadFactory { get; }
            IModuleFactory ModuleFactory { get; }
            IMemoryFactory MemoryFactory { get; }
            IWindowFactory WindowFactory { get; }
            IProcessModule this[string moduleName] { get; }
            IPointer this[IntPtr addr] { get; }
        }

        public QoLTable()
        {
            InitializeComponent();
            GetColor();
            PopulateComboBox();
            StoredValues();
            Task.Delay(0).ContinueWith(delegate { CheckForIllegalCrossThreadCalls = false; FetchPatches(); });
        }

        private void StoredValues()
        {
            // fetch(not store) past toggles per client process id
        }

        private Dictionary<string, Dictionary<string, List<string>>> Cpairs = new Dictionary<string, Dictionary<string, List<string>>>();

        private void FetchPatches()
        {
            UpdateStatusLabel("Reading Patches.xml");
            xmldoc = Form1.CurrentForm.fetch.PatchSet;
            string which = Form1.CurrentForm.fetch.OnlinePatches ? "Online" : "Offline";
            UpdateStatusLabel($"Using {which} patches. Reading...");
            foreach (XmlNode node in xmldoc.DocumentElement.GetElementsByTagName("patch"))
            {
                // Ini
                Dictionary<string, List<string>> valuePairs = new Dictionary<string, List<string>>();
                List<string> vs = new List<string>();
                // Add values in list (0 = on | 1 = off | 2 = pattern | 3 = length)
                if (node.ChildNodes[0].Name == "UsesBit" && node.ChildNodes[0].InnerText == "True")
                {
                    // 32bit
                    foreach (XmlNode Cnode in node.ChildNodes[1].ChildNodes)
                    {
                        vs.Add(Cnode.InnerText);
                    }
                    valuePairs.Add("bit32", vs);
                    // 64bit
                    vs = new List<string>();
                    foreach (XmlNode Cnode in node.ChildNodes[2].ChildNodes)
                    {
                        vs.Add(Cnode.InnerText);
                    }
                    valuePairs.Add("bit64", vs);
                }
                else
                {
                    foreach (XmlNode Cnode in node.ChildNodes)
                    {
                        vs.Add(Cnode.InnerText);
                    }
                    valuePairs.Add("bit", vs);
                }
                Cpairs.Add(node.Attributes["name"].Value, valuePairs);
            }
            UpdateStatusLabel("Waiting for selection");
        }

        private void GetColor()
        {
            base.Style = Prompt.ColorSet;
            Themer.Style = Prompt.ColorSet;
            Refresh();
        }

        string Bitness = "bit64";
        private void PopulateComboBox()
        {
            if (Form1.CurrentForm.metroComboBox4.SelectedItem.ToString() == "32bit")
            {
                Bitness = "bit32";
            }
            foreach (string region in pairs.Keys)
                foreach (string username in pairs[region].Keys)
                {
                    metroComboBox1.Items.Add($"[{region}] {username}");
                    Identify.Add($"[{region}] {username}", pairs[region][username]);
                }
            foreach (string pname in pairs2.Keys)
            {
                metroComboBox1.Items.Add($"[Unkn] {pname}");
                Identify.Add($"[Unkn] {pname}", pairs2[pname]);
            }
            metroComboBox1.SelectedIndex = 0;
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex != 0)
            {
                ProcessSharp = new ProcessSharp(Identify[metroComboBox1.SelectedItem.ToString()], MemoryType.Local);
                ProcessSharp.Memory = new ExternalProcessMemory(ProcessSharp.Handle);
                // Fetch pre-sets
                CheckPreSets();
                // Continue...
                UpdateStatusLabel("Ready for toggles.");
                CheckToggles(false);
            }
            else
            {
                CheckToggles(true);
            }
        }

        private void CheckPreSets()
        {
            if (Form1.CurrentForm.iDS.pairs.ContainsKey(Identify[metroComboBox1.SelectedItem.ToString()]))
            {
                Ignore = true;
                metroToggle1.Checked = Form1.CurrentForm.iDS.pairs[Identify[metroComboBox1.SelectedItem.ToString()]][0].Checked;
                Ignore = true;
                metroToggle2.Checked = Form1.CurrentForm.iDS.pairs[Identify[metroComboBox1.SelectedItem.ToString()]][1].Checked;
                Ignore = true;
                metroToggle3.Checked = Form1.CurrentForm.iDS.pairs[Identify[metroComboBox1.SelectedItem.ToString()]][2].Checked;
                Ignore = true;
                metroToggle4.Checked = Form1.CurrentForm.iDS.pairs[Identify[metroComboBox1.SelectedItem.ToString()]][3].Checked;
                Ignore = true;
                metroToggle5.Checked = Form1.CurrentForm.iDS.pairs[Identify[metroComboBox1.SelectedItem.ToString()]][4].Checked;
            }
        }

        private void CheckToggles(bool disable = true)
        {
            metroToggle1.Enabled = !disable;
            metroToggle2.Enabled = !disable;
            metroToggle3.Enabled = !disable;
            metroToggle4.Enabled = !disable;
            metroToggle5.Enabled = !disable;
        }

        // Infinite WallClimb
        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            if (Ignore)
            {
                Ignore = false;
                return;
            }
            if (IsBusy)
            {
                Ignore = true;
                metroToggle1.Checked = !metroToggle1.Checked;
                return;
            }

            UpdateStatusLabel("Not ready! Sorry :C");
            Ignore = true;
            metroToggle1.Checked = false;
        }

        private void UpdateStatusLabel(string text)
        {
            StatusLabel.Text = text;
            StatusLabel.Refresh();
        }

        bool Ignore = false;
        bool IsBusy = false;

        private void UpdateIDToggles(MetroFramework.Controls.MetroToggle toggle)
        {
            if (!Form1.CurrentForm.iDS.pairs.ContainsKey(Identify[metroComboBox1.SelectedItem.ToString()]))
            {
                List<MetroFramework.Controls.MetroToggle> list = new List<MetroFramework.Controls.MetroToggle>();
                list.Add(metroToggle1);
                list.Add(metroToggle2);
                list.Add(metroToggle3);
                list.Add(metroToggle4);
                list.Add(metroToggle5);
                Form1.CurrentForm.iDS.pairs.Add(Identify[metroComboBox1.SelectedItem.ToString()], list);
            }
            else
            {
                Form1.CurrentForm.iDS.pairs.Remove(Identify[metroComboBox1.SelectedItem.ToString()]);
                List<MetroFramework.Controls.MetroToggle> list = new List<MetroFramework.Controls.MetroToggle>();
                list.Add(metroToggle1);
                list.Add(metroToggle2);
                list.Add(metroToggle3);
                list.Add(metroToggle4);
                list.Add(metroToggle5);
                Form1.CurrentForm.iDS.pairs.Add(Identify[metroComboBox1.SelectedItem.ToString()], list);
            }
        }

        // F5 in F8
        private void metroToggle3_CheckedChanged(object sender, EventArgs e)
        {
            if (Ignore)
            {
                Ignore = false;
                return;
            }
            if (IsBusy)
            {
                Ignore = true;
                metroToggle3.Checked = !metroToggle3.Checked;
                return;
            }

            IsBusy = true;
            Task.Delay(0).ContinueWith(delegate
            {
                CheckForIllegalCrossThreadCalls = false;
                string ON = Cpairs["f5inf8"]["bit"][1];
                string OFF = Cpairs["f5inf8"]["bit"][2];
                if (metroToggle3.Checked)
                {
                    UpdateStatusLabel("Scanning for Pattern...");
                    IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["f5inf8"]["bit"][3].Replace("[bool]", OFF));
                    var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                    var Result = scanner.Find(FuncOffsetPattern);
                    if (Result.Found)
                    {
                        UpdateStatusLabel("Found. Patching...");
                        byte[] managedArray = new byte[0];
                        managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["f5inf8"]["bit"][4]));
                        managedArray[3] = (byte)int.Parse(ON, System.Globalization.NumberStyles.HexNumber);
                        ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                        UpdateStatusLabel("Patched.");
                        //
                        UpdateIDToggles(metroToggle3);
                    }
                    else
                    {
                        Ignore = true;
                        metroToggle3.Checked = false;
                        UpdateStatusLabel("Pattern not found!");
                    }
                }
                else
                {
                    UpdateStatusLabel("Scanning for Pattern...");
                    IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["f5inf8"]["bit"][3].Replace("[bool]", ON));
                    var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                    var Result = scanner.Find(FuncOffsetPattern);
                    if (Result.Found)
                    {
                        UpdateStatusLabel("Found. Restoring...");
                        byte[] managedArray = new byte[0];
                        managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["f5inf8"]["bit"][4]));
                        managedArray[3] = (byte)int.Parse(OFF, System.Globalization.NumberStyles.HexNumber);
                        ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                        UpdateStatusLabel("Restored.");
                        // 
                        UpdateIDToggles(metroToggle3);
                    }
                    else
                    {
                        Ignore = true;
                        metroToggle3.Checked = true;
                        UpdateStatusLabel("Pattern not found!");
                    }
                    UpdateStatusLabel("Restored.");
                }
                IsBusy = false;
            });
        }

        // Normal to auto Bait
        private void metroToggle2_CheckedChanged(object sender, EventArgs e)
        {
            if (Ignore)
            {
                Ignore = false;
                return;
            }
            if (IsBusy)
            {
                Ignore = true;
                metroToggle2.Checked = !metroToggle2.Checked;
                return;
            }

            IsBusy = true;
            Task.Delay(0).ContinueWith(delegate
            {
                CheckForIllegalCrossThreadCalls = false;
                string ON = Cpairs["no2au"][Bitness][0];
                string[] num = ON.Split(' ');
                byte[] vON = new byte[num.Length];
                int i = 0;
                foreach (string hex in num)
                {
                    vON[i] = (byte)int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                    i++;
                }
                string OFF = Cpairs["no2au"][Bitness][1];
                num = OFF.Split(' ');
                byte[] vOFF = new byte[num.Length];
                i = 0;
                foreach (string hex in num)
                {
                    vOFF[i] = (byte)int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                    i++;
                }

                if (metroToggle2.Checked)
                {
                    UpdateStatusLabel("Scanning for Pattern...");
                    IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["no2au"][Bitness][2].Replace("[bool]", OFF));
                    var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                    var Result = scanner.Find(FuncOffsetPattern);
                    if (Result.Found)
                    {
                        UpdateStatusLabel("Found. Patching...");
                        byte[] managedArray = new byte[0];
                        managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["no2au"][Bitness][3]));
                        managedArray[0] = vON[0];
                        managedArray[1] = vON[1];
                        managedArray[2] = vON[2];
                        if (vON.Length == 4)
                        {
                            managedArray[3] = vON[3];
                        }
                        ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                        UpdateStatusLabel("Patched.");
                        UpdateIDToggles(metroToggle2);
                    }
                    else
                    {
                        Ignore = true;
                        metroToggle3.Checked = false;
                        UpdateStatusLabel("Pattern not found!");
                    }
                }
                else
                {
                    UpdateStatusLabel("Scanning for Pattern...");
                    IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["no2au"][Bitness][2].Replace("[bool]", ON));
                    var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                    var Result = scanner.Find(FuncOffsetPattern);
                    if (Result.Found)
                    {
                        UpdateStatusLabel("Found. Restoring...");
                        byte[] managedArray = new byte[0];
                        managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["no2au"][Bitness][3]));
                        managedArray[0] = vOFF[0];
                        managedArray[1] = vOFF[1];
                        managedArray[2] = vOFF[2];
                        if (vOFF.Length == 4)
                        {
                            managedArray[3] = vOFF[3];
                        }
                        ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                        UpdateStatusLabel("Restored.");
                        UpdateIDToggles(metroToggle2);
                    }
                    else
                    {
                        Ignore = true;
                        metroToggle3.Checked = true;
                        UpdateStatusLabel("Pattern not found!");
                    }
                }
                IsBusy = false;
            });
        }

        // Less loading
        private void metroToggle5_CheckedChanged(object sender, EventArgs e)
        {
            if (Ignore)
            {
                Ignore = false;
                return;
            }
            if (IsBusy)
            {
                Ignore = true;
                metroToggle5.Checked = !metroToggle5.Checked;
                return;
            }

            IsBusy = true;
            Task.Delay(0).ContinueWith(delegate
            {
                CheckForIllegalCrossThreadCalls = false;
                string ON = Cpairs["lessload"][Bitness][0];
                string[] num = ON.Split(' ');
                byte[] vON = new byte[num.Length];
                int i = 0;
                foreach (string hex in num)
                {
                    vON[i] = (byte)int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                    i++;
                }
                string OFF = Cpairs["lessload"][Bitness][1];
                num = OFF.Split(' ');
                byte[] vOFF = new byte[num.Length];
                i = 0;
                foreach (string hex in num)
                {
                    vOFF[i] = (byte)int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                    i++;
                }

                if (metroToggle5.Checked)
                {
                    UpdateStatusLabel("Scanning for Pattern...");
                    IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["lessload"][Bitness][2].Replace("[bool]", OFF));
                    var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                    var Result = scanner.Find(FuncOffsetPattern);
                    if (Result.Found)
                    {
                        UpdateStatusLabel("Found. Patching...");
                        byte[] managedArray = new byte[0];
                        managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["lessload"][Bitness][3]));
                        if (Bitness == "bit64")
                        {
                            managedArray[2] = vON[0];
                            managedArray[3] = vON[1];
                        }
                        else
                        {
                            managedArray[12] = vON[0];
                            managedArray[13] = vON[1];
                        }
                        ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                        UpdateStatusLabel("Patched.");
                        UpdateIDToggles(metroToggle5);
                    }
                    else
                    {
                        Ignore = true;
                        metroToggle5.Checked = false;
                        UpdateStatusLabel("Pattern not found!");
                    }
                }
                else
                {
                    UpdateStatusLabel("Scanning for Pattern...");
                    IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["lessload"][Bitness][2].Replace("[bool]", ON));
                    var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                    var Result = scanner.Find(FuncOffsetPattern);
                    if (Result.Found)
                    {
                        UpdateStatusLabel("Found. Restoring...");
                        byte[] managedArray = new byte[0];
                        managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["lessload"][Bitness][3]));
                        if (Bitness == "bit64")
                        {
                            managedArray[2] = vOFF[0];
                            managedArray[3] = vOFF[1];
                        }
                        else
                        {
                            managedArray[12] = vOFF[0];
                            managedArray[13] = vOFF[1];
                        }
                        ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                        UpdateStatusLabel("Restored.");
                        UpdateIDToggles(metroToggle5);
                    }
                    else
                    {
                        Ignore = true;
                        metroToggle5.Checked = true;
                        UpdateStatusLabel("Pattern not found!");
                    }
                }
                IsBusy = false;
            });
        }

        // Simple mode everywhere
        private void metroToggle4_CheckedChanged(object sender, EventArgs e)
        {
            if (Ignore)
            {
                Ignore = false;
                return;
            }
            if (IsBusy)
            {
                Ignore = true;
                metroToggle4.Checked = !metroToggle4.Checked;
                return;
            }

            // Disable for 64bit
            if (Bitness == "bit64")
            {
                UpdateStatusLabel("Not ready for 64bit! Sorry :C");
                Ignore = true;
                metroToggle4.Checked = false;
                return;
            }

            IsBusy = true;
            Task.Delay(0).ContinueWith(delegate
            {
                CheckForIllegalCrossThreadCalls = false;
                string ON = Cpairs["simp4evr"][Bitness][0];
                string OFF = Cpairs["simp4evr"][Bitness][1];

                if (metroToggle4.Checked)
                {
                    for (int io = 0; io < 2; io++)
                    {
                        UpdateStatusLabel("Scanning for Pattern #" + (io + 1) + "...");
                        IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["simp4evr"][Bitness][io + 2].Replace("[bool]", OFF));
                        var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                        var Result = scanner.Find(FuncOffsetPattern);
                        if (Result.Found)
                        {
                            UpdateStatusLabel("Found. Patching...");
                            byte[] managedArray = new byte[0];
                            managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["simp4evr"][Bitness][4]));
                            int nume = 0;
                            if (io == 0) { nume = 11; }
                            else if (io == 1) { nume = 15; }
                            managedArray[nume] = (byte)int.Parse(ON, System.Globalization.NumberStyles.HexNumber);
                            ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                            UpdateStatusLabel("Patched.");
                            UpdateIDToggles(metroToggle4);
                        }
                        else
                        {
                            Ignore = true;
                            metroToggle4.Checked = false;
                            Prompt.Popup("Pattern #" + (io + 1) + " not found!");
                            UpdateStatusLabel("Pattern #" + (io + 1) + " not found!");
                            break;
                        }
                    }
                }
                else
                {
                    for (int io = 0; io < 2; io++)
                    {
                        UpdateStatusLabel("Scanning for Pattern #" + (io + 1) + "...");
                        IMemoryPattern FuncOffsetPattern = new DwordPattern(Cpairs["simp4evr"][Bitness][io + 2].Replace("[bool]", ON));
                        var scanner = new PatternScanner(ProcessSharp.ModuleFactory.MainModule);
                        var Result = scanner.Find(FuncOffsetPattern);
                        if (Result.Found)
                        {
                            UpdateStatusLabel("Found. Restoring...");
                            byte[] managedArray = new byte[0];
                            managedArray = ProcessSharp.Memory.Read(Result.BaseAddress, int.Parse(Cpairs["simp4evr"][Bitness][4]));
                            int nume = 0;
                            if (io == 0) { nume = 11; }
                            else if (io == 1) { nume = 15; }
                            managedArray[nume] = (byte)int.Parse(OFF, System.Globalization.NumberStyles.HexNumber);
                            ProcessSharp.Memory.Write(Result.BaseAddress, managedArray);
                            UpdateStatusLabel("Restored.");
                            UpdateIDToggles(metroToggle4);
                        }
                        else
                        {
                            Ignore = true;
                            metroToggle4.Checked = true;
                            UpdateStatusLabel("Pattern #" + (io + 1) + " not found!");
                            return;
                        }
                    }
                }
                IsBusy = false;
            });
        }
    }
}
