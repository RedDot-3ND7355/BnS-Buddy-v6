using MaterialSkin;
using System;
using System.Collections.Generic;

namespace BnSBuddy2.Settings
{
    class SetValues
    {
        public SetValues(Dictionary<string, string> pairs)
        {
            Dictionary<string, string> _pairs = new Dictionary<string,string>(pairs);
            // Set colors via argb int parsers
            Form1.CurrentForm.materialSkinManager.ColorScheme = new ColorScheme((Primary)Convert.ToInt32(_pairs["primary"].ToString()), (Primary)Convert.ToInt32(_pairs["darkprimary"].ToString()), (Primary)Convert.ToInt32(_pairs["lightprimary"].ToString()), (Accent)Convert.ToInt32(_pairs["accent"].ToString()), (TextShade)Convert.ToInt32(_pairs["textshade"].ToString()));
            Form1.CurrentForm.materialSkinManager.Theme = _pairs["theme"].ToString() == "DARK" ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            Form1.CurrentForm.Refresh();
            if (_pairs["theme"].ToString() == "LIGHT")
                Form1.CurrentForm.materialSwitch2.Checked = false;
            if (_pairs["useallcores"].ToString() == "true")
                Form1.CurrentForm.materialCheckbox1.Checked = true;
            if (_pairs["unattended"].ToString() == "true")
                Form1.CurrentForm.materialCheckbox2.Checked = true;
            if (_pairs["notexturestreaming"].ToString() == "true")
                Form1.CurrentForm.materialCheckbox3.Checked = true;
            if (_pairs["savelogs"].ToString() == "true")
                Form1.CurrentForm.materialSwitch36.Checked = true;
            if (_pairs["launcherlogs"].ToString() == "true")
                Form1.CurrentForm.materialSwitch17.Checked = true;
            if (_pairs["modmanlogs"].ToString() == "true")
                Form1.CurrentForm.materialSwitch35.Checked = true;
            if (_pairs["timestamplogs"].ToString() == "true")
                Form1.CurrentForm.materialSwitch44.Checked = true;
            if (_pairs["admincheck"].ToString() == "false")
                Form1.CurrentForm.materialSwitch38.Checked = false;
            if (_pairs["minimize"].ToString() == "false")
                Form1.CurrentForm.materialSwitch39.Checked = false;
            if (_pairs["showlogs"].ToString() == "false")
                Form1.CurrentForm.materialSwitch37.Checked = false;
            if (_pairs["gameversioncheck"].ToString() == "false")
                Form1.CurrentForm.materialSwitch47.Checked = false;
            if (_pairs["maintenancecheck"].ToString() == "false")
                Form1.CurrentForm.materialSwitch48.Checked = false;
            if (_pairs["langset"].ToString() == "true")
                Form1.CurrentForm.langremembered = _pairs["langpath"].ToString();
            string text8 = _pairs["garenaclientpath"].ToString();
            if (text8.Length > 0)
                Form1.CurrentForm.GarenaDefPath = text8;
            string text = _pairs["customclientname"].ToString();
            if (_pairs["customclientname"].ToString().Length >= 1)
                Form1.CurrentForm.materialTextBox5.Text = _pairs["customclientname"].ToString();
            if (_pairs["customgame"].ToString() == "true")
            {
                Form1.CurrentForm.materialButton51.Enabled = true;
                Form1.CurrentForm.materialButton48.Enabled = true;
                Form1.CurrentForm.materialButton45.Enabled = true;
                Form1.CurrentForm.materialTextBox1.Enabled = true;
                Form1.CurrentForm.materialSwitch15.Checked = true;
                Form1.CurrentForm.materialTextBox1.Text = _pairs["customgamepath"].ToString();
            }
            if (_pairs["customclient"].ToString() == "true")
            {
                Form1.CurrentForm.materialButton53.Enabled = true;
                Form1.CurrentForm.materialButton49.Enabled = true;
                Form1.CurrentForm.materialButton46.Enabled = true;
                Form1.CurrentForm.materialTextBox2.Enabled = true;
                Form1.CurrentForm.materialSwitch16.Checked = true;
                Form1.CurrentForm.materialTextBox2.Text = _pairs["customclientpath"].ToString();
            }
            if (_pairs["gamekiller"].ToString() == "false")
                Form1.CurrentForm.materialSwitch40.Checked = false;
            if (_pairs["boostprocess"].ToString() == "true")
                Form1.CurrentForm.materialSwitch53.Checked = true;
            if (_pairs["updatechecker"].ToString() == "false")
                Form1.CurrentForm.materialSwitch57.Checked = false;
            if (_pairs["pingchecker"].ToString() == "false")
                Form1.CurrentForm.materialSwitch41.Checked = false;
            Form1.CurrentForm.materialTextBox4.Text = _pairs["arguments"].ToString();
            if (_pairs["automemorycleanup"].ToString() == "true")
                Form1.CurrentForm.materialSwitch52.Checked = true;
            Form1.CurrentForm.materialLabel64.Text = _pairs["prtime"].ToString();
            if (Convert.ToInt32(Form1.CurrentForm.materialLabel64.Text) < 1000)
                Form1.CurrentForm.materialLabel64.Text = "5000";
            Form1.CurrentForm.materialScrollBar1.Value = Convert.ToInt32(Form1.CurrentForm.materialLabel64.Text);
            if (_pairs["cleanint"].ToString() != "OFF")
                Form1.CurrentForm.materialComboBox7.SelectedIndex = Form1.CurrentForm.materialComboBox7.FindStringExact(_pairs["cleanint"].ToString());
            else
                Form1.CurrentForm.materialComboBox7.SelectedIndex = 0;
            if (_pairs["defaultset"].ToString() == "true")
                Form1.CurrentForm.materialTextBox3.Text = _pairs["_default"].ToString();
            if (_pairs["usercountcheck"].ToString() == "false")
                Form1.CurrentForm.materialSwitch58.Checked = false;
            if (_pairs["showcount"].ToString() == "false")
                Form1.CurrentForm.materialSwitch59.Checked = true;
            if (_pairs["autoupdate"].ToString() == "false")
                Form1.CurrentForm.materialSwitch42.Checked = false;
            if (_pairs["rememberme"].ToString() == "true")
                Form1.CurrentForm.materialSwitch50.Checked = true;
            Form1.CurrentForm.materialComboBox8.SelectedIndex = Form1.CurrentForm.materialComboBox8.FindStringExact(_pairs["priority"].ToString());
            if (_pairs["modfolderset"].ToString() == "true")
            {
                Form1.CurrentForm.materialSwitch51.Checked = true;
                Form1.CurrentForm.materialButton54.Enabled = true;
                Form1.CurrentForm.materialButton52.Enabled = true;
                Form1.CurrentForm.materialButton55.Enabled = true;
                Form1.CurrentForm.materialTextBox6.Text = _pairs["modfolder"].ToString();
            }
            if (_pairs["gcdshow"].ToString() == "true")
                Form1.CurrentForm.materialSwitch55.Checked = true;
            if (_pairs["autologin"].ToString() == "true")
                Form1.CurrentForm.materialSwitch49.Checked = true;
            Form1.CurrentForm.materialLabel88.Text += _pairs["affinityproc"];
            if (_pairs["affinityproc"].ToString() == "Custom")
                Form1.CurrentForm.customValue = int.Parse(_pairs["customaffinity"].ToString());
            if (_pairs["keepintray"].ToString() == "true")
                Form1.CurrentForm.materialSwitch43.Checked = true;
            if (_pairs["startwWindows"].ToString() == "true")
                Form1.CurrentForm.materialSwitch45.Checked = true;
            if (_pairs["affinityman"].ToString() == "false")
                Form1.CurrentForm.materialSwitch54.Checked = false;
            if (_pairs["autogamecatcher"].ToString() == "false")
                Form1.CurrentForm.materialSwitch46.Checked = false;
            Form1.CurrentForm.readids = _pairs["readmessagesid"].ToString();
            Form1.CurrentForm.Inboxer.Routine();
            if (Form1.CurrentForm.Inboxer.IDMessagePairs.Count > 0)
            {
                string tmp_inb = "Message";
                if (Form1.CurrentForm.Inboxer.IDMessagePairs.Count > 1)
                    tmp_inb += "s";
                Form1.CurrentForm.FlashInbox.Enabled = true;
                Form1.CurrentForm.materialButton57.Text = $"{Form1.CurrentForm.Inboxer.IDMessagePairs.Count} {tmp_inb}.";
            }
        }
    }
}
