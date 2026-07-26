
namespace BnSBuddy2.Database
{
    partial class OnlineFetchUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlineFetchUI));
            this.materialComboBox1 = new MaterialSkin.Controls.MaterialComboBox();
            this.materialCard1 = new MaterialSkin.Controls.MaterialCard();
            this.authorLabel = new MaterialSkin.Controls.MaterialLabel();
            this.downloadButton = new MaterialSkin.Controls.MaterialButton();
            this.previewButton = new MaterialSkin.Controls.MaterialButton();
            this.descriptionTextbox = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            this.nameLabel = new MaterialSkin.Controls.MaterialLabel();
            this.materialComboBox2 = new MaterialSkin.Controls.MaterialComboBox();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.materialButton2 = new MaterialSkin.Controls.MaterialButton();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.materialButton3 = new MaterialSkin.Controls.MaterialButton();
            this.materialListBox1 = new MaterialSkin.Controls.MaterialListBox();
            this.materialCard1.SuspendLayout();
            this.SuspendLayout();
            // 
            // materialComboBox1
            // 
            this.materialComboBox1.AutoResize = false;
            this.materialComboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialComboBox1.Depth = 0;
            this.materialComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.materialComboBox1.DropDownHeight = 118;
            this.materialComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.materialComboBox1.DropDownWidth = 121;
            this.materialComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.materialComboBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialComboBox1.FormattingEnabled = true;
            this.materialComboBox1.Hint = "Select Category";
            this.materialComboBox1.IntegralHeight = false;
            this.materialComboBox1.ItemHeight = 29;
            this.materialComboBox1.Location = new System.Drawing.Point(6, 67);
            this.materialComboBox1.MaxDropDownItems = 4;
            this.materialComboBox1.MouseState = MaterialSkin.MouseState.OUT;
            this.materialComboBox1.Name = "materialComboBox1";
            this.materialComboBox1.Size = new System.Drawing.Size(174, 35);
            this.materialComboBox1.StartIndex = -1;
            this.materialComboBox1.TabIndex = 0;
            this.materialComboBox1.TabStop = false;
            this.materialComboBox1.UseTallSize = false;
            this.materialComboBox1.SelectedIndexChanged += new System.EventHandler(this.materialComboBox1_SelectedIndexChanged);
            // 
            // materialCard1
            // 
            this.materialCard1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialCard1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialCard1.Controls.Add(this.authorLabel);
            this.materialCard1.Controls.Add(this.downloadButton);
            this.materialCard1.Controls.Add(this.previewButton);
            this.materialCard1.Controls.Add(this.descriptionTextbox);
            this.materialCard1.Controls.Add(this.nameLabel);
            this.materialCard1.Depth = 0;
            this.materialCard1.Enabled = false;
            this.materialCard1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialCard1.Location = new System.Drawing.Point(421, 108);
            this.materialCard1.Margin = new System.Windows.Forms.Padding(14);
            this.materialCard1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCard1.Name = "materialCard1";
            this.materialCard1.Padding = new System.Windows.Forms.Padding(14);
            this.materialCard1.Size = new System.Drawing.Size(392, 562);
            this.materialCard1.TabIndex = 2;
            // 
            // authorLabel
            // 
            this.authorLabel.AutoSize = true;
            this.authorLabel.Depth = 0;
            this.authorLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.authorLabel.Location = new System.Drawing.Point(17, 33);
            this.authorLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.authorLabel.Name = "authorLabel";
            this.authorLabel.Size = new System.Drawing.Size(60, 19);
            this.authorLabel.TabIndex = 4;
            this.authorLabel.Text = "Author: -";
            // 
            // downloadButton
            // 
            this.downloadButton.AutoSize = false;
            this.downloadButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.downloadButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.downloadButton.Depth = 0;
            this.downloadButton.Enabled = false;
            this.downloadButton.HighEmphasis = true;
            this.downloadButton.Icon = null;
            this.downloadButton.Location = new System.Drawing.Point(222, 506);
            this.downloadButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.downloadButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.downloadButton.Size = new System.Drawing.Size(152, 36);
            this.downloadButton.TabIndex = 3;
            this.downloadButton.Text = "Download";
            this.downloadButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.downloadButton.UseAccentColor = false;
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // previewButton
            // 
            this.previewButton.AutoSize = false;
            this.previewButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.previewButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.previewButton.Depth = 0;
            this.previewButton.Enabled = false;
            this.previewButton.HighEmphasis = true;
            this.previewButton.Icon = null;
            this.previewButton.Location = new System.Drawing.Point(18, 506);
            this.previewButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.previewButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.previewButton.Name = "previewButton";
            this.previewButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.previewButton.Size = new System.Drawing.Size(152, 36);
            this.previewButton.TabIndex = 2;
            this.previewButton.Text = "Preview";
            this.previewButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.previewButton.UseAccentColor = false;
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // descriptionTextbox
            // 
            this.descriptionTextbox.AnimateReadOnly = false;
            this.descriptionTextbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.descriptionTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.descriptionTextbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.descriptionTextbox.Depth = 0;
            this.descriptionTextbox.HideSelection = true;
            this.descriptionTextbox.Hint = "Description";
            this.descriptionTextbox.Location = new System.Drawing.Point(17, 55);
            this.descriptionTextbox.MaxLength = 32767;
            this.descriptionTextbox.MouseState = MaterialSkin.MouseState.OUT;
            this.descriptionTextbox.Name = "descriptionTextbox";
            this.descriptionTextbox.PasswordChar = '\0';
            this.descriptionTextbox.ReadOnly = true;
            this.descriptionTextbox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.descriptionTextbox.SelectedText = "";
            this.descriptionTextbox.SelectionLength = 0;
            this.descriptionTextbox.SelectionStart = 0;
            this.descriptionTextbox.ShortcutsEnabled = false;
            this.descriptionTextbox.Size = new System.Drawing.Size(358, 442);
            this.descriptionTextbox.TabIndex = 1;
            this.descriptionTextbox.TabStop = false;
            this.descriptionTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.descriptionTextbox.UseSystemPasswordChar = false;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Depth = 0;
            this.nameLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nameLabel.Location = new System.Drawing.Point(17, 14);
            this.nameLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(55, 19);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name: -";
            // 
            // materialComboBox2
            // 
            this.materialComboBox2.AutoResize = false;
            this.materialComboBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialComboBox2.Depth = 0;
            this.materialComboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.materialComboBox2.DropDownHeight = 118;
            this.materialComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.materialComboBox2.DropDownWidth = 121;
            this.materialComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.materialComboBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialComboBox2.FormattingEnabled = true;
            this.materialComboBox2.Hint = "Search by Author";
            this.materialComboBox2.IntegralHeight = false;
            this.materialComboBox2.ItemHeight = 29;
            this.materialComboBox2.Location = new System.Drawing.Point(186, 67);
            this.materialComboBox2.MaxDropDownItems = 4;
            this.materialComboBox2.MouseState = MaterialSkin.MouseState.OUT;
            this.materialComboBox2.Name = "materialComboBox2";
            this.materialComboBox2.Size = new System.Drawing.Size(175, 35);
            this.materialComboBox2.StartIndex = -1;
            this.materialComboBox2.TabIndex = 4;
            this.materialComboBox2.TabStop = false;
            this.materialComboBox2.UseTallSize = false;
            this.materialComboBox2.SelectedIndexChanged += new System.EventHandler(this.materialComboBox2_SelectedIndexChanged);
            // 
            // materialButton1
            // 
            this.materialButton1.AutoSize = false;
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton1.Depth = 0;
            this.materialButton1.Enabled = false;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = null;
            this.materialButton1.Location = new System.Drawing.Point(661, 67);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton1.Size = new System.Drawing.Size(151, 36);
            this.materialButton1.TabIndex = 5;
            this.materialButton1.Text = "Manage";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            this.materialButton1.Click += new System.EventHandler(this.materialButton1_Click);
            // 
            // materialButton2
            // 
            this.materialButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton2.Depth = 0;
            this.materialButton2.HighEmphasis = true;
            this.materialButton2.Icon = null;
            this.materialButton2.Location = new System.Drawing.Point(421, 66);
            this.materialButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton2.Name = "materialButton2";
            this.materialButton2.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton2.Size = new System.Drawing.Size(90, 36);
            this.materialButton2.TabIndex = 6;
            this.materialButton2.Text = "Account";
            this.materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.materialButton2.UseAccentColor = false;
            this.materialButton2.UseVisualStyleBackColor = true;
            this.materialButton2.Click += new System.EventHandler(this.materialButton2_Click);
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.Location = new System.Drawing.Point(518, 76);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(100, 19);
            this.materialLabel1.TabIndex = 7;
            this.materialLabel1.Text = "Status: Visitor";
            // 
            // materialButton3
            // 
            this.materialButton3.AutoSize = false;
            this.materialButton3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton3.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton3.Depth = 0;
            this.materialButton3.Enabled = false;
            this.materialButton3.HighEmphasis = true;
            this.materialButton3.Icon = null;
            this.materialButton3.Location = new System.Drawing.Point(368, 66);
            this.materialButton3.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton3.Name = "materialButton3";
            this.materialButton3.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton3.Size = new System.Drawing.Size(36, 36);
            this.materialButton3.TabIndex = 8;
            this.materialButton3.Text = "X";
            this.materialButton3.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.materialButton3.UseAccentColor = false;
            this.materialButton3.UseVisualStyleBackColor = true;
            this.materialButton3.Click += new System.EventHandler(this.materialButton3_Click);
            // 
            // materialListBox1
            // 
            this.materialListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.materialListBox1.BackColor = System.Drawing.Color.White;
            this.materialListBox1.BorderColor = System.Drawing.Color.LightGray;
            this.materialListBox1.Depth = 0;
            this.materialListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialListBox1.Location = new System.Drawing.Point(6, 108);
            this.materialListBox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialListBox1.Name = "materialListBox1";
            this.materialListBox1.SelectedIndex = -1;
            this.materialListBox1.SelectedItem = null;
            this.materialListBox1.Size = new System.Drawing.Size(398, 562);
            this.materialListBox1.TabIndex = 9;
            this.materialListBox1.SelectedIndexChanged += new MaterialSkin.Controls.MaterialListBox.SelectedIndexChangedEventHandler(this.materialListBox1_SelectedIndexChanged);
            // 
            // OnlineFetchUI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(819, 676);
            this.Controls.Add(this.materialButton3);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.materialButton2);
            this.Controls.Add(this.materialButton1);
            this.Controls.Add(this.materialComboBox2);
            this.Controls.Add(this.materialCard1);
            this.Controls.Add(this.materialComboBox1);
            this.Controls.Add(this.materialListBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OnlineFetchUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Online Database";
            this.materialCard1.ResumeLayout(false);
            this.materialCard1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialComboBox materialComboBox1;
        private MaterialSkin.Controls.MaterialCard materialCard1;
        private MaterialSkin.Controls.MaterialComboBox materialComboBox2;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        private MaterialSkin.Controls.MaterialButton materialButton2;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialButton materialButton3;
        private MaterialSkin.Controls.MaterialListBox materialListBox1;
        private MaterialSkin.Controls.MaterialLabel nameLabel;
        private MaterialSkin.Controls.MaterialMultiLineTextBox2 descriptionTextbox;
        private MaterialSkin.Controls.MaterialLabel authorLabel;
        private MaterialSkin.Controls.MaterialButton previewButton;
        public MaterialSkin.Controls.MaterialButton downloadButton;
    }
}