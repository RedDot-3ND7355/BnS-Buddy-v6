using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Threading;
using System.Windows.Forms;

namespace BnSBuddy2.Pages
{
    public partial class enterCode : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        Packets.NCHandler f1;
        public bool called = false;
        // End Globals

        public enterCode(Packets.NCHandler f)
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            //
            f1 = f;
            materialTextBox1.ContextMenu = new ContextMenu();
        }

        private void materialTextBox1_TextChanged(object sender, EventArgs e) =>
            materialButton1.Enabled = (materialTextBox1.Text.Length == 6);

        private void materialTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (materialTextBox1.Text.Length == 6)
                if (e.KeyValue == (char)Keys.Return)
                    f1.submitCode();
            if ((e.KeyValue >= '0' && e.KeyValue <= '9') || e.KeyValue == (Char)Keys.Delete || e.KeyValue == (Char)Keys.Back || e.Control && e.KeyValue == (Char)Keys.V || e.KeyValue >= 96 && e.KeyValue <= 105 || e.KeyValue == 37 || e.KeyValue == 39 || (e.Control && e.KeyValue == (Char)Keys.A)) //The  character represents a backspace
                e.SuppressKeyPress = false;
            else
                e.SuppressKeyPress = true;
        }

        private void materialButton1_Click(object sender, EventArgs e) =>
            f1.submitCode();

        private void enterCode_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !called)
                f1.termConnection();
        }

        private void materialTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Thread task = new Thread(() => { materialTextBox1.Text = Clipboard.GetText(); });
                task.SetApartmentState(ApartmentState.STA);
                task.IsBackground = false;
                task.Start();
            }
        }
    }
}
