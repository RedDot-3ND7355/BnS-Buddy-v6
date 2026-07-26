using System.IO;
using System.Text;

namespace BnSBuddy2.INI
{
    public static class INIHandler
    {
        // Read INI File
        public static string ReadINIFile(string filepath)
        {
            return File.ReadAllText(filepath, Encoding.UTF8);
        }

        // Save INI File
        public static void SaveINIFile(string filepath, string text) =>
            File.WriteAllText(filepath, text, Encoding.UTF8);
    }
}
