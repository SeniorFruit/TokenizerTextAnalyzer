using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Services
{
    public class StopWordsFilter
    {
        private readonly HashSet<string> _stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Загрузка стоп-слов из файлов (английские и русские)
        public void LoadStopWords(string englishPath, string russianPath)
        {
            LoadFromFile(englishPath);
            LoadFromFile(russianPath);
        }

        // Удалить стоп-слова из текста (на английском и русском)
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
                        if (_stopWords.Contains(Normalize(w.Text)))
                        {
                            // пропускаем это слово
                            continue;
                        }

                        newSentence.AddToken(w);
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

        // Нормализация слова: нижний регистр, обрезка пробелов
        private static string Normalize(string s) => s.Trim().ToLowerInvariant();
    }
}
