using System;

namespace BnSBuddy2.LegacyBin
{
    /// <summary>
    /// App-wide editor options so format code does not depend on a specific form instance.
    /// </summary>
    public static class BinEditOptions
    {
        /// <summary>When true, field payloads display/edit as comma-separated ints; otherwise hex.</summary>
        public static bool UseIntData { get; set; } = true;

        /// <summary>Dark UI for WinForms editor/dialogs (default on).</summary>
        public static bool DarkMode { get; set; } = true;

        /// <summary>Optional progress sink for long open/save operations.</summary>
        public static Action<string> Progress { get; set; }

        public static void Report(string message)
        {
            Progress?.Invoke(message);
        }
    }
}
