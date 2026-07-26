using System;
using System.Windows.Forms;

namespace BnSBuddy2
{
    public partial class Loading : Form
    {
        private bool fadingOut = true; // Tracks if we're fading out or in
        private double opacityStep = 0.05; // Adjust this for faster/slower fading

        public Loading()
        {
            InitializeComponent();
            TopMost = true;
            Refresh();
            fadeTimer.Stop();
            fadeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (fadingOut)
                {
                    // Fade out towards 50% opacity
                    if (this.Opacity > 0.3)
                    {
                        this.Opacity -= opacityStep;
                    }
                    else
                    {
                        fadingOut = false; // Switch to fading in
                    }
                }
                else
                {
                    // Fade in towards 100% opacity
                    if (this.Opacity < 1.0)
                    {
                        this.Opacity += opacityStep;
                    }
                    else
                    {
                        fadingOut = true; // Switch back to fading out
                    }
                }
                // Ensure opacity stays within valid bounds
                this.Opacity = Math.Max(0.0, Math.Min(1.0, this.Opacity));
                Refresh();
            }
            catch (Exception ex) { }
        }

        private void Loading_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Continue closing
            if (e.CloseReason == CloseReason.UserClosing && !Form1.CurrentForm.AppStarted)
                Form1.CurrentForm.KillApp();
        }
    }
}
