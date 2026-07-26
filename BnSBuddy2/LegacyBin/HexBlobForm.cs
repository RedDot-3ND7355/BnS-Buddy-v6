using System;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace BnSBuddy2.LegacyBin
{
    /// <summary>
    /// Hex viewer/editor for opaque blobs (name table, region tail, etc.).
    /// Edit mode accepts continuous hex or a formatted dump; Save returns parsed bytes via DialogResult.OK.
    /// </summary>
    public sealed class HexBlobForm : Form
    {
        private readonly TextBox _text;
        private readonly Label _info;
        private readonly bool _editable;

        /// <summary>Result when DialogResult is OK in editable mode.</summary>
        public byte[] ResultData { get; private set; }

        public HexBlobForm(string title, byte[] data, string infoLine = null, bool editable = false)
        {
            _editable = editable;
            Text = title ?? "Hex view";
            StartPosition = FormStartPosition.CenterParent;
            Width = 920;
            Height = 620;
            MinimizeBox = false;

            _info = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Text = (infoLine ?? ("Length: " + (data == null ? 0 : data.Length) + " bytes"))
                    + (editable ? "  |  Edit hex (spaces/newlines OK). Save applies changes." : "")
            };

            _text = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = !editable,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Font = new System.Drawing.Font("Consolas", 9f),
                Text = editable ? FormatHexContinuous(data) : FormatHex(data),
                AcceptsReturn = true,
                AcceptsTab = true
            };

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8, 6, 8, 6)
            };

            var btnClose = new Button { Text = editable ? "Cancel" : "Close", AutoSize = true, DialogResult = DialogResult.Cancel };
            bottom.Controls.Add(btnClose);

            if (editable)
            {
                var btnSave = new Button { Text = "Save", AutoSize = true };
                btnSave.Click += (s, e) =>
                {
                    try
                    {
                        ResultData = ParseHexFlexible(_text.Text);
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Invalid hex:\n" + ex.Message, "Hex edit",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
                bottom.Controls.Add(btnSave);

                var btnDump = new Button { Text = "Pretty dump", AutoSize = true };
                btnDump.Click += (s, e) =>
                {
                    try
                    {
                        byte[] b = ParseHexFlexible(_text.Text);
                        _text.Text = FormatHex(b);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Pretty dump", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
                bottom.Controls.Add(btnDump);

                var btnRaw = new Button { Text = "Compact hex", AutoSize = true };
                btnRaw.Click += (s, e) =>
                {
                    try
                    {
                        byte[] b = ParseHexFlexible(_text.Text);
                        _text.Text = FormatHexContinuous(b);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Compact hex", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };
                bottom.Controls.Add(btnRaw);
            }

            Controls.Add(_text);
            Controls.Add(bottom);
            Controls.Add(_info);
            CancelButton = btnClose;
            UiTheme.Apply(this);
        }

        public static string FormatHexContinuous(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder(data.Length * 3);
            for (int i = 0; i < data.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(i % 16 == 0 ? "\r\n" : " ");
                }
                sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string FormatHex(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return "(empty)";
            }
            var sb = new StringBuilder(data.Length * 4);
            for (int i = 0; i < data.Length; i += 16)
            {
                sb.Append(i.ToString("X8")).Append("  ");
                var ascii = new StringBuilder(16);
                for (int j = 0; j < 16; j++)
                {
                    if (i + j < data.Length)
                    {
                        byte b = data[i + j];
                        sb.Append(b.ToString("X2")).Append(' ');
                        ascii.Append(b >= 32 && b < 127 ? (char)b : '.');
                    }
                    else
                    {
                        sb.Append("   ");
                        ascii.Append(' ');
                    }
                    if (j == 7)
                    {
                        sb.Append(' ');
                    }
                }
                sb.Append(" |").Append(ascii).Append('|').AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parse continuous hex or pretty dump lines (offset + hex + optional |ascii|).
        /// </summary>
        public static byte[] ParseHexFlexible(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "(empty)")
            {
                return new byte[0];
            }
            var hex = new StringBuilder();
            foreach (string rawLine in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                string line = rawLine.Trim();
                if (line.Length == 0)
                {
                    continue;
                }
                // Strip |ascii| tail
                int pipe = line.IndexOf('|');
                if (pipe >= 0)
                {
                    line = line.Substring(0, pipe).TrimEnd();
                }
                // Skip leading offset column (8 hex digits + spaces)
                if (line.Length >= 10 && IsHexRun(line, 0, 8) && line[8] == ' ')
                {
                    line = line.Substring(8).TrimStart();
                }
                foreach (char c in line)
                {
                    if (char.IsWhiteSpace(c) || c == '-')
                    {
                        continue;
                    }
                    if (Uri.IsHexDigit(c))
                    {
                        hex.Append(c);
                    }
                    else
                    {
                        throw new FormatException("Unexpected character '" + c + "' in hex data.");
                    }
                }
            }
            if (hex.Length % 2 != 0)
            {
                throw new FormatException("Odd number of hex digits (" + hex.Length + ").");
            }
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = byte.Parse(hex.ToString(i * 2, 2), NumberStyles.HexNumber);
            }
            return result;
        }

        private static bool IsHexRun(string s, int start, int len)
        {
            if (start + len > s.Length)
            {
                return false;
            }
            for (int i = 0; i < len; i++)
            {
                if (!Uri.IsHexDigit(s[start + i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
