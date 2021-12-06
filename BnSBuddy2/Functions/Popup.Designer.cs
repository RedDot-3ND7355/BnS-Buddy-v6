
namespace BnSBuddy2.Functions
{
    partial class Popup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Popup));
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.materialMultiLineTextBox1 = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.SuspendLayout();
            // 
            // materialButton1
            // 
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense;
            this.materialButton1.Depth = 0;
            this.materialButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.materialButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = null;
            this.materialButton1.Location = new System.Drawing.Point(3, 109);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.Size = new System.Drawing.Size(350, 36);
            this.materialButton1.TabIndex = 1;
            this.materialButton1.Text = "Ok";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            // 
            // materialMultiLineTextBox1
            // 
            this.materialMultiLineTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialMultiLineTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.materialMultiLineTextBox1.Depth = 0;
            this.materialMultiLineTextBox1.DetectUrls = false;
            this.materialMultiLineTextBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialMultiLineTextBox1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialMultiLineTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialMultiLineTextBox1.Location = new System.Drawing.Point(3, 24);
            this.materialMultiLineTextBox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialMultiLineTextBox1.Name = "materialMultiLineTextBox1";
            this.materialMultiLineTextBox1.ReadOnly = true;
            this.materialMultiLineTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.materialMultiLineTextBox1.Size = new System.Drawing.Size(350, 80);
            this.materialMultiLineTextBox1.TabIndex = 2;
            this.materialMultiLineTextBox1.TabStop = false;
            this.materialMultiLineTextBox1.Text = "";
            this.materialMultiLineTextBox1.WordWrap = false;
            // 
            // Popup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(356, 148);
            this.Controls.Add(this.materialMultiLineTextBox1);
            this.Controls.Add(this.materialButton1);
            this.FormStyle = MaterialSkin.Controls.MaterialForm.FormStyles.ActionBar_None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(356, 148);
            this.Name = "Popup";
            this.Padding = new System.Windows.Forms.Padding(3, 24, 3, 3);
            this.Sizable = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Popup";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialButton materialButton1;
        public MaterialSkin.Controls.MaterialMultiLineTextBox materialMultiLineTextBox1;
    }
}