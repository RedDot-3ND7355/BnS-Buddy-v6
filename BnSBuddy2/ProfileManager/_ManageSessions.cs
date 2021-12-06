using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using _Process = System.Diagnostics.Process;

namespace Revamped_BnS_Buddy.ProfileManager
{
    public partial class ManageSessions : MetroForm
    {
        private Dictionary<string, int> Identifyer = new Dictionary<string, int>();
        private ConfirmKillAll confirmDialog;
        private NewSession newSession;

        public ManageSessions()
        {
            InitializeComponent();
            GetColor();
            PopulateUnknown();
            PopulateActive();
        }

        // Refresh process list
        private void _Refresh()
        {
            listBox1.Items.Clear();
            Identifyer = new Dictionary<string, int>();
            PopulateUnknown();
            PopulateActive();
        }

        // Show Unknown game processes
        private void PopulateUnknown()
        {
            var UnknownSessions = Form1.CurrentForm.ActiveSessions.UnhandledClients;
            foreach (string UnkID in UnknownSessions.Keys)
            {
                string pid = metroToggle1.Checked ? " | PID: " + UnknownSessions[UnkID] : "";
                listBox1.Items.Add(UnkID + pid);
                Identifyer.Add(UnkID + pid, UnknownSessions[UnkID]);
            }
        }

        // Show Started game processes
        private void PopulateActive()
        {
            var ActiveSessions = Form1.CurrentForm.ActiveSessions.ActiveClients;
            foreach (string region in ActiveSessions.Keys)
            {
                foreach (string email in ActiveSessions[region].Keys)
                {
                    string pid = metroToggle1.Checked ? " | PID: " + ActiveSessions[region][email] : "";
                    listBox1.Items.Add($"[{region}] {email}" + pid);
                    Identifyer.Add($"[{region}] {email}" + pid, ActiveSessions[region][email]);
                }
            }
        }

        private void GetColor()
        {
            Themer.Style = Prompt.ColorSet;
            base.Style = Themer.Style;
            Refresh();
        }

        // Show PID Toggle
        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            _Refresh();
        }

        // Kill All
        private void metroButton2_Click(object sender, EventArgs e)
        {
            confirmDialog = new ConfirmKillAll();
            if (confirmDialog.ShowDialog() == DialogResult.OK)
                foreach (string name in Identifyer.Keys)
                {
                    confirmDialog = new ConfirmKillAll();
                    if (confirmDialog.ShowDialog() == DialogResult.OK)
                        if (Form1.CurrentForm.ActiveSessions.Verify(Identifyer[name]))
                            _Process.GetProcessById(Identifyer[name]).Kill();
                }
            if (listBox1.SelectedIndex >= 0)
                listBox1.ClearSelected();
            Form1.CurrentForm.ActiveSessions.ClearInactiveClients();
            _Refresh();
        }

        // Help
        private void metroButton4_Click(object sender, EventArgs e)
        {
            Prompt.Popup("How to use:" + Environment.NewLine + "-Left Click to select" + Environment.NewLine + "-Right click to deselect" + Environment.NewLine + "-CTRL + Left Click | SHIFT + Left Click to multiselect" + Environment.NewLine + "-Select action at the top");
        }

        // Deselect
        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Right")
            {
                if (listBox1.SelectedIndex >= 0)
                    listBox1.ClearSelected();
            }
        }

        // Kill Selected
        private void metroButton1_Click(object sender, EventArgs e)
        {
            List<string> ToRemove = new List<string>();
            if (listBox1.SelectedItems.Count > 0)
                foreach (string selected in listBox1.SelectedItems.Cast<Object>().ToList())
                    if (Form1.CurrentForm.ActiveSessions.Verify(Identifyer[selected]))
                    {
                        ToRemove.Add(selected);
                        _Process.GetProcessById(Identifyer[selected]).Kill();
                    }
                    else
                        ToRemove.Add(selected);
            foreach (string session in ToRemove)
                listBox1.Items.Remove(session);
            Form1.CurrentForm.ActiveSessions.ClearInactiveClients();
        }

        // New Session
        private void metroButton3_Click(object sender, EventArgs e)
        {
            newSession = new NewSession();
            if (newSession.ShowDialog() == DialogResult.OK)
            {
                listBox1.Enabled = false;
                _Refresh();
                listBox1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<string> ToRemove = new List<string>();
            if (listBox1.Items.Count > 0)
            {
                foreach (string session in listBox1.Items)
                    if (!Form1.CurrentForm.ActiveSessions.Verify(Identifyer[session]))
                        ToRemove.Add(session);
                if (ToRemove.Count > 0)
                    if (listBox1.SelectedIndex >= 0)
                        listBox1.ClearSelected();
                foreach (string item in ToRemove)
                    listBox1.Items.Remove(item);
            }
        }
    }
}
