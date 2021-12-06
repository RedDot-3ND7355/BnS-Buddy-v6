using System;
using MaterialSkin;
using BnSBuddy2.Settings;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMI_ProcessorInformation;
using BnSBuddy2.Functions;

namespace BnSBuddy2.Pages
{
    public partial class Affinity : MaterialSkin.Controls.MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public Affinity()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Load CPU Info and Set cores in tree
            Routine();
        }

        private void Routine()
        {
            Form1.CurrentForm.materialLabel88.Text = "Loading...";
            LoadingState(true);
            Task.Delay(1000).ContinueWith(delegate
            {
                SetProcessor_Info();
            });
            SetTree(Form1.CurrentForm.cpuCount);
        }

        private void LoadingState(bool v, int value = 0)
        {
            materialProgressBar1.Visible = v;
            materialProgressBar1.Value += value;
            this.Enabled = !v;
        }

        private void SetProcessor_Info()
        {
            name.Text += WMI_Processor_Information.GetCpuName();
            LoadingState(true, (int)1);
            core.Text += WMI_Processor_Information.GetCpuCores();
            LoadingState(true, (int)1);
            comp.Text += WMI_Processor_Information.GetCpuManufacturer();
            LoadingState(true, (int)1);
            freq.Text += WMI_Processor_Information.GetCpuClockSpeed() + " Mhz";
            LoadingState(true, (int)1);
            arch.Text += WMI_Processor_Information.GetCpuCaption();
            LoadingState(true, (int)1);
            lcore.Text += Form1.CurrentForm.cpuCount;
            LoadingState(true, (int)1);
            LoadingState(false);
            Form1.CurrentForm.materialLabel88.Text = "Loaded. Waiting...";
        }

        private void SetTree(int v)
        {
            for (int i = 0; i < v; i++)
                treeView1.Nodes.Add(new TreeNode("Core # " + i.ToString()));
            treeView1.Refresh();
        }

        bool startup = true;
        private void Affinity_Load(object sender, EventArgs e)
        {
            string text8 = Form1.CurrentForm.routine.CurrentSettings["affinityproc"];
            if (text8.Length == 0)
                materialComboBox1.SelectedIndex = 0;
            else
                materialComboBox1.SelectedIndex = materialComboBox1.FindString(text8);
            if (text8 == "Custom")
                for (var i = 0; i < treeView1.Nodes.Count; i++)
                    treeView1.Nodes[i].Checked = (int.Parse(Form1.CurrentForm.routine.CurrentSettings["customaffinity"]) & (1 << i)) > 0;
            startup = false;
        }

        int cores_selected = 0;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!startup)
                try
                {
                    cores_selected = 0;
                    for (int i = 0; i < treeView1.Nodes.Count; i++)
                        if (treeView1.Nodes[i].Checked)
                            cores_selected += 1 << i;
                    Form1.CurrentForm.routineChanger("affinityproc", "Custom");
                    Form1.CurrentForm.customValue = cores_selected;
                    Form1.CurrentForm.routineChanger("customaffinity", cores_selected.ToString());
                }
                catch (Exception a) { Prompt.Popup("Error:" + a.Message); }
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedItem.ToString() != "Custom")
                treeView1.Enabled = false;
            else
                treeView1.Enabled = true;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedItem.ToString() == null)
                materialComboBox1.SelectedIndex = 0;
            this.DialogResult = DialogResult.OK;
            Form1.CurrentForm.routineChanger("affinityproc", materialComboBox1.SelectedItem.ToString());
            Form1.CurrentForm.materialLabel88.Text = "Affinity: " + materialComboBox1.SelectedItem.ToString();
            this.Close();
        }
    }
}
