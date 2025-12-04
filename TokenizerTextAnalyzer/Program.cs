using System;
using System.IO;
using System.Text;
using TokenizerTextAnalyzer.Models;
using TokenizerTextAnalyzer.Parsers;
using TokenizerTextAnalyzer.Services;
using TokenizerTextAnalyzer.Export;
using static System.Text.CodePagesEncodingProvider;
class Program
{
    static void Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Console.OutputEncoding = Encoding.UTF8;
        var selector = new TextFileSelector();
        var files = selector.GetAllTxtFiles();

        if (files.Count == 0)
        {
            Console.WriteLine("Текстовые файлы не найдены.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\nВыберите текстовый файл для анализа:");
            for (int i = 0; i < files.Count; i++)
                Console.WriteLine($"{i + 1}. {files[i]}");
            Console.WriteLine("0. Выход");

            int fileChoice = GetUserChoice(files.Count, allowZero: true);
            if (fileChoice == 0) break;

            string selectedFile = files[fileChoice - 1];
            string? fullPath = selector.GetFullPath(selectedFile);

            if (fullPath == null || !File.Exists(fullPath))
            {
                Console.WriteLine("Ошибка: файл не найден.");
                continue;
            }

            string rawText = ReadTextRobust(fullPath);

            Console.WriteLine("\nВыберите парсер:");
            Console.WriteLine("1. Посимвольный парсер");
            Console.WriteLine("2. Парсер с регулярными выражениями");

            int parserChoice = GetUserChoice(2);
            ITextParser parser = parserChoice == 1 ? new CharByCharParser() : new RegexParser();
            Text text = parser.Parse(rawText);

            Console.WriteLine("\nВыберите операцию:");
            Console.WriteLine("1. Сортировать предложения по количеству слов");
            Console.WriteLine("2. Сортировать предложения по длине");
            Console.WriteLine("3. Найти слова заданной длины в вопросительных предложениях");
            Console.WriteLine("4. Удалить слова заданной длины, начинающиеся с согласной");
            Console.WriteLine("5. Заменить слова заданной длины в предложении на подстроку");
            Console.WriteLine("6. Удалить стоп-слова");
            Console.WriteLine("7. Экспортировать текст в XML");
            Console.WriteLine("0. Вернуться к выбору файла");

            int actionChoice = GetUserChoice(7, allowZero: true);
            if (actionChoice == 0) continue;

            var analyzer = new TextAnalyzer();
            var transformer = new SentenceTransformer();
            var stopFilter = new StopWordsFilter();
            var exporter = new XmlExporter();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            switch (actionChoice)
            {
                case 1:
                    var byWords = analyzer.SortByWordCount(text);
                    Console.WriteLine("\nПредложения по количеству слов:");
                    foreach (var s in byWords) Console.WriteLine(s);
                    break;

                case 2:
                    var byLen = analyzer.SortBySentenceLength(text);
                    Console.WriteLine("\nПредложения по длине:");
                    foreach (var s in byLen) Console.WriteLine(s);
                    break;

                case 3:
                    Console.Write("Введите длину слова: ");
                    int lenQ = ReadIntOrZero();
                    var found = analyzer.FindUniqueWordsByLength(text, lenQ, true);

                    Console.WriteLine("\nНайденные слова:");
                    foreach (var w in found) Console.WriteLine(w);
                    break;

                case 4:
                    Console.Write("Введите длину слова для удаления: ");
                    int lenDel = ReadIntOrZero();
                    var cleaned = analyzer.RemoveWordsByLengthStartingWithConsonant(text, lenDel);
                    Console.WriteLine("\nТекст после удаления слов:");
                    foreach (var s in cleaned.Sentences) Console.WriteLine(s);
                    break;

                case 5:
                    Console.Write("Введите индекс предложения (начиная с 1): ");
                    int index = ReadIntOrDefault(1) - 1;
                    Console.Write("Введите длину слова для замены: ");
                    int lenRep = ReadIntOrZero();
                    Console.Write("Введите подстроку для замены: ");
                    string replacement = Console.ReadLine() ?? "";
                    var replaced = analyzer.ReplaceWordsInSentence(text, index, lenRep, replacement);
                    Console.WriteLine("\nТекст после замены:");
                    foreach (var s in replaced.Sentences) Console.WriteLine(s);
                    break;

                case 6:
                    stopFilter.LoadStopWords(
                        Path.Combine(baseDir, "Resources", "stopwords_en.txt"),
                        Path.Combine(baseDir, "Resources", "stopwords_ru.txt")
                    );
                    var filtered = stopFilter.RemoveStopWords(text);
                    Console.WriteLine("\nТекст после удаления стоп-слов:");
                    foreach (var s in filtered.Sentences) Console.WriteLine(s);
                    break;

                case 7:
                    string xmlPath = Path.Combine(baseDir, "Resources", "exported.xml");

                    Directory.CreateDirectory(Path.GetDirectoryName(xmlPath)!);

                    exporter.SaveToFile(text, xmlPath);
                    Console.WriteLine($"\nТекст экспортирован в XML: {xmlPath}");
                    break;
            }
        }

        Console.WriteLine("\nРабота завершена.");
    }

    static int GetUserChoice(int max, bool allowZero = false)
    {
        int choice;
        do
        {
            Console.Write("Ваш выбор: ");
        } while (!int.TryParse(Console.ReadLine(), out choice) ||
                 choice < (allowZero ? 0 : 1) || choice > max);
        return choice;
    }

    static int ReadIntOrZero()
    {
        return int.TryParse(Console.ReadLine(), out var v) ? v : 0;
    }

    static int ReadIntOrDefault(int def)
    {
        return int.TryParse(Console.ReadLine(), out var v) ? v : def;
    }

    static string ReadTextRobust(string path)
    {
        string utf8 = File.ReadAllText(path, Encoding.UTF8);
        if (ContainsLetters(utf8)) return utf8;

        var cp1251 = Encoding.GetEncoding(1251);
        string ansi = File.ReadAllText(path, cp1251);
        return ansi;
    }

    static bool ContainsLetters(string s)
    {
        foreach (var ch in s) if (char.IsLetter(ch)) return true;
        return false;
    }
}
