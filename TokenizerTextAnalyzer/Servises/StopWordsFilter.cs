using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Services
{
    public class StopWordsFilter
    {
        private readonly HashSet<string> _stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public void LoadStopWords(string englishPath, string russianPath)
        {
            LoadFromFile(englishPath);
            LoadFromFile(russianPath);
        }

        public Text RemoveStopWords(Text text)
        {
            var newText = new Text();

            foreach (var sentence in text.Sentences)
            {
                var newSentence = new Sentence();

                foreach (var token in sentence.Tokens)
                {
                    if (token is Word w)
                    {
                        var cleaned = CleanWord(w.Text);
                        var normalized = Normalize(cleaned);

                        if (_stopWords.Contains(normalized))
                        {
                            // стоп-слово — пропускаем
                            continue;
                        }

                        // добавляем очищенное слово вместо исходного
                        if (!string.IsNullOrEmpty(cleaned))
                            newSentence.AddToken(new Word(cleaned));
                    }
                    else
                    {
                        newSentence.AddToken(token); // пунктуация сохраняется
                    }
                }

                if (newSentence.Tokens.Count > 0)
                    newText.AddSentence(newSentence);
            }

            return newText;
        }


        private void LoadFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadLines(path))
            {
                var token = line.Trim();
                if (token.Length == 0) continue;

                _stopWords.Add(Normalize(token));
            }
        }

        private static string Normalize(string s) => s.Trim().ToLowerInvariant();

        private static string CleanWord(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var trimmed = s.Trim();
            // убираем ведущую и замыкающую пунктуацию
            return System.Text.RegularExpressions.Regex.Replace(trimmed, @"^[^\p{L}\p{N}]+|[^\p{L}\p{N}]+$", "");
        }

    }
}
