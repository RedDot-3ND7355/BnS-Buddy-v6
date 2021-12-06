using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin;

namespace BnSBuddy2.Settings
{
    public class Default
    {
        public string value()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?><settings><setting name=\"primary\">-16748412</setting><setting name=\"darkprimary\">-16751002</setting><setting name=\"lightprimary\">-16748412</setting><setting name=\"accent\">-16733753</setting><setting name=\"textshade\">-1</setting><setting name=\"theme\">DARK</setting><setting name=\"unattended\">false</setting><setting name=\"notexturestreaming\">false</setting><setting name=\"savelogs\">false</setting><setting name=\"showlogs\">true</setting><setting name=\"customgame\">false</setting><setting name=\"customclient\">false</setting><setting name=\"admincheck\">true</setting><setting name=\"ncsoftlogin\">false</setting><setting name=\"showdonate\">true</setting><setting name=\"minimize\">true</setting><setting name=\"launcherlogs\">false</setting><setting name=\"modmanlogs\">false</setting><setting name=\"customclientpath\"></setting><setting name=\"customgamepath\"></setting><setting name=\"updatechecker\">true</setting><setting name=\"pingchecker\">true</setting><setting name=\"gamekiller\">true</setting><setting name=\"useallcores\">false</setting><setting name=\"arguments\"></setting><setting name=\"prtime\">5000</setting><setting name=\"autoupdate\">true</setting><setting name=\"firsttime\">true</setting><setting name=\"_default\"></setting><setting name=\"defaultset\">false</setting><setting name=\"defaultclient\"></setting><setting name=\"priority\">Normal</setting><setting name=\"modfolder\"></setting><setting name=\"modfolderset\">false</setting><setting name=\"rememberme\">false</setting><setting name=\"automemorycleanup\">false</setting><setting name=\"langset\">false</setting><setting name=\"langpath\"></setting><setting name=\"boostprocess\">true</setting><setting name=\"cleanint\">OFF</setting><setting name=\"uniquepass\">UNUSED</setting><setting name=\"gcdshow\">false</setting><setting name=\"igpshow\">false</setting><setting name=\"autologin\">false</setting><setting name=\"usercountcheck\">true</setting><setting name=\"showcount\">true</setting><setting name=\"customclientname\"></setting><setting name=\"lastserver\"></setting><setting name=\"affinityproc\">All</setting><setting name=\"keepintray\">false</setting><setting name=\"startwWindows\">false</setting><setting name=\"customaffinity\">0</setting><setting name=\"affinityman\">true</setting><setting name=\"gameversioncheck\">true</setting><setting name=\"usesystemproxy\">false</setting><setting name=\"maintenancecheck\">true</setting><setting name=\"timestamplogs\">false</setting><setting name=\"garenaclientpath\"></setting><setting name=\"dx912check\">true</setting><setting name=\"autogamecatcher\">true</setting><setting name=\"licenseemail\"></setting><setting name=\"readmessagesid\"></setting></settings>";
        }

        public void colors()
        {
            // Reset colors
            Form1.CurrentForm.materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            Form1.CurrentForm.materialSkinManager.ColorScheme = new ColorScheme((Primary)0x007084, (Primary)0x006666, (Primary)0x007084, (Accent)0x00A9C7, TextShade.WHITE);
            // Save changes
            Form1.CurrentForm.routineChanger("primary", Form1.CurrentForm.materialSkinManager.ColorScheme.PrimaryColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("darkprimary", Form1.CurrentForm.materialSkinManager.ColorScheme.DarkPrimaryColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("lightprimary", Form1.CurrentForm.materialSkinManager.ColorScheme.LightPrimaryColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("accent", Form1.CurrentForm.materialSkinManager.ColorScheme.AccentColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("textshade", Form1.CurrentForm.materialSkinManager.ColorScheme.TextColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("theme", Form1.CurrentForm.materialSkinManager.Theme.ToString());
            Form1.CurrentForm.Refresh();
        }
    }
}
