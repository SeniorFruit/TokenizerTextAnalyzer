using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TokenizerTextAnalyzer.Services
{
    public class TextFileSelector
    {
        private readonly string _searchRoot;

        
        private readonly HashSet<string> _excludedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "stopwords_en.txt",
            "stopwords_ru.txt"
        };

        public TextFileSelector()
        {
            _searchRoot = AppDomain.CurrentDomain.BaseDirectory;
        }

        public List<string> GetAllTxtFiles()
        {
            return Directory.GetFiles(_searchRoot, "*.txt", SearchOption.AllDirectories)
                            .Select(Path.GetFileName)
                            .Where(f => !string.IsNullOrWhiteSpace(f) && !_excludedFiles.Contains(f))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();
        }

        public string? GetFullPath(string fileName)
        {
            return Directory.GetFiles(_searchRoot, "*.txt", SearchOption.AllDirectories)
                            .FirstOrDefault(f => Path.GetFileName(f).Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
