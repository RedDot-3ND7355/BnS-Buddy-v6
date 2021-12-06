
namespace BnSBuddy2
{
    partial class Loading
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loading));
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.materialProgressBar1 = new MaterialSkin.Controls.MaterialProgressBar();
            this.progressbar = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // materialLabel1
            // 
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.FontType = MaterialSkin.MaterialSkinManager.fontType.H4;
            this.materialLabel1.HighEmphasis = true;
            this.materialLabel1.Location = new System.Drawing.Point(6, 24);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(294, 41);
            this.materialLabel1.TabIndex = 0;
            this.materialLabel1.Text = "Loading";
            this.materialLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // materialProgressBar1
            // 
            this.materialProgressBar1.Depth = 0;
            this.materialProgressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.materialProgressBar1.Location = new System.Drawing.Point(3, 69);
            this.materialProgressBar1.MarqueeAnimationSpeed = 10;
            this.materialProgressBar1.Maximum = 50;
            this.materialProgressBar1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialProgressBar1.Name = "materialProgressBar1";
            this.materialProgressBar1.Size = new System.Drawing.Size(300, 5);
            this.materialProgressBar1.Step = 1;
            this.materialProgressBar1.TabIndex = 1;
            // 
            // progressbar
            // 
            this.progressbar.Enabled = true;
            this.progressbar.Interval = 25;
            this.progressbar.Tick += new System.EventHandler(this.progressbar_Tick);
            // 
            // Loading
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(306, 77);
            this.Controls.Add(this.materialProgressBar1);
            this.Controls.Add(this.materialLabel1);
            this.FormStyle = MaterialSkin.Controls.MaterialForm.FormStyles.ActionBar_None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Loading";
            this.Padding = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.Sizable = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Loading_FormClosing);
            this.Shown += new System.EventHandler(this.Loading_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialProgressBar materialProgressBar1;
        private System.Windows.Forms.Timer progressbar;
    }
}