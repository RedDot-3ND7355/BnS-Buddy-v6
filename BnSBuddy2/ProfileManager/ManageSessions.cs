using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BnSBuddy2.Functions;
using MaterialSkin;
using MaterialSkin.Controls;

namespace BnSBuddy2.ProfileManager
{
    public partial class ManageSessions : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        private Dictionary<string, int> Identifyer = new Dictionary<string, int>();
        private ConfirmKillAll confirmDialog;
        private NewSession newSession;
        // End Globals

        public ManageSessions()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Populate 
            PopulateUnknown();
            PopulateActive();
        }

        // Show Unknown game processes
        private void PopulateUnknown()
        {
            var UnknownSessions = Form1.CurrentForm.ActiveSessions.UnhandledClients;
            foreach (string UnkID in UnknownSessions.Keys)
            {
                string pid = materialSwitch1.Checked ? " | PID: " + UnknownSessions[UnkID] : "";
                materialListBox1.Items.Add(new MaterialListBoxItem(UnkID + pid));
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
                    string pid = materialSwitch1.Checked ? " | PID: " + ActiveSessions[region][email] : "";
                    materialListBox1.Items.Add(new MaterialListBoxItem($"[{region}] {email}" + pid));
                    Identifyer.Add($"[{region}] {email}" + pid, ActiveSessions[region][email]);
                }
            }
        }

        // Refresh process list
        private void _Refresh()
        {
            materialListBox1.Items.Clear();
            Identifyer = new Dictionary<string, int>();
            PopulateUnknown();
            PopulateActive();
        }

        // Show PID Toggle
        private void materialSwitch1_CheckedChanged(object sender, EventArgs e) =>
            _Refresh();

        // Help Button
        private void materialButton1_Click(object sender, EventArgs e) =>
            Prompt.Popup("How to use:" + Environment.NewLine + "-Left Click to select" + Environment.NewLine + "-Right click to deselect" + Environment.NewLine + "-CTRL + Left Click | SHIFT + Left Click to multiselect" + Environment.NewLine + "-Select action at the top");

        // New Instance Button
        private void materialButton2_Click(object sender, EventArgs e)
        {
            newSession = new NewSession();
            if (newSession.ShowDialog() == DialogResult.OK)
            {
                materialListBox1.Enabled = false;
                _Refresh();
                materialListBox1.Enabled = true;
            }
        }

        // Kill All Button
        private void materialButton3_Click(object sender, EventArgs e)
        {
            confirmDialog = new ConfirmKillAll();
            if (confirmDialog.ShowDialog() == DialogResult.OK)
                foreach (string name in Identifyer.Keys)
                {
                    confirmDialog = new ConfirmKillAll();
                    if (confirmDialog.ShowDialog() == DialogResult.OK)
                        if (Form1.CurrentForm.ActiveSessions.Verify(Identifyer[name]))
                            Process.GetProcessById(Identifyer[name]).Kill();
                }
            materialListBox1.Clear();
            Form1.CurrentForm.ActiveSessions.ClearInactiveClients();
            _Refresh();
        }

        // Kill Selected Button
        private void materialButton4_Click(object sender, EventArgs e)
        {
            List<MaterialListBoxItem> ToRemove = new List<MaterialListBoxItem>();
            if (materialListBox1.SelectedItems.Count > 0)
                foreach (MaterialListBoxItem selected in materialListBox1.SelectedItems.Cast<Object>().ToList())
                    if (Form1.CurrentForm.ActiveSessions.Verify(Identifyer[selected.Text]))
                    {
                        ToRemove.Add(selected);
                        Process.GetProcessById(Identifyer[selected.Text]).Kill();
                    }
                    else
                        ToRemove.Add(selected);
            foreach (MaterialListBoxItem session in ToRemove)
                materialListBox1.Items.Remove(session);
            Form1.CurrentForm.ActiveSessions.ClearInactiveClients();
        }
    }
}
