using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Pilao
{
    public static class DatFilesMan
    {
        public static void Backup()
        {
            string FilePath = Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.pak";
            File.Copy(FilePath, FilePath.Replace(".pak", ".bak"), true);
            Form1.CurrentForm.SortOutputHandler("Backed up Pak.");
        }

        public static void Restore()
        {
            string FilePath = Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.bak";
            File.Copy(FilePath, FilePath.Replace(".bak", ".pak"), true);
            File.Delete(FilePath);
            Form1.CurrentForm.SortOutputHandler("Restored Pak.");
        }
    }
}
