using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BnSBuddy2.LegacyBin
{
    /// <summary>
    /// Simple app-wide light/dark theming for standard WinForms controls.
    /// (MaterialSkin Form1 already has its own dark theme.)
    /// </summary>
    public static class UiTheme
    {
        public static bool DarkMode
        {
            get => BinEditOptions.DarkMode;
            set
            {
                BinEditOptions.DarkMode = value;
                foreach (Form f in Application.OpenForms)
                {
                    Apply(f);
                }
            }
        }

        // Dark palette
        public static Color DarkBack => Color.FromArgb(32, 32, 36);
        public static Color DarkBackAlt => Color.FromArgb(45, 45, 50);
        public static Color DarkBackInput => Color.FromArgb(28, 28, 32);
        public static Color DarkFore => Color.FromArgb(230, 230, 235);
        public static Color DarkForeMuted => Color.FromArgb(170, 170, 180);
        public static Color DarkBorder => Color.FromArgb(70, 70, 78);
        public static Color DarkSelect => Color.FromArgb(55, 70, 110);
        public static Color DarkHeader => Color.FromArgb(40, 40, 48);
        public static Color DarkMenu => Color.FromArgb(38, 38, 44);
        public static Color DarkGridLine => Color.FromArgb(60, 60, 68);

        // Light palette
        public static Color LightBack => SystemColors.Control;
        public static Color LightBackAlt => SystemColors.Window;
        public static Color LightFore => SystemColors.ControlText;

        public static void Apply(Control root)
        {
            if (root == null)
            {
                return;
            }
            // MaterialSkin forms manage themselves
            if (root.GetType().FullName != null
                && root.GetType().FullName.StartsWith("MaterialSkin", System.StringComparison.Ordinal))
            {
                return;
            }
            if (root is MaterialSkin.Controls.MaterialForm || root.GetType().Name == "Form1")
            {
                return;
            }

            if (DarkMode)
            {
                ApplyDark(root);
            }
            else
            {
                ApplyLight(root);
            }
        }

        private static void ApplyDark(Control c)
        {
            if (c is Form form)
            {
                form.BackColor = DarkBack;
                form.ForeColor = DarkFore;
            }
            else if (c is MenuStrip ms)
            {
                ms.BackColor = DarkMenu;
                ms.ForeColor = DarkFore;
                ms.Renderer = new DarkMenuRenderer();
                foreach (ToolStripItem item in ms.Items)
                {
                    StyleToolStripItem(item, dark: true);
                }
            }
            else if (c is StatusStrip ss)
            {
                ss.BackColor = DarkMenu;
                ss.ForeColor = DarkForeMuted;
                ss.Renderer = new DarkMenuRenderer();
                foreach (ToolStripItem item in ss.Items)
                {
                    item.ForeColor = DarkForeMuted;
                }
            }
            else if (c is DataGridView dgv)
            {
                StyleDataGridView(dgv, dark: true);
            }
            else if (c is ListView lv)
            {
                StyleListView(lv, dark: true);
            }
            else if (c is TextBox || c is RichTextBox || c is ComboBox || c is NumericUpDown)
            {
                c.BackColor = DarkBackInput;
                c.ForeColor = DarkFore;
                if (c is TextBox tb)
                {
                    tb.BorderStyle = BorderStyle.FixedSingle;
                }
            }
            else if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = DarkBorder;
                btn.BackColor = DarkBackAlt;
                btn.ForeColor = DarkFore;
                btn.UseVisualStyleBackColor = false;
            }
            else if (c is Label || c is CheckBox)
            {
                c.ForeColor = DarkFore;
                // keep transparent-ish on parent
                if (!(c.Parent is FlowLayoutPanel) && !(c.Parent is TableLayoutPanel))
                {
                    c.BackColor = Color.Transparent;
                }
                else
                {
                    c.BackColor = c.Parent.BackColor;
                }
            }
            else if (c is Panel || c is SplitContainer || c is SplitterPanel
                     || c is FlowLayoutPanel || c is TableLayoutPanel || c is GroupBox)
            {
                c.BackColor = DarkBack;
                c.ForeColor = DarkFore;
            }
            else
            {
                try
                {
                    c.BackColor = DarkBack;
                    c.ForeColor = DarkFore;
                }
                catch
                {
                    // some controls reject colors
                }
            }

            foreach (Control child in c.Controls)
            {
                ApplyDark(child);
            }

            // SplitContainer panels
            if (c is SplitContainer sc)
            {
                ApplyDark(sc.Panel1);
                ApplyDark(sc.Panel2);
            }
        }

        private static void ApplyLight(Control c)
        {
            if (c is Form form)
            {
                form.BackColor = LightBack;
                form.ForeColor = LightFore;
            }
            else if (c is MenuStrip ms)
            {
                ms.BackColor = SystemColors.MenuBar;
                ms.ForeColor = SystemColors.MenuText;
                ms.Renderer = new ToolStripProfessionalRenderer();
                foreach (ToolStripItem item in ms.Items)
                {
                    StyleToolStripItem(item, dark: false);
                }
            }
            else if (c is StatusStrip ss)
            {
                ss.BackColor = SystemColors.Control;
                ss.ForeColor = SystemColors.ControlText;
                ss.Renderer = new ToolStripProfessionalRenderer();
            }
            else if (c is DataGridView dgv)
            {
                StyleDataGridView(dgv, dark: false);
            }
            else if (c is ListView lv)
            {
                StyleListView(lv, dark: false);
            }
            else if (c is TextBox || c is RichTextBox || c is ComboBox || c is NumericUpDown)
            {
                c.BackColor = SystemColors.Window;
                c.ForeColor = SystemColors.WindowText;
            }
            else if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Standard;
                btn.UseVisualStyleBackColor = true;
                btn.BackColor = SystemColors.Control;
                btn.ForeColor = SystemColors.ControlText;
            }
            else if (c is Label || c is CheckBox)
            {
                c.ForeColor = SystemColors.ControlText;
                c.BackColor = Color.Transparent;
            }
            else if (c is Panel || c is SplitContainer || c is FlowLayoutPanel || c is TableLayoutPanel)
            {
                c.BackColor = LightBack;
                c.ForeColor = LightFore;
            }

            foreach (Control child in c.Controls)
            {
                ApplyLight(child);
            }
            if (c is SplitContainer sc)
            {
                ApplyLight(sc.Panel1);
                ApplyLight(sc.Panel2);
            }
        }

        private static void StyleDataGridView(DataGridView dgv, bool dark)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.FixedSingle;

            // VirtualMode + AlternatingRowsDefaultCellStyle often leaves odd rows white with light text.
            // Force every cell via CellFormatting (hook once).
            if (dgv.Tag as string != "ui-theme-dgv")
            {
                dgv.CellFormatting += DataGridView_CellFormatting;
                dgv.Tag = "ui-theme-dgv";
            }

            if (dark)
            {
                dgv.BackgroundColor = DarkBackInput;
                dgv.GridColor = DarkGridLine;

                // Same colors for default + alternating so nothing falls back to system white
                var cell = new DataGridViewCellStyle
                {
                    BackColor = DarkBackInput,
                    ForeColor = DarkFore,
                    SelectionBackColor = DarkSelect,
                    SelectionForeColor = DarkFore
                };
                dgv.DefaultCellStyle = cell;
                dgv.RowsDefaultCellStyle = cell.Clone() as DataGridViewCellStyle;
                dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = DarkBackAlt,
                    ForeColor = DarkFore,
                    SelectionBackColor = DarkSelect,
                    SelectionForeColor = DarkFore
                };
                dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = DarkHeader,
                    ForeColor = DarkFore,
                    SelectionBackColor = DarkHeader,
                    SelectionForeColor = DarkFore
                };
                dgv.RowHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = DarkHeader,
                    ForeColor = DarkForeMuted,
                    SelectionBackColor = DarkSelect,
                    SelectionForeColor = DarkFore
                };
                dgv.RowTemplate.DefaultCellStyle = cell.Clone() as DataGridViewCellStyle;
            }
            else
            {
                dgv.BackgroundColor = SystemColors.Window;
                dgv.GridColor = SystemColors.ControlDark;
                dgv.DefaultCellStyle = new DataGridViewCellStyle();
                dgv.RowsDefaultCellStyle = new DataGridViewCellStyle();
                dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle();
                dgv.RowHeadersDefaultCellStyle = new DataGridViewCellStyle();
                dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle();
                dgv.RowTemplate.DefaultCellStyle = new DataGridViewCellStyle();
                dgv.EnableHeadersVisualStyles = true;
            }

            dgv.Invalidate();
        }

        private static void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!(sender is DataGridView dgv) || e.RowIndex < 0)
            {
                return;
            }

            if (!DarkMode)
            {
                return;
            }

            // Always force readable dark-mode colors (including VirtualMode rows)
            bool alt = (e.RowIndex % 2) == 1;
            e.CellStyle.BackColor = alt ? DarkBackAlt : DarkBackInput;
            e.CellStyle.ForeColor = DarkFore;
            e.CellStyle.SelectionBackColor = DarkSelect;
            e.CellStyle.SelectionForeColor = DarkFore;
        }

        /// <summary>
        /// ListView Details mode ignores BackColor on column headers — use owner-draw.
        /// Also paints the residual header strip past the last column (otherwise stays white).
        /// </summary>
        private static void StyleListView(ListView lv, bool dark)
        {
            lv.BackColor = dark ? DarkBackInput : SystemColors.Window;
            lv.ForeColor = dark ? DarkFore : SystemColors.WindowText;
            lv.BorderStyle = BorderStyle.FixedSingle;

            // Attach owner-draw handlers once
            if (lv.Tag as string != "ui-theme-listview")
            {
                lv.DrawColumnHeader += ListView_DrawColumnHeader;
                lv.DrawItem += ListView_DrawItem;
                lv.DrawSubItem += ListView_DrawSubItem;
                lv.Resize += ListView_ResizeFillLastColumn;
                // Paint empty client area / header remainder
                lv.HandleCreated += (s, e) => ListViewHeaderNative.Attach((ListView)s);
                if (lv.IsHandleCreated)
                {
                    ListViewHeaderNative.Attach(lv);
                }
                lv.Tag = "ui-theme-listview";
            }

            lv.OwnerDraw = true; // handlers fall back to default when not dark
            FillLastColumn(lv);
            lv.Invalidate(true);
            ListViewHeaderNative.InvalidateHeader(lv);
        }

        private static void ListView_ResizeFillLastColumn(object sender, EventArgs e)
        {
            if (sender is ListView lv)
            {
                FillLastColumn(lv);
            }
        }

        /// <summary>Stretch the last column so no empty white gap sits after Size.</summary>
        private static void FillLastColumn(ListView lv)
        {
            if (lv.View != View.Details || lv.Columns.Count == 0)
            {
                return;
            }
            int used = 0;
            for (int i = 0; i < lv.Columns.Count - 1; i++)
            {
                used += lv.Columns[i].Width;
            }
            // Account for vertical scrollbar / borders
            int avail = lv.ClientSize.Width - used - 4;
            if (SystemInformation.VerticalScrollBarWidth > 0 && lv.Items.Count > 0)
            {
                // leave a little room; ListView client width already excludes scrollbar when visible
            }
            if (avail < 40)
            {
                avail = 40;
            }
            if (lv.Columns[lv.Columns.Count - 1].Width != avail)
            {
                lv.Columns[lv.Columns.Count - 1].Width = avail;
            }
        }

        private static void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (!DarkMode)
            {
                e.DrawDefault = true;
                return;
            }

            using (var bg = new SolidBrush(DarkHeader))
            {
                e.Graphics.FillRectangle(bg, e.Bounds);
            }
            using (var pen = new Pen(DarkBorder))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                e.Graphics.DrawLine(pen, e.Bounds.Right - 1, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom);
            }

            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
            var textRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, Math.Max(0, e.Bounds.Width - 6), e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font ?? SystemFonts.MessageBoxFont, textRect, DarkFore, flags);
        }

        /// <summary>
        /// Subclasses the ListView header HWND so leftover header area past the last column is painted dark.
        /// </summary>
        private sealed class ListViewHeaderNative : NativeWindow
        {
            private static readonly System.Collections.Generic.Dictionary<IntPtr, ListViewHeaderNative> Map
                = new System.Collections.Generic.Dictionary<IntPtr, ListViewHeaderNative>();

            private readonly ListView _listView;

            private ListViewHeaderNative(ListView listView)
            {
                _listView = listView;
            }

            public static void Attach(ListView lv)
            {
                if (lv == null || !lv.IsHandleCreated)
                {
                    return;
                }
                IntPtr header = SendMessage(lv.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
                if (header == IntPtr.Zero)
                {
                    return;
                }
                if (Map.ContainsKey(header))
                {
                    return;
                }
                var hook = new ListViewHeaderNative(lv);
                hook.AssignHandle(header);
                Map[header] = hook;
                lv.HandleDestroyed += (s, e) =>
                {
                    try
                    {
                        if (Map.TryGetValue(header, out var h))
                        {
                            h.ReleaseHandle();
                            Map.Remove(header);
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                };
            }

            public static void InvalidateHeader(ListView lv)
            {
                if (lv == null || !lv.IsHandleCreated)
                {
                    return;
                }
                IntPtr header = SendMessage(lv.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
                if (header != IntPtr.Zero)
                {
                    InvalidateRect(header, IntPtr.Zero, true);
                }
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (!DarkMode)
                {
                    return;
                }
                // After default paint, fill any residual area past columns
                if (m.Msg == WM_PAINT || m.Msg == WM_NCPAINT || m.Msg == WM_ERASEBKGND)
                {
                    try
                    {
                        PaintResidualHeader();
                    }
                    catch
                    {
                        // ignore paint failures
                    }
                }
            }

            private void PaintResidualHeader()
            {
                if (_listView.Columns.Count == 0)
                {
                    return;
                }
                int colsWidth = 0;
                foreach (ColumnHeader col in _listView.Columns)
                {
                    colsWidth += col.Width;
                }
                RECT rc;
                GetClientRect(Handle, out rc);
                if (colsWidth >= rc.Right)
                {
                    return;
                }
                using (var g = Graphics.FromHwnd(Handle))
                {
                    var remainder = Rectangle.FromLTRB(colsWidth, 0, rc.Right, rc.Bottom);
                    using (var brush = new SolidBrush(DarkHeader))
                    {
                        g.FillRectangle(brush, remainder);
                    }
                }
            }

            private const int LVM_FIRST = 0x1000;
            private const int LVM_GETHEADER = LVM_FIRST + 31;
            private const int WM_PAINT = 0x000F;
            private const int WM_NCPAINT = 0x0085;
            private const int WM_ERASEBKGND = 0x0014;

            [DllImport("user32.dll")]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll")]
            private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

            [StructLayout(LayoutKind.Sequential)]
            private struct RECT
            {
                public int Left, Top, Right, Bottom;
            }
        }

        private static void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (!DarkMode)
            {
                e.DrawDefault = true;
                return;
            }

            // Details view paints cells in DrawSubItem; still fill background here for non-Details.
            var lv = (ListView)sender;
            if (lv.View != View.Details)
            {
                bool selected = e.Item.Selected;
                Color bg = selected ? DarkSelect : DarkBackInput;
                using (var brush = new SolidBrush(bg))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
                TextRenderer.DrawText(
                    e.Graphics,
                    e.Item.Text,
                    e.Item.Font ?? lv.Font,
                    new Rectangle(e.Bounds.X + 3, e.Bounds.Y, e.Bounds.Width - 3, e.Bounds.Height),
                    DarkFore,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private static void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (!DarkMode)
            {
                e.DrawDefault = true;
                return;
            }

            var lv = (ListView)sender;
            bool selected = e.Item.Selected;
            // Use a single dark row color (no light alternating fallback)
            Color bg = selected ? DarkSelect : DarkBackInput;

            using (var brush = new SolidBrush(bg))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            string text = e.SubItem != null ? e.SubItem.Text : e.Item.Text;
            var textRect = new Rectangle(e.Bounds.X + 3, e.Bounds.Y, Math.Max(0, e.Bounds.Width - 4), e.Bounds.Height);
            TextRenderer.DrawText(
                e.Graphics,
                text ?? string.Empty,
                e.SubItem?.Font ?? e.Item.Font ?? lv.Font,
                textRect,
                DarkFore,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        private static void StyleToolStripItem(ToolStripItem item, bool dark)
        {
            item.ForeColor = dark ? DarkFore : SystemColors.MenuText;
            if (item is ToolStripMenuItem mi)
            {
                foreach (ToolStripItem sub in mi.DropDownItems)
                {
                    StyleToolStripItem(sub, dark);
                }
            }
        }

        private sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
        {
            public DarkMenuRenderer() : base(new DarkColorTable())
            {
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = DarkFore;
                base.OnRenderItemText(e);
            }
        }

        private sealed class DarkColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin => DarkMenu;
            public override Color MenuStripGradientEnd => DarkMenu;
            public override Color MenuItemSelected => DarkSelect;
            public override Color MenuItemSelectedGradientBegin => DarkSelect;
            public override Color MenuItemSelectedGradientEnd => DarkSelect;
            public override Color MenuItemBorder => DarkBorder;
            public override Color MenuBorder => DarkBorder;
            public override Color MenuItemPressedGradientBegin => DarkBackAlt;
            public override Color MenuItemPressedGradientEnd => DarkBackAlt;
            public override Color ToolStripDropDownBackground => DarkMenu;
            public override Color ImageMarginGradientBegin => DarkMenu;
            public override Color ImageMarginGradientMiddle => DarkMenu;
            public override Color ImageMarginGradientEnd => DarkMenu;
            public override Color SeparatorDark => DarkBorder;
            public override Color SeparatorLight => DarkBorder;
            public override Color StatusStripGradientBegin => DarkMenu;
            public override Color StatusStripGradientEnd => DarkMenu;
            public override Color ButtonSelectedHighlight => DarkSelect;
            public override Color ButtonSelectedBorder => DarkBorder;
        }
    }
}
