namespace Revamped_BnS_Buddy.PluginLoader
{
    partial class PLAddonManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PLAddonManager));
            this.Themer = new MetroFramework.Components.MetroStyleManager(this.components);
            this.metroButton2 = new MetroFramework.Controls.MetroButton();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroButton3 = new MetroFramework.Controls.MetroButton();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroListBox2 = new MetroFramework.CustomMetroControls.MetroListBox();
            this.metroListBox1 = new MetroFramework.CustomMetroControls.MetroListBox();
            ((System.ComponentModel.ISupportInitialize)(this.Themer)).BeginInit();
            this.SuspendLayout();
            // 
            // Themer
            // 
            this.Themer.Owner = this;
            this.Themer.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroButton2
            // 
            this.metroButton2.Location = new System.Drawing.Point(702, 30);
            this.metroButton2.Name = "metroButton2";
            this.metroButton2.Size = new System.Drawing.Size(75, 23);
            this.metroButton2.TabIndex = 27;
            this.metroButton2.Text = "Add";
            this.metroButton2.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton2.UseSelectable = true;
            this.metroButton2.UseStyleColors = true;
            this.metroButton2.Click += new System.EventHandler(this.metroButton2_Click);
            // 
            // metroButton1
            // 
            this.metroButton1.Location = new System.Drawing.Point(621, 30);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(75, 23);
            this.metroButton1.TabIndex = 28;
            this.metroButton1.Text = "Remove";
            this.metroButton1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton1.UseSelectable = true;
            this.metroButton1.UseStyleColors = true;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel1.Location = new System.Drawing.Point(20, 30);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(70, 25);
            this.metroLabel1.TabIndex = 29;
            this.metroLabel1.Text = "Addons";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel1.UseStyleColors = true;
            // 
            // metroButton3
            // 
            this.metroButton3.Location = new System.Drawing.Point(540, 30);
            this.metroButton3.Name = "metroButton3";
            this.metroButton3.Size = new System.Drawing.Size(75, 23);
            this.metroButton3.TabIndex = 30;
            this.metroButton3.Text = "Help";
            this.metroButton3.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton3.UseSelectable = true;
            this.metroButton3.UseStyleColors = true;
            this.metroButton3.Click += new System.EventHandler(this.metroButton3_Click);
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel7.Location = new System.Drawing.Point(556, 62);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(82, 25);
            this.metroLabel7.TabIndex = 31;
            this.metroLabel7.Text = "BnSPatch";
            this.metroLabel7.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel7.UseStyleColors = true;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel2.Location = new System.Drawing.Point(163, 62);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(94, 25);
            this.metroLabel2.TabIndex = 32;
            this.metroLabel2.Text = "BnS Buddy";
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel2.UseStyleColors = true;
            // 
            // metroListBox2
            // 
            this.metroListBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.metroListBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.metroListBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.metroListBox2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.metroListBox2.ForeColor = System.Drawing.SystemColors.Info;
            this.metroListBox2.FormattingEnabled = true;
            this.metroListBox2.ItemHeight = 21;
            this.metroListBox2.Location = new System.Drawing.Point(23, 90);
            this.metroListBox2.Name = "metroListBox2";
            this.metroListBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.metroListBox2.Size = new System.Drawing.Size(377, 337);
            this.metroListBox2.Sorted = true;
            this.metroListBox2.TabIndex = 26;
            this.metroListBox2.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroListBox2.UseSelectable = true;
            this.metroListBox2.UseStyleColors = true;
            this.metroListBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // metroListBox1
            // 
            this.metroListBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.metroListBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.metroListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.metroListBox1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.metroListBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.metroListBox1.FormattingEnabled = true;
            this.metroListBox1.ItemHeight = 21;
            this.metroListBox1.Location = new System.Drawing.Point(400, 90);
            this.metroListBox1.Name = "metroListBox1";
            this.metroListBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.metroListBox1.Size = new System.Drawing.Size(377, 337);
            this.metroListBox1.Sorted = true;
            this.metroListBox1.TabIndex = 25;
            this.metroListBox1.UseSelectable = true;
            this.metroListBox1.UseStyleColors = true;
            this.metroListBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // PLAddonManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel7);
            this.Controls.Add(this.metroButton3);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.metroButton1);
            this.Controls.Add(this.metroButton2);
            this.Controls.Add(this.metroListBox2);
            this.Controls.Add(this.metroListBox1);
            this.DisplayHeader = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "PLAddonManager";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "BnSPatch Addons Manager";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PLAddonManager_FormClosing);
            this.Shown += new System.EventHandler(this.PLAddonManager_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Themer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Components.MetroStyleManager Themer;
        public MetroFramework.CustomMetroControls.MetroListBox metroListBox2;
        public MetroFramework.CustomMetroControls.MetroListBox metroListBox1;
        private MetroFramework.Controls.MetroButton metroButton1;
        private MetroFramework.Controls.MetroButton metroButton2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton metroButton3;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel7;
    }
}