namespace Revamped_BnS_Buddy.DX12Mod
{
    partial class CEGui
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CEGui));
            this.fastColoredTextBox2 = new FastColoredTextBoxNS.FastColoredTextBox();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.Themer = new MetroFramework.Components.MetroStyleManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Themer)).BeginInit();
            this.SuspendLayout();
            // 
            // fastColoredTextBox2
            // 
            this.fastColoredTextBox2.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fastColoredTextBox2.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.fastColoredTextBox2.BackBrush = null;
            this.fastColoredTextBox2.BackColor = System.Drawing.Color.Transparent;
            this.fastColoredTextBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.fastColoredTextBox2.CaretColor = System.Drawing.Color.White;
            this.fastColoredTextBox2.CharHeight = 14;
            this.fastColoredTextBox2.CharWidth = 8;
            this.fastColoredTextBox2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fastColoredTextBox2.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fastColoredTextBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastColoredTextBox2.ForeColor = System.Drawing.SystemColors.Control;
            this.fastColoredTextBox2.IsReplaceMode = false;
            this.fastColoredTextBox2.Location = new System.Drawing.Point(20, 30);
            this.fastColoredTextBox2.Name = "fastColoredTextBox2";
            this.fastColoredTextBox2.Paddings = new System.Windows.Forms.Padding(0);
            this.fastColoredTextBox2.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fastColoredTextBox2.ServiceColors = null;
            this.fastColoredTextBox2.Size = new System.Drawing.Size(760, 400);
            this.fastColoredTextBox2.TabIndex = 3;
            this.fastColoredTextBox2.TabStop = false;
            this.fastColoredTextBox2.TextAreaBorderColor = System.Drawing.Color.White;
            this.fastColoredTextBox2.Zoom = 100;
            // 
            // metroButton1
            // 
            this.metroButton1.FontWeight = MetroFramework.MetroButtonWeight.Regular;
            this.metroButton1.Highlight = true;
            this.metroButton1.Location = new System.Drawing.Point(20, 8);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(95, 20);
            this.metroButton1.TabIndex = 5;
            this.metroButton1.Text = "Save";
            this.metroButton1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroButton1.UseSelectable = true;
            this.metroButton1.UseStyleColors = true;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // Themer
            // 
            this.Themer.Owner = this;
            this.Themer.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CEGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.metroButton1);
            this.Controls.Add(this.fastColoredTextBox2);
            this.DisplayHeader = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CEGui";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Config Editor";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Themer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox2;
        private MetroFramework.Controls.MetroButton metroButton1;
        private MetroFramework.Components.MetroStyleManager Themer;
    }
}