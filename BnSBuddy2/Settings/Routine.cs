using System.Collections.Generic;

namespace BnSBuddy2.Settings
{
    public class Routine
    {
        public Dictionary<string, string> CurrentSettings = new Dictionary<string, string>();
        ConfigParser parser = new ConfigParser();
        SetValues values;

        public void _Routine()
        {
            // Convert if old found
            parser.Convert();
            // Create if missing
            parser.NewSettings();
            // Fix if needed
            parser.CheckCorrupt();
            // Update if outdated
            parser.Update();
            // Get CurrentSettings
            CurrentSettings = parser.Get();
            // Set Values
            values = new SetValues(CurrentSettings);
        }

        public Dictionary<string, string> _OnlyColors()
        {
            return parser.Get();
        }

        public void ResetSettings() =>
            parser.NewSettings(true);

        public void SaveSettings() =>
            parser.Save(CurrentSettings);
    }
}
