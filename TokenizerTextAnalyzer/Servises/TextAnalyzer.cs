using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Services
{
    public class TextAnalyzer
    {
        // 1. Сортировка предложений по возрастанию количества слов
        public IList<Sentence> SortByWordCount(Text text)
        {
            return text.Sentences
                       .OrderBy(s => s.WordCount)
                       .ToList();
        }

        // 2. Сортировка предложений по возрастанию длины предложения
        public IList<Sentence> SortBySentenceLength(Text text)
        {
            return text.Sentences
                       .OrderBy(s => s.Length)
                       .ToList();
        }

        // 3. Найти слова заданной длины (во всех или только в вопросительных)
        public ISet<string> FindUniqueWordsByLength(Text text, int length, bool onlyQuestions = true)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var sentence in text.Sentences)
            {
                if (onlyQuestions && !sentence.IsQuestion) continue;

                foreach (var word in sentence.Words)
                {
                    var cleaned = CleanWord(word.Text);
                    if (cleaned.Length == length)
                        result.Add(cleaned);
                }
            }

            return result;
        }

        // 4. Удалить из текста все слова заданной длины, начинающиеся с согласной буквы
        public Text RemoveWordsByLengthStartingWithConsonant(Text text, int length)
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
                        var first = FirstLetter(cleaned);

                        if (cleaned.Length == length && first.HasValue && IsConsonant(first.Value))
                        {
                            // пропускаем (удаляем)
                            continue;
                        }
                        newSentence.AddToken(w);
                    }
                    else
                    {
                        newSentence.AddToken(token);
                    }
                }

                if (newSentence.Tokens.Count > 0)
                    newText.AddSentence(newSentence);
            }

            return newText;
        }

        // 5. Замена слов заданной длины в конкретном предложении
        public Text ReplaceWordsInSentence(Text text, int sentenceIndex, int targetLength, string replacement)
        {
            if (sentenceIndex < 0 || sentenceIndex >= text.Sentences.Count)
                return text;

            var newText = new Text();

            for (int i = 0; i < text.Sentences.Count; i++)
            {
                var sentence = text.Sentences[i];
                var newSentence = new Sentence();

                foreach (var token in sentence.Tokens)
                {
                    if (i == sentenceIndex && token is Word w)
                    {
                        var cleaned = CleanWord(w.Text);
                        if (cleaned.Length == targetLength)
                        {
                            newSentence.AddToken(new Word(replacement));
                            continue;
                        }
                    }
                    newSentence.AddToken(token);
                }

                newText.AddSentence(newSentence);
            }

            return newText;
        }

        // Проверка согласной
        private static bool IsConsonant(char c)
        {
            if (!char.IsLetter(c)) return false;

            var englishVowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u', 'y',
                                                    'A', 'E', 'I', 'O', 'U', 'Y' };

            var russianVowels = new HashSet<char> { 'а','е','ё','и','о','у','ы','э','ю','я',
                                                    'А','Е','Ё','И','О','У','Ы','Э','Ю','Я' };

            if (c <= 0x7F) // Latin
                return !englishVowels.Contains(c);
            else
                return !russianVowels.Contains(c);
        }

        // Возвращает слово без ведущей/замыкающей пунктуации и служебных символов
        private static string CleanWord(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            // Оставляем только буквы/цифры и внутренние дефисы/апострофы
            // Убираем ведущие/замыкающие не-слова
            var trimmed = s.Trim();
            // Удалим обрамляющие кавычки/тире/скобки
            trimmed = Regex.Replace(trimmed, @"^[^\p{L}\p{N}]+|[^\p{L}\p{N}]+$", "");
            // Если внутри остался мусор, можно дополнительно чистить, но обычно этого достаточно
            return trimmed;
        }

        // Первая буква в строке (если есть)
        private static char? FirstLetter(string s)
        {
            foreach (var ch in s)
            {
                if (char.IsLetter(ch)) return ch;
            }
            return null;
        }

        public Dictionary<string, WordInfo> BuildConcordance(Text text)
        {
            var concordance = new Dictionary<string, WordInfo>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < text.Sentences.Count; i++)
            {
                var sentence = text.Sentences[i];
                int lineNumber = i + 1;

                foreach (var word in sentence.Words)
                {
                    var cleaned = CleanWord(word.Text);
                    if (cleaned.Length == 0) continue;

                    string key = cleaned.ToLowerInvariant();

                    if (!concordance.TryGetValue(key, out var info))
                    {
                        info = new WordInfo();
                        concordance[key] = info;
                    }

                    info.Count++;
                    info.LineNumbers.Add(lineNumber);
                }
            }

            return concordance;
        }
    }
}
