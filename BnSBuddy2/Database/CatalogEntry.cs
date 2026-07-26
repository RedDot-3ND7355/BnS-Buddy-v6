namespace BnSBuddy2.Database
{
    /// <summary>One row from files_n_hash.php catalog XML.</summary>
    public class CatalogEntry
    {
        public string Filename { get; set; } = "";
        public string FileHash { get; set; } = "";
        public string Author { get; set; } = "";
        public string Description { get; set; } = "";
        /// <summary>Mods: png URL. Splashes: same as file. Addons: empty.</summary>
        public string Preview { get; set; } = "";
        public string Url { get; set; } = "";

        /// <summary>Base name without extension (for Delete.php entry=).</summary>
        public string EntryName
        {
            get
            {
                if (string.IsNullOrEmpty(Filename)) return "";
                int i = Filename.LastIndexOf('.');
                return i >= 0 ? Filename.Substring(0, i) : Filename;
            }
        }
    }
}
