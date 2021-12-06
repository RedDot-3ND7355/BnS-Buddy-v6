using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace BnSBuddy2
{
    public partial class Loading : MaterialForm
    {
        public Loading()
        {
            InitializeComponent();
            Application.DoEvents();
        }

        private void progressbar_Tick(object sender, EventArgs e)
        {
            if (materialProgressBar1.Value == materialProgressBar1.Maximum)
                materialProgressBar1.Value = 0;
            materialProgressBar1.PerformStep();
        }

        private void Loading_Shown(object sender, EventArgs e) =>
            materialProgressBar1.Enabled = true;

        private void Loading_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !Form1.CurrentForm.AppStarted)
                Form1.CurrentForm.KillApp();
        }
    }
}
