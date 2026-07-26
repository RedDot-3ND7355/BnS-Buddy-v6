using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.LegacyBin
{
    /// <summary>
    /// localfile translation UI (export / merge / apply) — BnsDatTool-compatible Translation.xml.
    /// </summary>
    public sealed class TranslateForm : Form
    {
        private readonly BinSession _session;
        private readonly Action _onApplied;

        private TextBox _txtSourceXml;
        private TextBox _txtTargetXml;
        private TextBox _txtLog;
        private NumericUpDown _numTable;
        private CheckBox _chkAutoTable;
        private CheckBox _chkResplit;
        private Label _lblStatus;

        public TranslateForm(BinSession session, Action onApplied = null)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _onApplied = onApplied;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Localfile Translate";
            Width = 720;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(10)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Text = _session.IsOpen
                    ? ("Open bin: " + Path.GetFileName(_session.FilePath) + (_session.Is64Bit ? " [64-bit]" : " [32-bit]"))
                    : "No bin open in editor — Apply requires an open localfile."
            };
            root.Controls.Add(_lblStatus, 0, 0);

            // Source XML row
            var rowSrc = MakePathRow("Source / merge-from XML:", out _txtSourceXml, out var btnSrc);
            btnSrc.Click += (s, e) => BrowseXml(_txtSourceXml, save: false);
            root.Controls.Add(rowSrc, 0, 1);

            // Target XML row
            var rowTgt = MakePathRow("Target XML (export/merge into):", out _txtTargetXml, out var btnTgt);
            btnTgt.Click += (s, e) => BrowseXml(_txtTargetXml, save: false);
            root.Controls.Add(rowTgt, 0, 2);

            // Options
            var opts = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            _chkAutoTable = new CheckBox { Text = "Auto-detect text table", Checked = true, AutoSize = true, Margin = new Padding(0, 6, 12, 0) };
            _numTable = new NumericUpDown { Minimum = 0, Maximum = 9999, Width = 70, Enabled = false };
            _chkAutoTable.CheckedChanged += (s, e) => _numTable.Enabled = !_chkAutoTable.Checked;
            _chkResplit = new CheckBox
            {
                Text = "Split oversized blocks after apply",
                Checked = true,
                AutoSize = true,
                Margin = new Padding(12, 6, 0, 0)
            };
            opts.Controls.Add(_chkAutoTable);
            opts.Controls.Add(new Label { Text = "Table index:", AutoSize = true, Margin = new Padding(0, 8, 4, 0) });
            opts.Controls.Add(_numTable);
            opts.Controls.Add(_chkResplit);
            root.Controls.Add(opts, 0, 3);

            // Buttons
            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            var btnExport = new Button { Text = "Export XML from open bin", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            var btnMerge = new Button { Text = "Merge XMLs by alias", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            var btnApply = new Button { Text = "Apply XML → open bin", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            var btnClose = new Button { Text = "Close", AutoSize = true, DialogResult = DialogResult.Cancel };
            btnExport.Click += async (s, e) => await ExportAsync();
            btnMerge.Click += async (s, e) => await MergeAsync();
            btnApply.Click += async (s, e) => await ApplyAsync();
            buttons.Controls.Add(btnExport);
            buttons.Controls.Add(btnMerge);
            buttons.Controls.Add(btnApply);
            buttons.Controls.Add(btnClose);
            root.Controls.Add(buttons, 0, 4);

            _txtLog = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new System.Drawing.Font("Consolas", 9f)
            };
            root.Controls.Add(_txtLog, 0, 5);

            Controls.Add(root);
            CancelButton = btnClose;
            UiTheme.Apply(this);

            Log("Workflow (BnsDatTool-compatible):");
            Log("1) Open localfile / localfile64 in the editor.");
            Log("2) Export Translation XML from this bin (aliases + original text).");
            Log("3) Optionally Merge with another region's Translation XML (by alias).");
            Log("4) Apply the Translation XML into the open bin, then Save.");
            Log("");
            if (_session.IsOpen)
            {
                try
                {
                    int idx = LocalfileTranslation.FindTextTableIndex(_session.Content);
                    if (idx >= 0)
                    {
                        var L = _session.Content.Lists[idx];
                        Log("Detected text table: index=" + idx + " id=" + L.ID + " size=" + L.Size);
                        _numTable.Value = idx;
                    }
                    else
                    {
                        Log("Warning: could not auto-detect text table.");
                    }
                }
                catch (Exception ex)
                {
                    Log("Detect error: " + ex.Message);
                }
            }
        }

        private static Panel MakePathRow(string label, out TextBox textBox, out Button browse)
        {
            var p = new Panel { Dock = DockStyle.Fill, Height = 32 };
            var lbl = new Label { Text = label, AutoSize = true, Location = new System.Drawing.Point(0, 6) };
            textBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top, Location = new System.Drawing.Point(0, 24), Width = 580 };
            // simpler single line layout
            p.Controls.Clear();
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            var l = new Label { Text = label, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            textBox = new TextBox { Dock = DockStyle.Fill };
            browse = new Button { Text = "Browse…", Dock = DockStyle.Fill };
            layout.Controls.Add(l, 0, 0);
            layout.Controls.Add(textBox, 1, 0);
            layout.Controls.Add(browse, 2, 0);
            p.Controls.Add(layout);
            return p;
        }

        private void BrowseXml(TextBox target, bool save)
        {
            if (save)
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Translation XML (*.xml)|*.xml|All files (*.*)|*.*";
                    dlg.FileName = _session.Is64Bit ? "Translation64.xml" : "Translation.xml";
                    DialogPaths.Apply(dlg);
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        DialogPaths.RememberFile(dlg.FileName);
                        target.Text = dlg.FileName;
                    }
                }
            }
            else
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Translation XML (*.xml)|*.xml|All files (*.*)|*.*";
                    DialogPaths.Apply(dlg);
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        DialogPaths.RememberFile(dlg.FileName);
                        target.Text = dlg.FileName;
                    }
                }
            }
        }

        private void Log(string msg)
        {
            _txtLog.AppendText(msg + Environment.NewLine);
        }

        private int ResolveTableIndex()
        {
            if (_chkAutoTable.Checked)
            {
                return LocalfileTranslation.FindTextTableIndex(_session.Content);
            }
            return (int)_numTable.Value;
        }

        private async Task ExportAsync()
        {
            if (!_session.IsOpen)
            {
                MessageBox.Show(this, "Open a localfile bin in the editor first.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Translation XML (*.xml)|*.xml";
                dlg.FileName = _session.Is64Bit ? "Translation64.xml" : "Translation.xml";
                DialogPaths.Apply(dlg);
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                DialogPaths.RememberFile(dlg.FileName);
                string path = dlg.FileName;
                UseWaitCursor = true;
                try
                {
                    List<LocalfileTranslation.Entry> entries = null;
                    int tableIndex = ResolveTableIndex();
                    await Task.Run(() =>
                    {
                        entries = LocalfileTranslation.ExportFromContent(_session.Content, tableIndex);
                        LocalfileTranslation.SaveXml(path, entries);
                    });
                    _txtTargetXml.Text = path;
                    Log("Exported " + entries.Count + " entries → " + path + " (table " + tableIndex + ")");
                    MessageBox.Show(this, "Exported " + entries.Count + " text entries.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Log("Export failed: " + ex.Message);
                    MessageBox.Show(this, ex.ToString(), "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UseWaitCursor = false;
                }
            }
        }

        private async Task MergeAsync()
        {
            string source = _txtSourceXml.Text.Trim();
            string target = _txtTargetXml.Text.Trim();
            if (!File.Exists(source) || !File.Exists(target))
            {
                MessageBox.Show(this, "Select existing Source and Target Translation XML files.\n"
                    + "Source = language you want to pull from\nTarget = structure to keep (usually current client export).",
                    "Merge", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Translation XML (*.xml)|*.xml";
                dlg.FileName = Path.GetFileNameWithoutExtension(target) + "_merged.xml";
                DialogPaths.Apply(dlg);
                string targetDir = Path.GetDirectoryName(target);
                if (!string.IsNullOrEmpty(targetDir) && Directory.Exists(targetDir))
                {
                    dlg.InitialDirectory = targetDir;
                }
                if (dlg.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                DialogPaths.RememberFile(dlg.FileName);
                string outPath = dlg.FileName;
                UseWaitCursor = true;
                try
                {
                    int count = 0;
                    await Task.Run(() =>
                    {
                        var src = LocalfileTranslation.LoadXml(source);
                        var tgt = LocalfileTranslation.LoadXml(target);
                        var merged = LocalfileTranslation.MergeByAlias(tgt, src);
                        LocalfileTranslation.SaveXml(outPath, merged);
                        count = merged.Count;
                    });
                    _txtTargetXml.Text = outPath;
                    Log("Merged " + count + " entries → " + outPath);
                    MessageBox.Show(this, "Merge complete.\n" + outPath, "Merge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Log("Merge failed: " + ex.Message);
                    MessageBox.Show(this, ex.ToString(), "Merge failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UseWaitCursor = false;
                }
            }
        }

        private async Task ApplyAsync()
        {
            if (!_session.IsOpen)
            {
                MessageBox.Show(this, "Open a localfile bin in the editor first.", "Apply", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string xmlPath = _txtTargetXml.Text.Trim();
            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
            {
                xmlPath = _txtSourceXml.Text.Trim();
            }
            if (!File.Exists(xmlPath))
            {
                MessageBox.Show(this, "Select a Translation XML to apply (Target preferred, else Source).",
                    "Apply", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(this,
                    "Apply translations from:\n" + xmlPath + "\n\nto the open bin in memory?\n(You still need to Save the bin afterward.)",
                    "Apply translation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            UseWaitCursor = true;
            try
            {
                LocalfileTranslation.ApplyResult result = null;
                int tableIndex = ResolveTableIndex();
                bool resplit = _chkResplit.Checked;
                await Task.Run(() =>
                {
                    var entries = LocalfileTranslation.LoadXml(xmlPath);
                    result = LocalfileTranslation.Apply(_session.Content, entries, tableIndex, resplit);
                });
                _session.MarkDirty();
                _onApplied?.Invoke();
                var sb = new StringBuilder();
                sb.AppendLine(result.TextTableSummary);
                sb.AppendLine("Records scanned: " + result.RecordsScanned);
                sb.AppendLine("Applied by alias: " + result.AppliedByAlias);
                sb.AppendLine("Applied by original text: " + result.AppliedByOriginal);
                sb.AppendLine("Unchanged: " + result.Unchanged);
                sb.AppendLine("Skipped: " + result.Skipped);
                sb.AppendLine("Blocks split: " + result.BlocksSplit);
                Log(sb.ToString());
                MessageBox.Show(this, sb.ToString() + "\nRemember to File → Save the bin.", "Apply complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log("Apply failed: " + ex.Message);
                MessageBox.Show(this, ex.ToString(), "Apply failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }
    }
}
