namespace BnSBuddy2.LegacyBin
{
    partial class BinEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useIntDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.legacyXmlToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelTables = new System.Windows.Forms.Panel();
            this.listTables = new System.Windows.Forms.ListView();
            this.colTableIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTableId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTableKind = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTableRecords = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTableSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtTableFilter = new System.Windows.Forms.TextBox();
            this.lblTables = new System.Windows.Forms.Label();
            this.splitRight = new System.Windows.Forms.SplitContainer();
            this.panelFields = new System.Windows.Forms.Panel();
            this.gridFields = new System.Windows.Forms.DataGridView();
            this.txtFieldSearch = new System.Windows.Forms.TextBox();
            this.panelSubArchive = new System.Windows.Forms.Panel();
            this.comboSubArchive = new System.Windows.Forms.ComboBox();
            this.lblSubArchive = new System.Windows.Forms.Label();
            this.lblFields = new System.Windows.Forms.Label();
            this.panelStrings = new System.Windows.Forms.Panel();
            this.gridStrings = new System.Windows.Forms.DataGridView();
            this.txtStringSearch = new System.Windows.Forms.TextBox();
            this.lblStrings = new System.Windows.Forms.Label();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).BeginInit();
            this.splitRight.Panel1.SuspendLayout();
            this.splitRight.Panel2.SuspendLayout();
            this.splitRight.SuspendLayout();
            this.panelFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridFields)).BeginInit();
            this.panelSubArchive.SuspendLayout();
            this.panelStrings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridStrings)).BeginInit();
            this.SuspendLayout();
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileToolStripMenuItem,
                this.viewToolStripMenuItem,
                this.toolsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 24);
            this.menuStrip.TabIndex = 0;
            //
            // fileToolStripMenuItem
            //
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.openToolStripMenuItem,
                this.saveToolStripMenuItem,
                this.saveAsToolStripMenuItem,
                this.toolStripSeparator1,
                this.exportXmlToolStripMenuItem,
                this.toolStripSeparator2,
                this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            //
            // openToolStripMenuItem
            //
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            //
            // saveToolStripMenuItem
            //
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            //
            // saveAsToolStripMenuItem
            //
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
            //
            // exportXmlToolStripMenuItem
            //
            this.exportXmlToolStripMenuItem.Name = "exportXmlToolStripMenuItem";
            this.exportXmlToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.exportXmlToolStripMenuItem.Text = "&Export XML...";
            this.exportXmlToolStripMenuItem.Click += new System.EventHandler(this.exportXmlToolStripMenuItem_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(192, 6);
            //
            // exitToolStripMenuItem
            //
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            //
            // viewToolStripMenuItem
            //
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.useIntDataToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            //
            // useIntDataToolStripMenuItem
            //
            this.useIntDataToolStripMenuItem.Checked = true;
            this.useIntDataToolStripMenuItem.CheckOnClick = true;
            this.useIntDataToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useIntDataToolStripMenuItem.Name = "useIntDataToolStripMenuItem";
            this.useIntDataToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.useIntDataToolStripMenuItem.Text = "Field data as &ints";
            this.useIntDataToolStripMenuItem.CheckedChanged += new System.EventHandler(this.useIntDataToolStripMenuItem_CheckedChanged);
            //
            // toolsToolStripMenuItem
            //
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.legacyXmlToolsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            //
            // legacyXmlToolsToolStripMenuItem
            //
            this.legacyXmlToolsToolStripMenuItem.Name = "legacyXmlToolsToolStripMenuItem";
            this.legacyXmlToolsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.legacyXmlToolsToolStripMenuItem.Text = "Legacy Unpack/Repack...";
            this.legacyXmlToolsToolStripMenuItem.Click += new System.EventHandler(this.legacyXmlToolsToolStripMenuItem_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 678);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 22);
            this.statusStrip.TabIndex = 1;
            //
            // statusLabel
            //
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(1185, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Ready. Open a .bin file to begin.";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 24);
            this.splitMain.Name = "splitMain";
            //
            // splitMain.Panel1
            //
            this.splitMain.Panel1.Controls.Add(this.panelTables);
            //
            // splitMain.Panel2
            //
            this.splitMain.Panel2.Controls.Add(this.splitRight);
            this.splitMain.Size = new System.Drawing.Size(1200, 654);
            this.splitMain.SplitterDistance = 280;
            this.splitMain.TabIndex = 2;
            //
            // panelTables
            //
            this.panelTables.Controls.Add(this.listTables);
            this.panelTables.Controls.Add(this.txtTableFilter);
            this.panelTables.Controls.Add(this.lblTables);
            this.panelTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTables.Location = new System.Drawing.Point(0, 0);
            this.panelTables.Name = "panelTables";
            this.panelTables.Padding = new System.Windows.Forms.Padding(6);
            this.panelTables.Size = new System.Drawing.Size(280, 654);
            this.panelTables.TabIndex = 0;
            //
            // listTables
            //
            this.listTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colTableIndex,
                this.colTableId,
                this.colTableKind,
                this.colTableRecords,
                this.colTableSize});
            this.listTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listTables.FullRowSelect = true;
            this.listTables.HideSelection = false;
            this.listTables.Location = new System.Drawing.Point(6, 51);
            this.listTables.MultiSelect = false;
            this.listTables.Name = "listTables";
            this.listTables.Size = new System.Drawing.Size(268, 597);
            this.listTables.TabIndex = 2;
            this.listTables.UseCompatibleStateImageBehavior = false;
            this.listTables.View = System.Windows.Forms.View.Details;
            this.listTables.SelectedIndexChanged += new System.EventHandler(this.listTables_SelectedIndexChanged);
            //
            // colTableIndex
            //
            this.colTableIndex.Text = "#";
            this.colTableIndex.Width = 36;
            //
            // colTableId
            //
            this.colTableId.Text = "Id";
            this.colTableId.Width = 50;
            //
            // colTableKind
            //
            this.colTableKind.Text = "Kind";
            this.colTableKind.Width = 60;
            //
            // colTableRecords
            //
            this.colTableRecords.Text = "Recs";
            this.colTableRecords.Width = 55;
            //
            // colTableSize
            //
            this.colTableSize.Text = "Size";
            this.colTableSize.Width = 70;
            //
            // txtTableFilter
            //
            this.txtTableFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtTableFilter.Location = new System.Drawing.Point(6, 25);
            this.txtTableFilter.Name = "txtTableFilter";
            this.txtTableFilter.Size = new System.Drawing.Size(268, 26);
            this.txtTableFilter.TabIndex = 1;
            this.txtTableFilter.TextChanged += new System.EventHandler(this.txtTableFilter_TextChanged);
            //
            // lblTables
            //
            this.lblTables.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTables.Location = new System.Drawing.Point(6, 6);
            this.lblTables.Name = "lblTables";
            this.lblTables.Size = new System.Drawing.Size(268, 19);
            this.lblTables.TabIndex = 0;
            this.lblTables.Text = "Tables";
            //
            // splitRight
            //
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.Location = new System.Drawing.Point(0, 0);
            this.splitRight.Name = "splitRight";
            this.splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitRight.Panel1
            //
            this.splitRight.Panel1.Controls.Add(this.panelFields);
            //
            // splitRight.Panel2
            //
            this.splitRight.Panel2.Controls.Add(this.panelStrings);
            this.splitRight.Size = new System.Drawing.Size(916, 654);
            this.splitRight.SplitterDistance = 360;
            this.splitRight.TabIndex = 0;
            //
            // panelFields
            //
            this.panelFields.Controls.Add(this.gridFields);
            this.panelFields.Controls.Add(this.txtFieldSearch);
            this.panelFields.Controls.Add(this.panelSubArchive);
            this.panelFields.Controls.Add(this.lblFields);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFields.Location = new System.Drawing.Point(0, 0);
            this.panelFields.Name = "panelFields";
            this.panelFields.Padding = new System.Windows.Forms.Padding(6);
            this.panelFields.Size = new System.Drawing.Size(916, 360);
            this.panelFields.TabIndex = 0;
            //
            // gridFields
            //
            this.gridFields.AllowUserToAddRows = false;
            this.gridFields.AllowUserToDeleteRows = false;
            this.gridFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridFields.Location = new System.Drawing.Point(6, 77);
            this.gridFields.MultiSelect = false;
            this.gridFields.Name = "gridFields";
            this.gridFields.RowHeadersWidth = 50;
            this.gridFields.Size = new System.Drawing.Size(904, 277);
            this.gridFields.TabIndex = 3;
            this.gridFields.VirtualMode = true;
            this.gridFields.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridFields_CellValueNeeded);
            this.gridFields.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridFields_CellValuePushed);
            //
            // txtFieldSearch
            //
            this.txtFieldSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtFieldSearch.Location = new System.Drawing.Point(6, 51);
            this.txtFieldSearch.Margin = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.txtFieldSearch.Name = "txtFieldSearch";
            this.txtFieldSearch.Size = new System.Drawing.Size(904, 26);
            this.txtFieldSearch.TabIndex = 2;
            this.txtFieldSearch.TextChanged += new System.EventHandler(this.txtFieldSearch_TextChanged);
            //
            // panelSubArchive
            //
            this.panelSubArchive.Controls.Add(this.comboSubArchive);
            this.panelSubArchive.Controls.Add(this.lblSubArchive);
            this.panelSubArchive.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSubArchive.Location = new System.Drawing.Point(6, 25);
            this.panelSubArchive.Name = "panelSubArchive";
            this.panelSubArchive.Padding = new System.Windows.Forms.Padding(0, 4, 0, 6);
            this.panelSubArchive.Size = new System.Drawing.Size(904, 40);
            this.panelSubArchive.TabIndex = 1;
            this.panelSubArchive.Visible = false;
            //
            // comboSubArchive
            //
            this.comboSubArchive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSubArchive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSubArchive.FormattingEnabled = true;
            this.comboSubArchive.Location = new System.Drawing.Point(90, 6);
            this.comboSubArchive.Name = "comboSubArchive";
            this.comboSubArchive.Size = new System.Drawing.Size(600, 24);
            this.comboSubArchive.TabIndex = 1;
            this.comboSubArchive.SelectedIndexChanged += new System.EventHandler(this.comboSubArchive_SelectedIndexChanged);
            //
            // lblSubArchive
            //
            this.lblSubArchive.AutoSize = true;
            this.lblSubArchive.Location = new System.Drawing.Point(0, 10);
            this.lblSubArchive.Name = "lblSubArchive";
            this.lblSubArchive.Size = new System.Drawing.Size(84, 16);
            this.lblSubArchive.TabIndex = 0;
            this.lblSubArchive.Text = "Sub-archive:";
            //
            // lblFields
            //
            this.lblFields.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFields.Location = new System.Drawing.Point(6, 6);
            this.lblFields.Name = "lblFields";
            this.lblFields.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblFields.Size = new System.Drawing.Size(904, 22);
            this.lblFields.TabIndex = 0;
            this.lblFields.Text = "Fields";
            //
            // panelStrings
            //
            this.panelStrings.Controls.Add(this.gridStrings);
            this.panelStrings.Controls.Add(this.txtStringSearch);
            this.panelStrings.Controls.Add(this.lblStrings);
            this.panelStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStrings.Location = new System.Drawing.Point(0, 0);
            this.panelStrings.Name = "panelStrings";
            this.panelStrings.Padding = new System.Windows.Forms.Padding(6);
            this.panelStrings.Size = new System.Drawing.Size(916, 290);
            this.panelStrings.TabIndex = 0;
            //
            // gridStrings
            //
            this.gridStrings.AllowUserToAddRows = false;
            this.gridStrings.AllowUserToDeleteRows = false;
            this.gridStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridStrings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridStrings.Location = new System.Drawing.Point(6, 51);
            this.gridStrings.MultiSelect = false;
            this.gridStrings.Name = "gridStrings";
            this.gridStrings.RowHeadersWidth = 50;
            this.gridStrings.Size = new System.Drawing.Size(904, 233);
            this.gridStrings.TabIndex = 2;
            this.gridStrings.VirtualMode = true;
            this.gridStrings.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridStrings_CellValueNeeded);
            this.gridStrings.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridStrings_CellValuePushed);
            //
            // txtStringSearch
            //
            this.txtStringSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtStringSearch.Location = new System.Drawing.Point(6, 25);
            this.txtStringSearch.Name = "txtStringSearch";
            this.txtStringSearch.Size = new System.Drawing.Size(904, 26);
            this.txtStringSearch.TabIndex = 1;
            this.txtStringSearch.TextChanged += new System.EventHandler(this.txtStringSearch_TextChanged);
            //
            // lblStrings
            //
            this.lblStrings.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStrings.Location = new System.Drawing.Point(6, 6);
            this.lblStrings.Name = "lblStrings";
            this.lblStrings.Size = new System.Drawing.Size(904, 19);
            this.lblStrings.TabIndex = 0;
            this.lblStrings.Text = "Lookup strings";
            //
            // BinEditorForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "BinEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LegacyBin Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BinEditorForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelTables.ResumeLayout(false);
            this.panelTables.PerformLayout();
            this.splitRight.Panel1.ResumeLayout(false);
            this.splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).EndInit();
            this.splitRight.ResumeLayout(false);
            this.panelFields.ResumeLayout(false);
            this.panelFields.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridFields)).EndInit();
            this.panelSubArchive.ResumeLayout(false);
            this.panelSubArchive.PerformLayout();
            this.panelStrings.ResumeLayout(false);
            this.panelStrings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridStrings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exportXmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useIntDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem legacyXmlToolsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel panelTables;
        private System.Windows.Forms.ListView listTables;
        private System.Windows.Forms.ColumnHeader colTableIndex;
        private System.Windows.Forms.ColumnHeader colTableId;
        private System.Windows.Forms.ColumnHeader colTableKind;
        private System.Windows.Forms.ColumnHeader colTableRecords;
        private System.Windows.Forms.ColumnHeader colTableSize;
        private System.Windows.Forms.TextBox txtTableFilter;
        private System.Windows.Forms.Label lblTables;
        private System.Windows.Forms.SplitContainer splitRight;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.DataGridView gridFields;
        private System.Windows.Forms.TextBox txtFieldSearch;
        private System.Windows.Forms.Panel panelSubArchive;
        private System.Windows.Forms.ComboBox comboSubArchive;
        private System.Windows.Forms.Label lblSubArchive;
        private System.Windows.Forms.Label lblFields;
        private System.Windows.Forms.Panel panelStrings;
        private System.Windows.Forms.DataGridView gridStrings;
        private System.Windows.Forms.TextBox txtStringSearch;
        private System.Windows.Forms.Label lblStrings;
    }
}
