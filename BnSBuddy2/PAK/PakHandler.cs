using BnSBuddy2.Functions;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BnSBuddy2.PAK
{
    public static class PakHandler
    {
        // Global
        static string TempPath = Path.GetTempPath();
        static string WorkingPath = "";
        public static Dictionary<string, string> FileList = new Dictionary<string, string>();
        public static string ExtractedDir = "";
        // End Globals

        private static void IniPak()
        {
            File.WriteAllBytes(TempPath + "quickbms.exe", Properties.Resources.quickbms);
            File.WriteAllBytes(TempPath + "BNS.bms", Properties.Resources.BNS);
            File.WriteAllBytes(TempPath + "UnrealPak.zip", Properties.Resources.UnrealPak);
            WorkingPath = TempPath + "QuickBMS";
            using (ZipFile zip = ZipFile.Read(TempPath + "UnrealPak.zip"))
                zip.ExtractAll(WorkingPath, ExtractExistingFileAction.OverwriteSilently);
        }

        private static void CleanPak()
        {
            File.Delete(TempPath + "quickbms.exe");
            File.Delete(TempPath + "BNS.bms");
            File.Delete(TempPath + "UnrealPak.zip");
            Directory.Delete(WorkingPath, true);
            WorkingPath = "";
        }


        public static void PakUnpack(string file)
        {
            if (!File.Exists(file))
                file = file.Replace("Pak0-Local.pak", "Pak0-Local_p.pak");
            Form1.CurrentForm.SortOutputHandler("Unpacking Pak...");
            Form1.CurrentForm.materialComboBox5.Items.Clear();
            FileList = new Dictionary<string, string>();
            IniPak();
            // QuickBMS Unpack
            string ExtractedPath = "";
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            Process process = new Process();
            processStartInfo.WorkingDirectory = WorkingPath;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            process.StartInfo = processStartInfo;
            process.Start();
            if (file != "")
            {
                int length = file.IndexOf('.');
                string text = file.Substring(0, length);
                string text2 = $"{TempPath}quickbms.exe \"{TempPath}BNS.bms\" \"" + file + "\" \"" + text + "\"";
                ExtractedPath = Form1.CurrentForm.PakPaths.PakPath + new DirectoryInfo(text).Name;
                ExtractedDir = ExtractedPath;
                DirectoryInfo directoryInfo = new DirectoryInfo(text);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();
                process.StandardInput.Write(text2 + Environment.NewLine);
                process.StandardInput.Close();
                process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }
            else
            {
                process.StandardInput.Close();
                process.Close();
            }
            // Populate Combobox & FileList
            string[] files = Directory.GetFiles(ExtractedPath, "*.dat", SearchOption.AllDirectories);
            foreach (string datfile in files)
            {
                Form1.CurrentForm.materialComboBox5.Items.Add(Path.GetFileName(datfile));
                FileList.Add(Path.GetFileName(datfile), datfile);
            }
            //
            CleanPak();
            Form1.CurrentForm.SortOutputHandler("Unpacked.");
        }

        public static void PakRepack(string folder)
        {
            Form1.CurrentForm.SortOutputHandler("Repacking Pak...");
            IniPak();
            // UnrealPak Repack
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            Process process = new Process();
            processStartInfo.WorkingDirectory = WorkingPath;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            process.StartInfo = processStartInfo;
            process.Start();
            if (folder != "")
            {
                string text2 = "@setlocal enableextensions";
                process.StandardInput.Write(text2 + Environment.NewLine);
                text2 = "@pushd " + WorkingPath;
                process.StandardInput.Write(text2 + Environment.NewLine);
                text2 = "@echo \"" + Path.GetDirectoryName(folder) + "\\" + Path.GetFileName(folder) + $"\\*.*\" \"..\\..\\..\\*.*\" >filelist.txt";
                process.StandardInput.Write(text2 + Environment.NewLine);
                string tmp = folder.Contains("_p") ? ".pak" : "_p.pak";
                text2 = $"{WorkingPath}\\UnrealPak\\Engine\\Binaries\\Win64\\UnrealPak.exe \"" + folder + $"{tmp}\" -create=\"{WorkingPath}\\filelist.txt\" -compress -compressionformat=\"oodle\" ";
                process.StandardInput.Write(text2 + Environment.NewLine);
                text2 = "@popd";
                process.StandardInput.Write(text2 + Environment.NewLine);
                text2 = "exit";
                process.StandardInput.Write(text2 + Environment.NewLine);
                process.WaitForExit();
                process.Close();
                if (File.Exists(WorkingPath + "\\filelist.txt"))
                {
                    try
                    {
                        File.Delete(WorkingPath + "\\filelist.txt");
                    }
                    catch (IOException ex)
                    {
                        Prompt.Popup(ex.ToString());
                    }
                }
            }
            else
            {
                process.StandardInput.Close();
                process.Close();
            }
            //
            CleanPak();
            // Clean tree, combobox & fastcoloredtextbox
            Form1.CurrentForm.fastColoredTextBox1.Text = "";
            Form1.CurrentForm.materialComboBox5.Items.Clear();
            Form1.CurrentForm.treeView1.Nodes.Clear();
            FileList = new Dictionary<string, string>();
            Directory.Delete(ExtractedDir, true);
            Form1.CurrentForm.SortOutputHandler("Repacked.");
            if (File.Exists(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.sig"))
            {
                File.Copy(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.sig", Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local_p.sig");
                File.Delete(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.sig");
                File.Delete(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.pak");
            }
        }
    }
}
