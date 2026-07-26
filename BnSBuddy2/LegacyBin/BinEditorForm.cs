using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.LegacyBin
{
    public partial class BinEditorForm : Form
    {
        private readonly BinSession _session = new BinSession();
        private readonly EditorUndoStack _undo = new EditorUndoStack();
        private bool _busy;
        private bool _suppressTableSelect;
        private bool _undoRecording = true;

        // Current table editing context
        private int _selectedTableIndex = -1;
        private BDAT_LIST _selectedList;
        private BDAT_FIELDTABLE[] _fields = new BDAT_FIELDTABLE[0];
        private string[] _words = new string[0];
        private int[] _fieldMap = new int[0];   // filtered -> real index
        private int[] _stringMap = new int[0];
        private bool _stringsDirty;
        private bool _isArchiveTable;
        private List<BDAT_LOOKUPTABLE> _archiveLookups;
        private BDAT_ARCHIVE _currentArchive;
        private int _currentSubIndex = -1; // -1 = all blocks merged; >=0 = one sub-archive
        private bool _suppressSubArchiveSelect;

        // QoL chrome
        private ToolStripMenuItem _undoMenu;
        private ToolStripMenuItem _redoMenu;
        private Button _btnAddField;
        private Button _btnDelField;
        private Button _btnAddString;
        private Button _btnDelString;
        private Button _btnAddSub;
        private Button _btnDelSub;

        public BinEditorForm()
        {
            InitializeComponent();
            InitGrids();
            InitQoLChrome();
            BinEditOptions.Progress = msg =>
            {
                if (IsDisposed)
                {
                    return;
                }
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => SetStatus(msg)));
                }
                else
                {
                    SetStatus(msg);
                }
            };
            BinEditOptions.UseIntData = useIntDataToolStripMenuItem.Checked;
            _undo.Changed += UpdateUndoRedoMenus;
            UpdateTitle();
            UpdateUndoRedoMenus();
            UiTheme.Apply(this);
        }

        private void InitGrids()
        {
            gridFields.Columns.Clear();
            gridFields.Columns.Add(new DataGridViewTextBoxColumn { Name = "colIdx", HeaderText = "#", ReadOnly = true, Width = 50 });
            gridFields.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "Id", Width = 70 });
            gridFields.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSize", HeaderText = "Size", Width = 60 });
            gridFields.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnk1", HeaderText = "Unk1", Width = 55 });
            gridFields.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUnk2", HeaderText = "Unk2", Width = 55 });
            gridFields.Columns.Add(new DataGridViewTextBoxColumn { Name = "colData", HeaderText = "Data", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            gridFields.VirtualMode = true;
            gridFields.RowCount = 0;
            gridFields.KeyDown += gridFields_KeyDown;

            gridStrings.Columns.Clear();
            gridStrings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSIdx", HeaderText = "#", ReadOnly = true, Width = 50 });
            gridStrings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colWord", HeaderText = "Text", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            gridStrings.VirtualMode = true;
            gridStrings.RowCount = 0;
            gridStrings.KeyDown += gridStrings_KeyDown;
        }

        private void InitQoLChrome()
        {
            // --- Edit menu ---
            var editMenu = new ToolStripMenuItem("&Edit");
            _undoMenu = new ToolStripMenuItem("Undo", null, (s, e) => DoUndo()) { ShortcutKeys = Keys.Control | Keys.Z };
            _redoMenu = new ToolStripMenuItem("Redo", null, (s, e) => DoRedo()) { ShortcutKeys = Keys.Control | Keys.Y };
            var findAll = new ToolStripMenuItem("Find in &all tables...", null, (s, e) => ShowSearchAllDialog())
            {
                ShortcutKeys = Keys.Control | Keys.Shift | Keys.F
            };
            editMenu.DropDownItems.Add(_undoMenu);
            editMenu.DropDownItems.Add(_redoMenu);
            editMenu.DropDownItems.Add(new ToolStripSeparator());
            editMenu.DropDownItems.Add(findAll);
            menuStrip.Items.Insert(1, editMenu);

            // --- Tools: localfile translate ---
            toolsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem("Localfile &Translate...", null, (s, e) =>
            {
                using (var f = new TranslateForm(_session, () =>
                {
                    // Refresh UI after apply
                    if (_selectedTableIndex >= 0)
                    {
                        LoadTable(_selectedTableIndex);
                    }
                    PopulateTableList();
                    UpdateTitle();
                    SetStatus("Translation applied (remember to Save)");
                }))
                {
                    f.ShowDialog(this);
                }
            }));
            toolsToolStripMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());

            // --- View extras ---
            var darkModeItem = new ToolStripMenuItem("&Dark mode")
            {
                Checked = BinEditOptions.DarkMode,
                CheckOnClick = true
            };
            darkModeItem.CheckedChanged += (s, e) =>
            {
                UiTheme.DarkMode = darkModeItem.Checked;
                SetStatus(darkModeItem.Checked ? "Dark mode on" : "Light mode on");
            };
            viewToolStripMenuItem.DropDownItems.Add(darkModeItem);
            viewToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            viewToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("File &header...", null, (s, e) => ShowHeaderInfo()));
            viewToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("&Name table (hex)...", null, (s, e) => ShowNameTable()));
            viewToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("Region &tail / padding (hex)...", null, (s, e) => ShowRegionTail()));

            // --- Sub-archive add/delete (next to dropdown panel) ---
            _btnAddSub = new Button
            {
                Text = "Add block",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnDelSub = new Button
            {
                Text = "Delete block",
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnAddSub.Click += (s, e) => AddSubArchiveBlock();
            _btnDelSub.Click += (s, e) => DeleteSubArchiveBlock();
            panelSubArchive.Controls.Add(_btnAddSub);
            panelSubArchive.Controls.Add(_btnDelSub);
            void LayoutSubArchiveBar()
            {
                int top = 6;
                _btnDelSub.Top = top;
                _btnAddSub.Top = top;
                comboSubArchive.Top = top;
                lblSubArchive.Top = top + 4;
                _btnDelSub.Left = Math.Max(200, panelSubArchive.ClientSize.Width - _btnDelSub.Width - 4);
                _btnAddSub.Left = _btnDelSub.Left - _btnAddSub.Width - 8;
                comboSubArchive.Left = 90;
                comboSubArchive.Width = Math.Max(80, _btnAddSub.Left - comboSubArchive.Left - 10);
            }
            panelSubArchive.Resize += (s, e) => LayoutSubArchiveBar();
            LayoutSubArchiveBar();

            // --- Field toolbar ---
            var fieldTools = new FlowLayoutPanel
            {
                Name = "fieldTools",
                Dock = DockStyle.Top,
                Height = 36,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 4, 0, 6),
                Margin = new Padding(0)
            };
            _btnAddField = new Button { Text = "Add field", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            _btnDelField = new Button { Text = "Delete field", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            _btnAddField.Click += (s, e) => AddField();
            _btnDelField.Click += (s, e) => DeleteSelectedField();
            fieldTools.Controls.Add(_btnAddField);
            fieldTools.Controls.Add(_btnDelField);
            panelFields.Controls.Add(fieldTools);

            // WinForms: control at bottom of z-order is docked first (appears at top).
            panelFields.SuspendLayout();
            panelFields.Controls.SetChildIndex(gridFields, 0);
            panelFields.Controls.SetChildIndex(txtFieldSearch, 1);
            panelFields.Controls.SetChildIndex(fieldTools, 2);
            panelFields.Controls.SetChildIndex(panelSubArchive, 3);
            panelFields.Controls.SetChildIndex(lblFields, 4);
            panelFields.ResumeLayout(true);
            UiTheme.Apply(panelFields);

            // --- String toolbar ---
            var stringTools = new FlowLayoutPanel
            {
                Name = "stringTools",
                Dock = DockStyle.Top,
                Height = 36,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 4, 0, 6)
            };
            _btnAddString = new Button { Text = "Add string", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            _btnDelString = new Button { Text = "Delete string", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            _btnAddString.Click += (s, e) => AddString();
            _btnDelString.Click += (s, e) => DeleteSelectedString();
            stringTools.Controls.Add(_btnAddString);
            stringTools.Controls.Add(_btnDelString);
            panelStrings.Controls.Add(stringTools);
            panelStrings.SuspendLayout();
            panelStrings.Controls.SetChildIndex(gridStrings, 0);
            panelStrings.Controls.SetChildIndex(txtStringSearch, 1);
            panelStrings.Controls.SetChildIndex(stringTools, 2);
            panelStrings.Controls.SetChildIndex(lblStrings, 3);
            panelStrings.ResumeLayout(true);
        }

        private void UpdateUndoRedoMenus()
        {
            if (_undoMenu == null)
            {
                return;
            }
            _undoMenu.Enabled = _undo.CanUndo;
            _redoMenu.Enabled = _undo.CanRedo;
            _undoMenu.Text = _undo.CanUndo ? "Undo " + _undo.UndoDescription : "Undo";
            _redoMenu.Text = _undo.CanRedo ? "Redo " + _undo.RedoDescription : "Redo";
        }

        private void DoUndo()
        {
            if (!_undo.CanUndo || _busy)
            {
                return;
            }
            gridFields.EndEdit();
            gridStrings.EndEdit();
            _undoRecording = false;
            try
            {
                _undo.Undo();
                RefreshCurrentViewAfterUndo();
                _session.MarkDirty();
                UpdateTitle();
                SetStatus("Undo");
            }
            finally
            {
                _undoRecording = true;
            }
        }

        private void DoRedo()
        {
            if (!_undo.CanRedo || _busy)
            {
                return;
            }
            gridFields.EndEdit();
            gridStrings.EndEdit();
            _undoRecording = false;
            try
            {
                _undo.Redo();
                RefreshCurrentViewAfterUndo();
                _session.MarkDirty();
                UpdateTitle();
                SetStatus("Redo");
            }
            finally
            {
                _undoRecording = true;
            }
        }

        private void RefreshCurrentViewAfterUndo()
        {
            // Reload current table projection from model
            if (_selectedTableIndex >= 0 && _session.IsOpen)
            {
                // Don't flush strings (would overwrite undo)
                _stringsDirty = false;
                if (_isArchiveTable && _selectedList?.Collection?.Archive != null)
                {
                    _currentArchive = _selectedList.Collection.Archive;
                    // Rebuild combo in case block list changed
                    int prefer = _currentSubIndex;
                    SetupArchiveSubCombo(_currentArchive);
                    if (prefer >= 0 && prefer + 1 < comboSubArchive.Items.Count)
                    {
                        _suppressSubArchiveSelect = true;
                        comboSubArchive.SelectedIndex = prefer + 1;
                        _suppressSubArchiveSelect = false;
                        ApplySelectedSubArchive();
                    }
                }
                else if (_selectedList?.Collection?.Loose != null)
                {
                    LoadLooseTable(_selectedList.Collection.Loose);
                    ApplyFieldFilter();
                    ApplyStringFilter();
                }
            }
            gridFields.Invalidate();
            gridStrings.Invalidate();
            PopulateTableList();
        }

        private void PushUndo(IEditorUndoAction action)
        {
            if (_undoRecording && action != null)
            {
                _undo.Push(action);
            }
        }

        private void SetStatus(string text)
        {
            statusLabel.Text = text ?? string.Empty;
            statusStrip.Refresh();
        }

        private void UpdateTitle()
        {
            string name = string.IsNullOrEmpty(_session.FilePath) ? "Untitled" : Path.GetFileName(_session.FilePath);
            string dirty = _session.IsDirty ? "*" : "";
            string arch = _session.IsOpen ? (_session.Is64Bit ? " [64-bit]" : " [32-bit]") : "";
            Text = dirty + name + arch + " — LegacyBin Editor";
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            UseWaitCursor = busy;
            menuStrip.Enabled = !busy;
            listTables.Enabled = !busy;
            gridFields.Enabled = !busy;
            gridStrings.Enabled = !busy;
        }

        private bool ConfirmDiscardIfDirty()
        {
            if (!_session.IsDirty)
            {
                return true;
            }
            var r = MessageBox.Show(
                "You have unsaved changes. Discard them?",
                "Unsaved changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);
            return r == DialogResult.Yes;
        }

        /// <summary>Open a bin path from Buddy UI (async, non-blocking).</summary>
        public void OpenPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }
            if (_busy)
            {
                return;
            }
            if (!ConfirmDiscardIfDirty())
            {
                return;
            }
            BeginInvoke(new Action(async () => await OpenPathAsync(path)));
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_busy)
            {
                return;
            }
            if (!ConfirmDiscardIfDirty())
            {
                return;
            }

            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Open bin file";
                dlg.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
                DialogPaths.Apply(dlg);
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                DialogPaths.RememberFile(dlg.FileName);
                await OpenPathAsync(dlg.FileName);
            }
        }

        private async Task OpenPathAsync(string path)
        {
            SetBusy(true);
            FlushCurrentTableEdits();
            try
            {
                await Task.Run(() => _session.Open(path));
                _undo.Clear();
                PopulateTableList();
                ClearDetailPanes();
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Open failed: " + ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_busy || !_session.IsOpen)
            {
                return;
            }
            if (string.IsNullOrEmpty(_session.FilePath))
            {
                saveAsToolStripMenuItem_Click(sender, e);
                return;
            }
            await SaveAsync(null);
        }

        private async void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_busy || !_session.IsOpen)
            {
                return;
            }
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "Save bin file";
                dlg.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
                DialogPaths.Apply(dlg);
                if (!string.IsNullOrEmpty(_session.FilePath))
                {
                    dlg.FileName = Path.GetFileName(_session.FilePath);
                    string dir = Path.GetDirectoryName(_session.FilePath);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    {
                        dlg.InitialDirectory = dir;
                    }
                }
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                DialogPaths.RememberFile(dlg.FileName);
                await SaveAsync(dlg.FileName);
            }
        }

        private async Task SaveAsync(string saveAsPath)
        {
            SetBusy(true);
            FlushCurrentTableEdits();
            try
            {
                await Task.Run(() =>
                {
                    if (string.IsNullOrEmpty(saveAsPath))
                    {
                        _session.Save();
                    }
                    else
                    {
                        _session.SaveAs(saveAsPath);
                    }
                });
                UpdateTitle();
                SetStatus("Saved " + Path.GetFileName(_session.FilePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Save failed: " + ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void exportXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_busy || !_session.IsOpen)
            {
                return;
            }
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Export per-table XML files to folder";
                DialogPaths.Apply(dlg);
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                DialogPaths.RememberFolder(dlg.SelectedPath);
                SetBusy(true);
                FlushCurrentTableEdits();
                try
                {
                    string dir = dlg.SelectedPath;
                    await Task.Run(() => _session.ExportXml(dir));
                    MessageBox.Show(this, "Exported XML to:\n" + dir, "Export XML", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusy(false);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void useIntDataToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            BinEditOptions.UseIntData = useIntDataToolStripMenuItem.Checked;
            if (_fields != null && _fields.Length > 0)
            {
                gridFields.Invalidate();
            }
        }

        private void legacyXmlToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this,
                "XML dump/repack for bins is available from BnS Buddy's Dat Editor:\n" +
                "right-click a .bin under an extracted .dat, then File Unpack / File Repack.\n\n" +
                "This editor also supports File → Export XML… for per-table XML export.",
                "Legacy XML tools",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void BinEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_busy)
            {
                e.Cancel = true;
                return;
            }
            FlushCurrentTableEdits();
            if (!ConfirmDiscardIfDirty())
            {
                e.Cancel = true;
            }
        }

        private void PopulateTableList()
        {
            _suppressTableSelect = true;
            listTables.BeginUpdate();
            listTables.Items.Clear();
            if (!_session.IsOpen)
            {
                listTables.EndUpdate();
                _suppressTableSelect = false;
                return;
            }

            string filter = (txtTableFilter.Text ?? string.Empty).Trim();
            for (int i = 0; i < _session.Content.ListCount; i++)
            {
                BDAT_LIST list = _session.Content.Lists[i];
                string kind = _session.GetTableKind(list);
                int recs = _session.GetRecordCount(list);
                string idStr = list.ID.ToString();
                if (!string.IsNullOrEmpty(filter))
                {
                    if (idStr.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0
                        && kind.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0
                        && i.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                }
                var item = new ListViewItem(i.ToString());
                item.SubItems.Add(idStr);
                item.SubItems.Add(kind);
                item.SubItems.Add(recs.ToString());
                item.SubItems.Add(list.Size.ToString());
                item.Tag = i;
                listTables.Items.Add(item);
            }
            listTables.EndUpdate();
            _suppressTableSelect = false;
            lblTables.Text = "Tables (" + listTables.Items.Count + "/" + _session.Content.ListCount + ")";
        }

        private void txtTableFilter_TextChanged(object sender, EventArgs e)
        {
            if (_session.IsOpen)
            {
                PopulateTableList();
            }
        }

        private void listTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressTableSelect || _busy)
            {
                return;
            }
            if (listTables.SelectedItems.Count == 0)
            {
                return;
            }
            FlushCurrentTableEdits();
            int tableIndex = (int)listTables.SelectedItems[0].Tag;
            LoadTable(tableIndex);
        }

        private void ClearDetailPanes()
        {
            _selectedTableIndex = -1;
            _selectedList = null;
            _fields = new BDAT_FIELDTABLE[0];
            _words = new string[0];
            _fieldMap = new int[0];
            _stringMap = new int[0];
            _stringsDirty = false;
            _isArchiveTable = false;
            _archiveLookups = null;
            _currentArchive = null;
            _currentSubIndex = -1;
            gridFields.RowCount = 0;
            gridStrings.RowCount = 0;
            lblFields.Text = "Fields";
            lblStrings.Text = "Lookup strings";
            ResetSubArchiveCombo(visible: false);
        }

        private void ResetSubArchiveCombo(bool visible)
        {
            _suppressSubArchiveSelect = true;
            comboSubArchive.Items.Clear();
            panelSubArchive.Visible = visible;
            _suppressSubArchiveSelect = false;
        }

        private void LoadTable(int tableIndex)
        {
            if (!_session.IsOpen || tableIndex < 0 || tableIndex >= _session.Content.ListCount)
            {
                ClearDetailPanes();
                return;
            }

            _selectedTableIndex = tableIndex;
            _selectedList = _session.Content.Lists[tableIndex];
            _stringsDirty = false;
            _archiveLookups = null;
            _currentArchive = null;
            _currentSubIndex = -1;

            if (_selectedList.Collection != null && _selectedList.Collection.Compressed >= 1
                && _selectedList.Collection.Archive != null)
            {
                _isArchiveTable = true;
                SetupArchiveSubCombo(_selectedList.Collection.Archive);
            }
            else if (_selectedList.Collection?.Loose != null)
            {
                _isArchiveTable = false;
                ResetSubArchiveCombo(visible: false);
                LoadLooseTable(_selectedList.Collection.Loose);
                ApplyFieldFilter();
                ApplyStringFilter();
            }
            else
            {
                _isArchiveTable = false;
                ResetSubArchiveCombo(visible: false);
                _fields = new BDAT_FIELDTABLE[0];
                _words = new string[0];
                lblFields.Text = "Fields (empty table)";
                lblStrings.Text = "Lookup strings";
                ApplyFieldFilter();
                ApplyStringFilter();
            }

            UpdateStatusForCurrentTable();
        }

        private void UpdateStatusForCurrentTable()
        {
            if (_selectedList == null)
            {
                return;
            }
            string kind = _isArchiveTable ? "Archive" : "Loose";
            string sub = "";
            if (_isArchiveTable)
            {
                if (_currentSubIndex < 0)
                {
                    sub = ", all blocks";
                }
                else
                {
                    sub = ", sub-archive " + _currentSubIndex;
                }
            }
            SetStatus("Table id=" + _selectedList.ID + " (" + kind + sub + "), fields=" + _fields.Length
                + ", strings=" + _words.Length
                + (_isArchiveTable ? " — edits recompress on save" : ""));
        }

        private void LoadLooseTable(BDAT_LOOSE loose)
        {
            _fields = loose.Fields ?? new BDAT_FIELDTABLE[0];
            if (loose.Lookup != null && loose.Lookup.Data != null && loose.Lookup.Size > 0)
            {
                var list = bnsTool.LookupSplitToWords(loose.Lookup.Data, (uint)loose.Lookup.Size);
                _words = list.ToArray();
            }
            else
            {
                _words = new string[0];
            }
            lblFields.Text = "Fields — loose (count=" + _fields.Length + ", unfixed=" + loose.FieldCountUnfixed
                + (loose.Is64 ? ", 64-pad" : "") + ")";
            lblStrings.Text = "Lookup strings (" + _words.Length + ")";
        }

        private void SetupArchiveSubCombo(BDAT_ARCHIVE archive)
        {
            _currentArchive = archive;
            var subs = archive.SubArchives ?? new BDAT_SUBARCHIVE[0];
            _suppressSubArchiveSelect = true;
            comboSubArchive.Items.Clear();
            comboSubArchive.Items.Add(new SubArchiveItem(-1, "All blocks (merged) — " + CountArchiveRecords(subs) + " records, "
                + subs.Length + " blocks"));
            for (int i = 0; i < subs.Length; i++)
            {
                var sub = subs[i];
                int recs = sub?.Fields == null ? 0 : sub.Fields.Length;
                int comp = sub == null ? 0 : sub.SizeCompressed;
                int decomp = sub == null ? 0 : sub.SizeDecompressed;
                comboSubArchive.Items.Add(new SubArchiveItem(i,
                    "Block " + i + " — " + recs + " records (comp=" + comp + ", decomp=" + decomp + ")"));
            }
            panelSubArchive.Visible = true;
            // Default to first real block when any exist; else "All"
            if (subs.Length > 0)
            {
                comboSubArchive.SelectedIndex = 1; // first sub-archive
            }
            else
            {
                comboSubArchive.SelectedIndex = 0;
            }
            _suppressSubArchiveSelect = false;
            // Force load even if SelectedIndexChanged was suppressed
            ApplySelectedSubArchive();
        }

        private static int CountArchiveRecords(BDAT_SUBARCHIVE[] subs)
        {
            int n = 0;
            if (subs == null)
            {
                return 0;
            }
            foreach (var s in subs)
            {
                if (s?.Fields != null)
                {
                    n += s.Fields.Length;
                }
            }
            return n;
        }

        private void comboSubArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressSubArchiveSelect || _busy)
            {
                return;
            }
            FlushCurrentTableEdits();
            ApplySelectedSubArchive();
            UpdateStatusForCurrentTable();
        }

        private void ApplySelectedSubArchive()
        {
            if (_currentArchive == null)
            {
                return;
            }
            int subIndex = -1;
            if (comboSubArchive.SelectedItem is SubArchiveItem item)
            {
                subIndex = item.Index;
            }
            else if (comboSubArchive.SelectedIndex > 0)
            {
                subIndex = comboSubArchive.SelectedIndex - 1;
            }
            _currentSubIndex = subIndex;
            _stringsDirty = false;

            if (subIndex < 0)
            {
                LoadArchiveMerged(_currentArchive);
            }
            else
            {
                LoadArchiveBlock(_currentArchive, subIndex);
            }
            ApplyFieldFilter();
            ApplyStringFilter();
        }

        private void LoadArchiveMerged(BDAT_ARCHIVE archive)
        {
            var fields = new List<BDAT_FIELDTABLE>();
            var lookups = new List<BDAT_LOOKUPTABLE>();
            if (archive.SubArchives != null)
            {
                foreach (var sub in archive.SubArchives)
                {
                    AppendSubArchive(sub, fields, lookups);
                }
            }
            _fields = fields.ToArray();
            _archiveLookups = lookups;
            _words = BuildWordsFromLookups(lookups);
            int blocks = archive.SubArchives == null ? 0 : archive.SubArchives.Length;
            lblFields.Text = "Fields — archive ALL blocks (records=" + _fields.Length + ", blocks=" + blocks + ")";
            lblStrings.Text = "Lookup strings — all blocks (" + _words.Length + " words)";
        }

        private void LoadArchiveBlock(BDAT_ARCHIVE archive, int subIndex)
        {
            var fields = new List<BDAT_FIELDTABLE>();
            var lookups = new List<BDAT_LOOKUPTABLE>();
            BDAT_SUBARCHIVE sub = null;
            if (archive.SubArchives != null && subIndex >= 0 && subIndex < archive.SubArchives.Length)
            {
                sub = archive.SubArchives[subIndex];
                AppendSubArchive(sub, fields, lookups);
            }
            _fields = fields.ToArray();
            _archiveLookups = lookups;
            _words = BuildWordsFromLookups(lookups);
            int comp = sub == null ? 0 : sub.SizeCompressed;
            int decomp = sub == null ? 0 : sub.SizeDecompressed;
            lblFields.Text = "Fields — sub-archive " + subIndex + " (records=" + _fields.Length
                + ", compressed=" + comp + ", decompressed=" + decomp + ")";
            lblStrings.Text = "Lookup strings — block " + subIndex + " (" + _words.Length + " words)";
        }

        private static void AppendSubArchive(BDAT_SUBARCHIVE sub, List<BDAT_FIELDTABLE> fields, List<BDAT_LOOKUPTABLE> lookups)
        {
            if (sub?.Fields == null)
            {
                return;
            }
            for (int i = 0; i < sub.Fields.Length; i++)
            {
                fields.Add(sub.Fields[i]);
                BDAT_LOOKUPTABLE lu = (sub.Lookups != null && i < sub.Lookups.Length) ? sub.Lookups[i] : null;
                lookups.Add(lu);
            }
        }

        private static string[] BuildWordsFromLookups(List<BDAT_LOOKUPTABLE> lookups)
        {
            var all = new List<string>();
            if (lookups == null)
            {
                return new string[0];
            }
            for (int i = 0; i < lookups.Count; i++)
            {
                var lu = lookups[i];
                if (lu == null || lu.Data == null || lu.Size <= 0)
                {
                    continue;
                }
                all.AddRange(bnsTool.LookupSplitToWords(lu.Data, (uint)lu.Size));
            }
            return all.ToArray();
        }

        private sealed class SubArchiveItem
        {
            public int Index { get; }
            public string Label { get; }

            public SubArchiveItem(int index, string label)
            {
                Index = index;
                Label = label;
            }

            public override string ToString()
            {
                return Label;
            }
        }

        private void ApplyFieldFilter()
        {
            string q = (txtFieldSearch.Text ?? string.Empty).Trim();
            var map = new List<int>();
            for (int i = 0; i < _fields.Length; i++)
            {
                var f = _fields[i];
                if (f == null)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(q))
                {
                    map.Add(i);
                    continue;
                }
                if (f.ID.ToString().IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                    || i.ToString().IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                    || FormatFieldData(f).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    map.Add(i);
                }
            }
            _fieldMap = map.ToArray();
            gridFields.RowCount = 0;
            gridFields.RowCount = _fieldMap.Length;
        }

        private void ApplyStringFilter()
        {
            string q = (txtStringSearch.Text ?? string.Empty).Trim();
            var map = new List<int>();
            for (int i = 0; i < _words.Length; i++)
            {
                string w = _words[i] ?? string.Empty;
                if (string.IsNullOrEmpty(q)
                    || w.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                    || i.ToString().IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    map.Add(i);
                }
            }
            _stringMap = map.ToArray();
            gridStrings.RowCount = 0;
            gridStrings.RowCount = _stringMap.Length;
        }

        private void txtFieldSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFieldFilter();
        }

        private void txtStringSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyStringFilter();
        }

        private static string FormatFieldData(BDAT_FIELDTABLE f)
        {
            if (f == null || f.Data == null || f.Data.Length == 0)
            {
                return string.Empty;
            }
            if (BinEditOptions.UseIntData)
            {
                return bcrypt.BytesToInt(f.Data, (uint)f.Data.Length);
            }
            return bcrypt.BytesToHex(f.Data, (uint)f.Data.Length);
        }

        private void gridFields_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _fieldMap.Length)
            {
                return;
            }
            int real = _fieldMap[e.RowIndex];
            if (real < 0 || real >= _fields.Length)
            {
                return;
            }
            var f = _fields[real];
            if (f == null)
            {
                return;
            }
            switch (e.ColumnIndex)
            {
                case 0: e.Value = real; break;
                case 1: e.Value = f.ID; break;
                case 2: e.Value = f.Size; break;
                case 3: e.Value = f.Unknown1; break;
                case 4: e.Value = f.Unknown2; break;
                case 5: e.Value = FormatFieldData(f); break;
            }
        }

        private void gridFields_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _fieldMap.Length)
            {
                return;
            }
            int real = _fieldMap[e.RowIndex];
            if (real < 0 || real >= _fields.Length)
            {
                return;
            }
            var f = _fields[real];
            if (f == null)
            {
                return;
            }

            string s = e.Value == null ? string.Empty : e.Value.ToString();
            try
            {
                // Snapshot for undo
                int oldId = f.ID, oldSize = f.Size, oldU1 = f.Unknown1, oldU2 = f.Unknown2;
                byte[] oldData = f.Data == null ? null : (byte[])f.Data.Clone();

                switch (e.ColumnIndex)
                {
                    case 1:
                        f.ID = int.Parse(s);
                        break;
                    case 2:
                        f.Size = int.Parse(s);
                        break;
                    case 3:
                        f.Unknown1 = int.Parse(s);
                        break;
                    case 4:
                        f.Unknown2 = int.Parse(s);
                        break;
                    case 5:
                        ApplyFieldDataEdit(f, s);
                        break;
                    default:
                        return;
                }

                if (oldId != f.ID || oldSize != f.Size || oldU1 != f.Unknown1 || oldU2 != f.Unknown2
                    || !BytesEqual(oldData, f.Data))
                {
                    var fieldRef = f;
                    byte[] newData = f.Data == null ? null : (byte[])f.Data.Clone();
                    int newId = f.ID, newSize = f.Size, newU1 = f.Unknown1, newU2 = f.Unknown2;
                    PushUndo(new LambdaUndo(
                        "edit field #" + real,
                        () =>
                        {
                            fieldRef.ID = oldId;
                            fieldRef.Size = oldSize;
                            fieldRef.Unknown1 = oldU1;
                            fieldRef.Unknown2 = oldU2;
                            fieldRef.Data = oldData == null ? null : (byte[])oldData.Clone();
                        },
                        () =>
                        {
                            fieldRef.ID = newId;
                            fieldRef.Size = newSize;
                            fieldRef.Unknown1 = newU1;
                            fieldRef.Unknown2 = newU2;
                            fieldRef.Data = newData == null ? null : (byte[])newData.Clone();
                        }));
                }

                _session.MarkDirty();
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Invalid value: " + ex.Message, "Edit field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static bool BytesEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static void ApplyFieldDataEdit(BDAT_FIELDTABLE f, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                f.Data = new byte[0];
                if (f.Size >= 12)
                {
                    f.Size = f.Unknown1 == 255 ? 10 : 12;
                }
                return;
            }

            byte[] data;
            if (BinEditOptions.UseIntData)
            {
                data = bcrypt.IntToBytes(text, 0);
            }
            else
            {
                string hex = text.Replace(" ", string.Empty).Replace("-", string.Empty);
                data = bcrypt.HexToBytes(hex, (uint)(hex.Length / 2));
            }
            f.Data = data ?? new byte[0];
            if (f.Unknown1 == 255)
            {
                int body = 4 + f.Data.Length;
                f.Size = Math.Max(6, 6 + body);
                if (f.Size < 12 && f.Data.Length == 0)
                {
                    f.Size = 6;
                }
            }
            else
            {
                f.Size = 12 + f.Data.Length;
            }
        }

        private void gridFields_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedField();
                e.Handled = true;
            }
        }

        private void gridStrings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedString();
                e.Handled = true;
            }
        }

        private void gridStrings_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _stringMap.Length)
            {
                return;
            }
            int real = _stringMap[e.RowIndex];
            if (real < 0 || real >= _words.Length)
            {
                return;
            }
            switch (e.ColumnIndex)
            {
                case 0: e.Value = real; break;
                case 1: e.Value = _words[real] ?? string.Empty; break;
            }
        }

        private void gridStrings_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _stringMap.Length)
            {
                return;
            }
            int real = _stringMap[e.RowIndex];
            if (real < 0 || real >= _words.Length)
            {
                return;
            }
            if (e.ColumnIndex != 1)
            {
                return;
            }
            string oldVal = _words[real] ?? string.Empty;
            string newVal = e.Value == null ? string.Empty : e.Value.ToString();
            if (oldVal == newVal)
            {
                return;
            }
            int idx = real;
            _words[idx] = newVal;
            PushUndo(new LambdaUndo(
                "edit string #" + idx,
                () => { _words[idx] = oldVal; _stringsDirty = true; },
                () => { _words[idx] = newVal; _stringsDirty = true; }));
            _stringsDirty = true;
            _session.MarkDirty();
            UpdateTitle();
        }

        // ----------------- Add / Delete -----------------

        private void AddField()
        {
            if (_selectedList == null || _busy)
            {
                return;
            }
            if (_isArchiveTable && _currentSubIndex < 0)
            {
                MessageBox.Show(this, "Select a single sub-archive block before adding a field (not \"All blocks\").",
                    "Add field", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var nf = new BDAT_FIELDTABLE
            {
                ID = 0,
                Unknown1 = 0,
                Unknown2 = 0,
                Size = 0,
                Data = new byte[0]
            };

            if (_isArchiveTable)
            {
                var sub = _currentArchive.SubArchives[_currentSubIndex];
                var fields = new List<BDAT_FIELDTABLE>(sub.Fields ?? new BDAT_FIELDTABLE[0]) { nf };
                var lookups = new List<BDAT_LOOKUPTABLE>(sub.Lookups ?? new BDAT_LOOKUPTABLE[0])
                {
                    new BDAT_LOOKUPTABLE { Size = 0, Data = new byte[0] }
                };
                var oldF = sub.Fields;
                var oldL = sub.Lookups;
                int oldCount = sub.FieldLookupCount;
                sub.Fields = fields.ToArray();
                sub.Lookups = lookups.ToArray();
                sub.FieldLookupCount = sub.Fields.Length;
                PushUndo(new LambdaUndo("add field",
                    () =>
                    {
                        sub.Fields = oldF;
                        sub.Lookups = oldL;
                        sub.FieldLookupCount = oldCount;
                    },
                    () =>
                    {
                        sub.Fields = fields.ToArray();
                        sub.Lookups = lookups.ToArray();
                        sub.FieldLookupCount = fields.Count;
                    }));
                LoadArchiveBlock(_currentArchive, _currentSubIndex);
            }
            else
            {
                var loose = _selectedList.Collection.Loose;
                var list = new List<BDAT_FIELDTABLE>(loose.Fields ?? new BDAT_FIELDTABLE[0]) { nf };
                var old = loose.Fields;
                int oldFc = loose.FieldCount;
                int oldUnf = loose.FieldCountUnfixed;
                loose.Fields = list.ToArray();
                loose.FieldCount = loose.Fields.Length;
                if (loose.FieldCountUnfixed < loose.FieldCount)
                {
                    loose.FieldCountUnfixed = loose.FieldCount;
                }
                PushUndo(new LambdaUndo("add field",
                    () =>
                    {
                        loose.Fields = old;
                        loose.FieldCount = oldFc;
                        loose.FieldCountUnfixed = oldUnf;
                    },
                    () =>
                    {
                        loose.Fields = list.ToArray();
                        loose.FieldCount = list.Count;
                        if (loose.FieldCountUnfixed < list.Count)
                        {
                            loose.FieldCountUnfixed = list.Count;
                        }
                    }));
                LoadLooseTable(loose);
            }
            ApplyFieldFilter();
            ApplyStringFilter();
            _session.MarkDirty();
            UpdateTitle();
            SetStatus("Added field");
        }

        private void DeleteSelectedField()
        {
            if (_selectedList == null || _busy || gridFields.CurrentCell == null)
            {
                return;
            }
            int row = gridFields.CurrentCell.RowIndex;
            if (row < 0 || row >= _fieldMap.Length)
            {
                return;
            }
            int real = _fieldMap[row];
            if (real < 0 || real >= _fields.Length)
            {
                return;
            }
            if (_isArchiveTable && _currentSubIndex < 0)
            {
                MessageBox.Show(this, "Select a single sub-archive block before deleting a field.",
                    "Delete field", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show(this, "Delete field #" + real + " (id=" + _fields[real].ID + ")?",
                    "Delete field", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (_isArchiveTable)
            {
                var sub = _currentArchive.SubArchives[_currentSubIndex];
                var fields = new List<BDAT_FIELDTABLE>(sub.Fields);
                var lookups = new List<BDAT_LOOKUPTABLE>(sub.Lookups ?? new BDAT_LOOKUPTABLE[fields.Count]);
                while (lookups.Count < fields.Count)
                {
                    lookups.Add(null);
                }
                var removedF = fields[real];
                var removedL = real < lookups.Count ? lookups[real] : null;
                var oldF = sub.Fields;
                var oldL = sub.Lookups;
                int oldCount = sub.FieldLookupCount;
                fields.RemoveAt(real);
                if (real < lookups.Count)
                {
                    lookups.RemoveAt(real);
                }
                sub.Fields = fields.ToArray();
                sub.Lookups = lookups.ToArray();
                sub.FieldLookupCount = sub.Fields.Length;
                PushUndo(new LambdaUndo("delete field #" + real,
                    () =>
                    {
                        sub.Fields = oldF;
                        sub.Lookups = oldL;
                        sub.FieldLookupCount = oldCount;
                    },
                    () =>
                    {
                        sub.Fields = fields.ToArray();
                        sub.Lookups = lookups.ToArray();
                        sub.FieldLookupCount = fields.Count;
                    }));
                LoadArchiveBlock(_currentArchive, _currentSubIndex);
            }
            else
            {
                var loose = _selectedList.Collection.Loose;
                var list = new List<BDAT_FIELDTABLE>(loose.Fields);
                var removed = list[real];
                var old = loose.Fields;
                int oldFc = loose.FieldCount;
                int oldUnf = loose.FieldCountUnfixed;
                list.RemoveAt(real);
                loose.Fields = list.ToArray();
                loose.FieldCount = loose.Fields.Length;
                PushUndo(new LambdaUndo("delete field #" + real,
                    () =>
                    {
                        loose.Fields = old;
                        loose.FieldCount = oldFc;
                        loose.FieldCountUnfixed = oldUnf;
                    },
                    () =>
                    {
                        loose.Fields = list.ToArray();
                        loose.FieldCount = list.Count;
                    }));
                LoadLooseTable(loose);
            }
            ApplyFieldFilter();
            ApplyStringFilter();
            _session.MarkDirty();
            UpdateTitle();
            SetStatus("Deleted field");
        }

        private void AddString()
        {
            if (_selectedList == null || _busy)
            {
                return;
            }
            var list = new List<string>(_words) { string.Empty };
            var old = _words;
            _words = list.ToArray();
            _stringsDirty = true;
            PushUndo(new LambdaUndo("add string",
                () => { _words = old; _stringsDirty = true; },
                () => { _words = list.ToArray(); _stringsDirty = true; }));
            ApplyStringFilter();
            _session.MarkDirty();
            UpdateTitle();
            SetStatus("Added string");
        }

        private void DeleteSelectedString()
        {
            if (_selectedList == null || _busy || gridStrings.CurrentCell == null)
            {
                return;
            }
            int row = gridStrings.CurrentCell.RowIndex;
            if (row < 0 || row >= _stringMap.Length)
            {
                return;
            }
            int real = _stringMap[row];
            if (real < 0 || real >= _words.Length)
            {
                return;
            }
            var list = new List<string>(_words);
            string removed = list[real];
            var old = _words;
            list.RemoveAt(real);
            _words = list.ToArray();
            _stringsDirty = true;
            PushUndo(new LambdaUndo("delete string #" + real,
                () => { _words = old; _stringsDirty = true; },
                () => { _words = list.ToArray(); _stringsDirty = true; }));
            ApplyStringFilter();
            _session.MarkDirty();
            UpdateTitle();
            SetStatus("Deleted string");
        }

        // ----------------- Search all / viewers -----------------

        private void ShowSearchAllDialog()
        {
            if (!_session.IsOpen)
            {
                MessageBox.Show(this, "Open a bin first.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var dlg = new Form())
            {
                dlg.Text = "Find in all tables";
                dlg.Width = 720;
                dlg.Height = 480;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MinimizeBox = false;

                var txt = new TextBox { Dock = DockStyle.Top, Height = 24 };
                var btn = new Button { Text = "Search", Dock = DockStyle.Top, Height = 28 };
                var list = new ListView
                {
                    Dock = DockStyle.Fill,
                    View = View.Details,
                    FullRowSelect = true,
                    HideSelection = false
                };
                list.Columns.Add("Table#", 50);
                list.Columns.Add("Id", 50);
                list.Columns.Add("Where", 70);
                list.Columns.Add("Detail", 480);

                void RunSearch()
                {
                    list.Items.Clear();
                    string q = (txt.Text ?? string.Empty).Trim();
                    if (q.Length == 0)
                    {
                        return;
                    }
                    FlushCurrentTableEdits();
                    for (int ti = 0; ti < _session.Content.ListCount; ti++)
                    {
                        var L = _session.Content.Lists[ti];
                        if (L.ID.ToString().IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var it = new ListViewItem(ti.ToString());
                            it.SubItems.Add(L.ID.ToString());
                            it.SubItems.Add("table");
                            it.SubItems.Add("table id match");
                            it.Tag = new SearchHit(ti, -1, -1, false);
                            list.Items.Add(it);
                        }
                        SearchTableContent(list, ti, L, q);
                        if (list.Items.Count > 500)
                        {
                            var cap = new ListViewItem("…");
                            cap.SubItems.Add("");
                            cap.SubItems.Add("");
                            cap.SubItems.Add("(stopped at 500 hits)");
                            list.Items.Add(cap);
                            break;
                        }
                    }
                    SetStatus("Search: " + list.Items.Count + " hit(s)");
                }

                btn.Click += (s, e) => RunSearch();
                txt.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        RunSearch();
                        e.SuppressKeyPress = true;
                    }
                };
                list.DoubleClick += (s, e) =>
                {
                    if (list.SelectedItems.Count == 0)
                    {
                        return;
                    }
                    if (list.SelectedItems[0].Tag is SearchHit hit)
                    {
                        dlg.Tag = hit;
                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                    }
                };

                dlg.Controls.Add(list);
                dlg.Controls.Add(btn);
                dlg.Controls.Add(txt);

                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.Tag is SearchHit go)
                {
                    NavigateToHit(go);
                }
            }
        }

        private void SearchTableContent(ListView list, int tableIndex, BDAT_LIST L, string q)
        {
            void Add(string where, string detail, int fieldIdx, int strIdx, bool isArchive)
            {
                var it = new ListViewItem(tableIndex.ToString());
                it.SubItems.Add(L.ID.ToString());
                it.SubItems.Add(where);
                it.SubItems.Add(detail);
                it.Tag = new SearchHit(tableIndex, fieldIdx, strIdx, isArchive);
                list.Items.Add(it);
            }

            if (L.Collection?.Loose != null)
            {
                var loose = L.Collection.Loose;
                if (loose.Fields != null)
                {
                    for (int i = 0; i < loose.Fields.Length; i++)
                    {
                        var f = loose.Fields[i];
                        if (f == null)
                        {
                            continue;
                        }
                        if (f.ID.ToString().IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                            || FormatFieldData(f).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Add("field", "#" + i + " id=" + f.ID, i, -1, false);
                        }
                    }
                }
                if (loose.Lookup?.Data != null && loose.Lookup.Size > 0)
                {
                    var words = bnsTool.LookupSplitToWords(loose.Lookup.Data, (uint)loose.Lookup.Size);
                    for (int i = 0; i < words.Count; i++)
                    {
                        if ((words[i] ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string preview = words[i] ?? "";
                            if (preview.Length > 80)
                            {
                                preview = preview.Substring(0, 80) + "…";
                            }
                            Add("string", "#" + i + " " + preview, -1, i, false);
                        }
                    }
                }
            }
            else if (L.Collection?.Archive?.SubArchives != null)
            {
                int globalField = 0;
                foreach (var sub in L.Collection.Archive.SubArchives)
                {
                    if (sub?.Fields == null)
                    {
                        continue;
                    }
                    for (int i = 0; i < sub.Fields.Length; i++)
                    {
                        var f = sub.Fields[i];
                        if (f != null && (f.ID.ToString().IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                            || FormatFieldData(f).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            Add("field", "merged#" + globalField + " id=" + f.ID, globalField, -1, true);
                        }
                        if (sub.Lookups != null && i < sub.Lookups.Length && sub.Lookups[i]?.Data != null)
                        {
                            var words = bnsTool.LookupSplitToWords(sub.Lookups[i].Data, (uint)sub.Lookups[i].Size);
                            for (int w = 0; w < words.Count; w++)
                            {
                                if ((words[w] ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    string preview = words[w] ?? "";
                                    if (preview.Length > 60)
                                    {
                                        preview = preview.Substring(0, 60) + "…";
                                    }
                                    Add("string", "field#" + globalField + " w" + w + " " + preview, globalField, w, true);
                                }
                            }
                        }
                        globalField++;
                    }
                }
            }
        }

        private void NavigateToHit(SearchHit hit)
        {
            // Select table in list
            foreach (ListViewItem item in listTables.Items)
            {
                if (item.Tag is int ti && ti == hit.TableIndex)
                {
                    listTables.SelectedItems.Clear();
                    item.Selected = true;
                    item.EnsureVisible();
                    break;
                }
            }
            // If filtered out, clear filter and retry
            if (listTables.SelectedItems.Count == 0)
            {
                txtTableFilter.Text = string.Empty;
                PopulateTableList();
                foreach (ListViewItem item in listTables.Items)
                {
                    if (item.Tag is int ti && ti == hit.TableIndex)
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                        break;
                    }
                }
            }
            Application.DoEvents();
            if (hit.IsArchive && comboSubArchive.Items.Count > 0)
            {
                // Show all blocks for archive hits
                _suppressSubArchiveSelect = true;
                comboSubArchive.SelectedIndex = 0;
                _suppressSubArchiveSelect = false;
                FlushCurrentTableEdits();
                ApplySelectedSubArchive();
            }
            if (hit.FieldIndex >= 0)
            {
                txtFieldSearch.Text = string.Empty;
                ApplyFieldFilter();
                for (int r = 0; r < _fieldMap.Length; r++)
                {
                    if (_fieldMap[r] == hit.FieldIndex)
                    {
                        gridFields.ClearSelection();
                        gridFields.CurrentCell = gridFields[1, r];
                        gridFields.FirstDisplayedScrollingRowIndex = Math.Max(0, r);
                        break;
                    }
                }
            }
            if (hit.StringIndex >= 0)
            {
                txtStringSearch.Text = string.Empty;
                ApplyStringFilter();
                for (int r = 0; r < _stringMap.Length; r++)
                {
                    if (_stringMap[r] == hit.StringIndex)
                    {
                        gridStrings.ClearSelection();
                        gridStrings.CurrentCell = gridStrings[1, r];
                        if (r < gridStrings.RowCount)
                        {
                            gridStrings.FirstDisplayedScrollingRowIndex = Math.Max(0, r);
                        }
                        break;
                    }
                }
            }
        }

        private sealed class SearchHit
        {
            public int TableIndex;
            public int FieldIndex;
            public int StringIndex;
            public bool IsArchive;
            public SearchHit(int t, int f, int s, bool a)
            {
                TableIndex = t;
                FieldIndex = f;
                StringIndex = s;
                IsArchive = a;
            }
        }

        private void ShowHeaderInfo()
        {
            if (!_session.IsOpen)
            {
                MessageBox.Show(this, "Open a bin first.", "Header", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var c = _session.Content;
            var sb = new StringBuilder();
            sb.AppendLine("Path: " + _session.FilePath);
            sb.AppendLine("Architecture: " + (_session.Is64Bit ? "64-bit" : "32-bit"));
            sb.AppendLine("Magic: " + Encoding.ASCII.GetString(c.Signature ?? new byte[0]));
            sb.AppendLine("ListCount: " + c.ListCount);
            if (c.HeadList != null)
            {
                sb.AppendLine("Head Size_1 (alias map): " + c.HeadList.Size_1);
                sb.AppendLine("Head Size_2: " + c.HeadList.Size_2);
                sb.AppendLine("Head Size_3: " + c.HeadList.Size_3);
                sb.AppendLine("Complement (no name table body): " + c.HeadList.Complement);
                sb.AppendLine("Name table Data length: " + (c.HeadList.Data == null ? 0 : c.HeadList.Data.Length));
            }
            MessageBox.Show(this, sb.ToString(), "File header", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowNameTable()
        {
            if (!_session.IsOpen)
            {
                MessageBox.Show(this, "Open a bin first.", "Name table", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var head = _session.Content.HeadList;
            if (head == null || head.Complement || head.Data == null || head.Data.Length == 0)
            {
                MessageBox.Show(this, "No name-table body in this file (typical for localfile / ListCount ≤ 10).",
                    "Name table", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string info = "Alias map Size_1=" + head.Size_1 + ", Size_2=" + head.Size_2
                + ", entries roughly " + (_session.Is64Bit ? "16" : "12") + " bytes each (opaque dump).";
            using (var f = new HexBlobForm("Name table / alias map", head.Data, info))
            {
                f.ShowDialog(this);
            }
        }

        private void ShowRegionTail()
        {
            if (_selectedList?.Collection?.Loose == null)
            {
                MessageBox.Show(this,
                    "Select a loose table.\n"
                    + "Region tail is the padding after the last field inside SizeFields (not used by archive tables).\n"
                    + "You can create one by saving non-empty hex if currently empty.",
                    "Region tail", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var loose = _selectedList.Collection.Loose;
            byte[] pad = loose.Padding ?? new byte[0];
            string info = "Table id=" + _selectedList.ID + " SizePadding=" + loose.SizePadding
                + " — advanced edit. Changes update SizeFields tail; list trailing pad is separate.";
            using (var f = new HexBlobForm("Region tail — table " + _selectedList.ID, pad, info, editable: true))
            {
                if (f.ShowDialog(this) != DialogResult.OK || f.ResultData == null)
                {
                    return;
                }
                byte[] oldPad = pad.Length == 0 ? new byte[0] : (byte[])pad.Clone();
                int oldSizePad = loose.SizePadding;
                byte[] newPad = f.ResultData;
                loose.Padding = newPad.Length == 0 ? null : newPad;
                loose.SizePadding = newPad.Length;
                // Keep SizeFields consistent with fields region + tail
                // SizeFields is recalculated on write from fields+padding; mark dirty is enough
                PushUndo(new LambdaUndo(
                    "edit region tail",
                    () =>
                    {
                        loose.Padding = oldPad.Length == 0 ? null : (byte[])oldPad.Clone();
                        loose.SizePadding = oldSizePad;
                    },
                    () =>
                    {
                        loose.Padding = newPad.Length == 0 ? null : (byte[])newPad.Clone();
                        loose.SizePadding = newPad.Length;
                    }));
                _session.MarkDirty();
                UpdateTitle();
                SetStatus("Region tail updated (" + newPad.Length + " bytes)");
            }
        }

        // ----------------- Sub-archive blocks -----------------

        private void AddSubArchiveBlock()
        {
            if (!_isArchiveTable || _currentArchive == null || _busy)
            {
                MessageBox.Show(this, "Select an archive table first.", "Add block",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            FlushCurrentTableEdits();
            var archive = _currentArchive;
            var oldSubs = archive.SubArchives;
            int oldCount = archive.SubArchiveCount;
            var list = new List<BDAT_SUBARCHIVE>(oldSubs ?? new BDAT_SUBARCHIVE[0])
            {
                BDAT_SUBARCHIVE.CreateEmpty()
            };
            archive.SubArchives = list.ToArray();
            archive.SubArchiveCount = list.Count;
            int newIndex = list.Count - 1;
            PushUndo(new LambdaUndo(
                "add sub-archive block",
                () =>
                {
                    archive.SubArchives = oldSubs;
                    archive.SubArchiveCount = oldCount;
                },
                () =>
                {
                    archive.SubArchives = list.ToArray();
                    archive.SubArchiveCount = list.Count;
                }));
            _session.MarkDirty();
            UpdateTitle();
            SetupArchiveSubCombo(archive);
            // Select the new block (index in combo = newIndex + 1 because of "All" at 0)
            _suppressSubArchiveSelect = true;
            if (comboSubArchive.Items.Count > newIndex + 1)
            {
                comboSubArchive.SelectedIndex = newIndex + 1;
            }
            _suppressSubArchiveSelect = false;
            ApplySelectedSubArchive();
            UpdateStatusForCurrentTable();
            // Refresh table list record counts
            PopulateTableList();
            SetStatus("Added sub-archive block " + newIndex);
        }

        private void DeleteSubArchiveBlock()
        {
            if (!_isArchiveTable || _currentArchive == null || _busy)
            {
                return;
            }
            if (_currentSubIndex < 0)
            {
                MessageBox.Show(this, "Select a single sub-archive block to delete (not \"All blocks\").",
                    "Delete block", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var archive = _currentArchive;
            if (archive.SubArchives == null || archive.SubArchives.Length == 0)
            {
                return;
            }
            if (archive.SubArchives.Length == 1)
            {
                if (MessageBox.Show(this,
                        "This is the last block. Delete it anyway? (A new empty block can be added later.)",
                        "Delete block", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }
            else if (MessageBox.Show(this,
                    "Delete sub-archive block " + _currentSubIndex + " ("
                    + (archive.SubArchives[_currentSubIndex].Fields?.Length ?? 0) + " records)?",
                    "Delete block", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            FlushCurrentTableEdits();
            int delIndex = _currentSubIndex;
            var oldSubs = archive.SubArchives;
            int oldCount = archive.SubArchiveCount;
            var list = new List<BDAT_SUBARCHIVE>(oldSubs);
            list.RemoveAt(delIndex);
            archive.SubArchives = list.ToArray();
            archive.SubArchiveCount = list.Count;
            PushUndo(new LambdaUndo(
                "delete sub-archive block " + delIndex,
                () =>
                {
                    archive.SubArchives = oldSubs;
                    archive.SubArchiveCount = oldCount;
                },
                () =>
                {
                    archive.SubArchives = list.ToArray();
                    archive.SubArchiveCount = list.Count;
                }));
            _session.MarkDirty();
            UpdateTitle();
            SetupArchiveSubCombo(archive);
            PopulateTableList();
            SetStatus("Deleted sub-archive block " + delIndex);
        }

        private sealed class LambdaUndo : IEditorUndoAction
        {
            private readonly Action _undo;
            private readonly Action _redo;
            public string Description { get; }
            public LambdaUndo(string desc, Action undo, Action redo)
            {
                Description = desc;
                _undo = undo;
                _redo = redo;
            }
            public void Undo() => _undo?.Invoke();
            public void Redo() => _redo?.Invoke();
        }

        /// <summary>
        /// Commit string edits back into BDAT lookup / archive lookups before save or table switch.
        /// </summary>
        private void FlushCurrentTableEdits()
        {
            if (_selectedList == null)
            {
                return;
            }
            // End edit on grids
            gridFields.EndEdit();
            gridStrings.EndEdit();

            if (!_stringsDirty)
            {
                return;
            }

            if (_isArchiveTable)
            {
                // Rebuild lookups currently in view (one block or merged). Same object refs as on BDAT_SUBARCHIVE.
                RebuildArchiveLookupsFromWords();
            }
            else if (_selectedList.Collection?.Loose != null)
            {
                var loose = _selectedList.Collection.Loose;
                if (loose.Lookup == null)
                {
                    loose.Lookup = new BDAT_LOOKUPTABLE();
                }
                int sizeLookup = 0;
                loose.Lookup.Data = bnsTool.WordToLookUpData(_words, ref sizeLookup);
                loose.Lookup.Size = sizeLookup;
                loose.SizeLookup = sizeLookup;
            }
            _stringsDirty = false;
        }

        private void RebuildArchiveLookupsFromWords()
        {
            if (_archiveLookups == null)
            {
                return;
            }
            // Per-field word counts before edit (from existing lookup blobs)
            var counts = new int[_archiveLookups.Count];
            int total = 0;
            for (int i = 0; i < _archiveLookups.Count; i++)
            {
                var lu = _archiveLookups[i];
                if (lu == null || lu.Data == null || lu.Size <= 0)
                {
                    counts[i] = 0;
                }
                else
                {
                    counts[i] = bnsTool.LookupSplitToWords(lu.Data, (uint)lu.Size).Count;
                }
                total += counts[i];
            }

            int offset = 0;
            if (total == _words.Length && total > 0)
            {
                for (int i = 0; i < _archiveLookups.Count; i++)
                {
                    if (counts[i] == 0)
                    {
                        continue;
                    }
                    var slice = new string[counts[i]];
                    Array.Copy(_words, offset, slice, 0, counts[i]);
                    offset += counts[i];
                    int sz = 0;
                    byte[] data = bnsTool.WordToLookUpData(slice, ref sz);
                    if (_archiveLookups[i] == null)
                    {
                        _archiveLookups[i] = new BDAT_LOOKUPTABLE();
                    }
                    _archiveLookups[i].Data = data;
                    _archiveLookups[i].Size = sz;
                }
            }
            else if (_words.Length > 0)
            {
                // Word count changed: assign whole list to the first non-null lookup slot
                for (int i = 0; i < _archiveLookups.Count; i++)
                {
                    if (_archiveLookups[i] == null && i < _fields.Length)
                    {
                        _archiveLookups[i] = new BDAT_LOOKUPTABLE();
                    }
                    if (_archiveLookups[i] == null)
                    {
                        continue;
                    }
                    int sz = 0;
                    _archiveLookups[i].Data = bnsTool.WordToLookUpData(_words, ref sz);
                    _archiveLookups[i].Size = sz;
                    break;
                }
            }
        }
    }
}
