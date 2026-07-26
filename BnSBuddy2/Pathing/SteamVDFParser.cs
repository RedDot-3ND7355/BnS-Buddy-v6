using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Pathing
{
    public class SteamVDFParser
    {
        public VdfNode ParseVdf(string vdfContent)
        {
            if (string.IsNullOrEmpty(vdfContent))
                throw new ArgumentException("VDF content cannot be null or empty.", nameof(vdfContent));
            var lines = vdfContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var parser = new VdfParser(lines);
            return parser.Parse();
        }

        public class VdfNode
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public Dictionary<string, VdfNode> Children { get; set; } = new Dictionary<string, VdfNode>();
        }

        public class VdfParser
        {
            private readonly string[] lines;
            private int currentLine;

            public VdfParser(string[] lines)
            {
                this.lines = lines;
                this.currentLine = 0;
            }

            public VdfNode Parse()
            {
                var root = new VdfNode();
                while (currentLine < lines.Length)
                {
                    string line = lines[currentLine].Trim();
                    if (string.IsNullOrEmpty(line) || line == "}")
                    {
                        currentLine++;
                        continue;
                    }

                    // Parse the key of the current node
                    if (line.StartsWith("\""))
                    {
                        var parts = SplitVdfLine(line);
                        if (parts.Length < 1)
                        {
                            currentLine++;
                            continue;
                        }

                        string key = parts[0];
                        var node = new VdfNode { Key = key };

                        // Check if the next line is an opening brace (indicating a nested object)
                        currentLine++;
                        if (currentLine < lines.Length && lines[currentLine].Trim() == "{")
                        {
                            currentLine++; // Move into the block
                            ParseNode(node); // Parse the nested block
                            root.Children[key] = node;
                        }
                        else if (parts.Length == 2)
                        {
                            // Simple key-value pair
                            node.Value = parts[1];
                            root.Children[key] = node;
                        }
                    }
                    else
                    {
                        currentLine++; // Skip unexpected lines
                    }
                }
                return root;
            }

            private void ParseNode(VdfNode parent)
            {
                while (currentLine < lines.Length)
                {
                    string line = lines[currentLine].Trim();
                    if (string.IsNullOrEmpty(line) || line == "}")
                    {
                        currentLine++;
                        return; // End of current block
                    }

                    if (line.StartsWith("\""))
                    {
                        var parts = SplitVdfLine(line);
                        if (parts.Length < 1)
                        {
                            currentLine++;
                            continue;
                        }

                        string key = parts[0];
                        var node = new VdfNode { Key = key };

                        // Check if the next line is an opening brace (nested object)
                        currentLine++;
                        if (currentLine < lines.Length && lines[currentLine].Trim() == "{")
                        {
                            currentLine++; // Move into the block
                            ParseNode(node); // Recursively parse nested block
                            parent.Children[key] = node;
                        }
                        else if (parts.Length == 2)
                        {
                            // Simple key-value pair
                            node.Value = parts[1];
                            parent.Children[key] = node;
                        }
                    }
                    else
                    {
                        currentLine++; // Skip unexpected lines
                    }
                }
            }

            private string[] SplitVdfLine(string line)
            {
                var parts = new List<string>();
                string current = "";
                bool inQuotes = false;

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '"')
                    {
                        inQuotes = !inQuotes;
                        if (!inQuotes && current.Length > 0)
                        {
                            parts.Add(current);
                            current = "";
                        }
                        continue;
                    }
                    if (inQuotes)
                    {
                        current += line[i];
                    }
                }
                return parts.ToArray();
            }
        }

    }
}
