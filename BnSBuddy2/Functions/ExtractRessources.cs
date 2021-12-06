using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Functions
{
    public static class ExtractRessources
    {
        public static byte[] ExtractRessource(byte[] zippedBuffer)
        {
            MemoryStream stream = new MemoryStream(zippedBuffer);
            using (Ionic.Zip.ZipFile z = Ionic.Zip.ZipFile.Read(stream))
            {
                MemoryStream TempArrray = new MemoryStream();
                z[0].Extract(TempArrray);
                return TempArrray.ToArray();
            }
        }
    }
}
