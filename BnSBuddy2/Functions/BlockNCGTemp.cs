using System.IO;
using System.Security.AccessControl;

namespace BnSBuddy2.Functions
{
    public static class BlockNCGTemp
    {

        // This is a temporary solution to prevent NCG from being used until further notice.

        public static void BlockNCG()
        {

            string tempPath = System.IO.Path.GetTempPath();
            string ncgTempPath = System.IO.Path.Combine(tempPath, "ncgtmp");
            if (System.IO.Directory.Exists(ncgTempPath))
            {
                try
                {
                    // Delete existing bb64.dll file to block NCG functionality.
                    foreach (var file in System.IO.Directory.GetFiles(ncgTempPath, "*", System.IO.SearchOption.AllDirectories))
                        System.IO.File.Delete(file);
                    // Set Access Control Perms to block NCG functionality.
                    DirectoryInfo directoryInfo = new DirectoryInfo(ncgTempPath);
                    DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                    string targetIdentity = "Administrators";
                    FileSystemAccessRule denyRule = new FileSystemAccessRule
                       (
                       targetIdentity,
                       FileSystemRights.Write | FileSystemRights.Modify, // Deny Write and Modify
                       InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, // Apply to folder and subfolders/files
                       PropagationFlags.None,
                       AccessControlType.Deny
                       );
                    directorySecurity.AddAccessRule(denyRule);
                    directoryInfo.SetAccessControl(directorySecurity);
                    Form1.CurrentForm.AddLauncherLog("Blocked ncgtmp folder.");
                }
                catch { Form1.CurrentForm.AddLauncherLog("[Error] Could not add Deny Rule!"); }
            }
        }

        public static void UnblockNCG()
        {
            string tempPath = System.IO.Path.GetTempPath();
            string ncgTempPath = System.IO.Path.Combine(tempPath, "ncgtmp");
            if (System.IO.Directory.Exists(ncgTempPath))
            {
                try
                {
                    // Set Access Control Perms to block NCG functionality.
                    DirectoryInfo directoryInfo = new DirectoryInfo(ncgTempPath);
                    DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                    string targetIdentity = "Administrators";
                    FileSystemAccessRule denyRule = new FileSystemAccessRule
                       (
                       targetIdentity,
                       FileSystemRights.Write | FileSystemRights.Modify, // Deny Write and Modify
                       InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, // Apply to folder and subfolders/files
                       PropagationFlags.None,
                       AccessControlType.Deny
                       );
                    directorySecurity.RemoveAccessRule(denyRule);
                    directoryInfo.SetAccessControl(directorySecurity);
                    Form1.CurrentForm.AddLauncherLog("Unblocked ncgtmp folder.");
                }
                catch { Form1.CurrentForm.AddLauncherLog("[Error] Could not remove Deny Rule!"); }
            }
        }
    }
}
