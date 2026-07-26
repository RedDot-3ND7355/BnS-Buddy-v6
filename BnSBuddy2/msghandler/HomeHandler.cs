using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;

namespace BnSBuddy2.msghandler
{
    public static class HomeHandler
    {
        public static void InboxAction(bool IsOpened)
        {
            if (!IsOpened) // Open
            {
                Form1.CurrentForm.FlashInbox.Enabled = false;
                Form1.CurrentForm.materialButton57.HighEmphasis = true;
                Form1.CurrentForm.materialButton57.Text = "X";
                Form1.CurrentForm.materialListBox2.Visible = true;
                Form1.CurrentForm.materialCard1.Size = new Size(634, 551);
                Form1.CurrentForm.materialCard1.Location = new Point(274, 32);
            }
            else // Close
            {
                string tmp = "Message";
                if (Form1.CurrentForm.Inboxer.IDMessagePairs.Count - Form1.CurrentForm.Inboxer.ReadIDs.Count > 1)
                    tmp += "s";
                if (Form1.CurrentForm.Inboxer.IDMessagePairs.Count - Form1.CurrentForm.Inboxer.ReadIDs.Count >= 1)
                    Form1.CurrentForm.FlashInbox.Enabled = true;
                Form1.CurrentForm.materialButton57.Text = $"{Form1.CurrentForm.Inboxer.IDMessagePairs.Count - Form1.CurrentForm.Inboxer.ReadIDs.Count} {tmp}";
                Form1.CurrentForm.materialMultiLineTextBox21.Text = "";
                Form1.CurrentForm.materialButton60.Visible = false;
                Form1.CurrentForm.materialListBox2.Visible = false;
                Form1.CurrentForm.materialCard1.Size = new Size(894, 551);
                Form1.CurrentForm.materialCard1.Location = new Point(14, 32);
            }
        }

        public static void OpenSelectedInbox(MaterialListBoxItem SelectedInbox)
        {
            SelectedInbox.Text = SelectedInbox.Text.Replace(" (unread)", "");
            Form1.CurrentForm.materialMultiLineTextBox21.Text = Form1.CurrentForm.Inboxer.IDMessagePairs[Form1.CurrentForm.Inboxer.TitleIDPairs[SelectedInbox.Text.Replace(" (unreead)", "")]];
            int CurrID = Form1.CurrentForm.Inboxer.TitleIDPairs[SelectedInbox.Text.Replace(" (unreead)", "")];
            if (!Form1.CurrentForm.routine.CurrentSettings["readmessagesid"].Contains("," + CurrID + ",") && !Form1.CurrentForm.routine.CurrentSettings["readmessagesid"].StartsWith(CurrID + ","))
                Form1.CurrentForm.routineChanger("readmessagesid", Form1.CurrentForm.routine.CurrentSettings["readmessagesid"] + CurrID + ",");
            Form1.CurrentForm.Inboxer.ReadID(CurrID);
        }
    }
}
