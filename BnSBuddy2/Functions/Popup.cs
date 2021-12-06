using MaterialSkin;
using System;

namespace BnSBuddy2.Functions
{
    public partial class Popup : MaterialSkin.Controls.MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public Popup()
        {
            InitializeComponent();
            // Ini material
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            // Get Color & Theme
            try
            {
                materialSkinManager.Theme = Form1.CurrentForm.routine.CurrentSettings["theme"] == "DARK" ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
                materialSkinManager.ColorScheme = new ColorScheme((Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["primary"]), (Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["darkprimary"]), (Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["lightprimary"]), (Accent)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["accent"]), (TextShade)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["textshade"]));
            }
            catch { }
        }
    }
}
