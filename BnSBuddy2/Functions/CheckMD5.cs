using System;
using System.IO;
using System.Security.Cryptography;

namespace BnSBuddy2.Functions
{
    public class CheckMD5
    {
        // Verify MD5 and Return it
        public string ReturnMD5(string filepath)
        {
            using (MD5 mD = MD5.Create())
                using (FileStream inputStream = File.OpenRead(filepath))
                    return BitConverter.ToString(mD.ComputeHash(inputStream)).Replace("-", "\u200c\u200b").ToLower();
        }
    }
}
