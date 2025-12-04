using System.IO;
using System.Xml.Serialization;
using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Export
{
    public class XmlExporter
    {
        // Экспорт в XML-файл
        public void SaveToFile(Text text, string filePath)
        {
            var serializer = new XmlSerializer(typeof(TextExportDto));
            var dto = TextExportDto.From(text);

            // если папка не существует — создаём
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            serializer.Serialize(fs, dto);
        }


        // Получить XML-строку
        public string ToXmlString(Text text)
        {
            var serializer = new XmlSerializer(typeof(TextExportDto));
            var dto = TextExportDto.From(text);

            using var sw = new StringWriter();
            serializer.Serialize(sw, dto);
            return sw.ToString();
        }
    }

    // DTO для сериализации (упрощает XML-структуру)
    [XmlRoot("text")]
    public class TextExportDto
    {
        [XmlElement("sentence")]
        public SentenceExportDto[] Sentences { get; set; } = [];

        public static TextExportDto From(Text text)
        {
            return new TextExportDto
            {
                Sentences = text.Sentences
                                .Select(SentenceExportDto.From)
                                .ToArray()
            };
        }
    }

    public class SentenceExportDto
    {
        [XmlElement("word")]
        public string[] Words { get; set; } = [];

        [XmlElement("punctuation")]
        public string[] Punctuation { get; set; } = [];

        public static SentenceExportDto From(TokenizerTextAnalyzer.Models.Sentence sentence)
        {
            return new SentenceExportDto
            {
                Words = sentence.Words.Select(w => w.Text).ToArray(),
                Punctuation = sentence.Punctuations.Select(p => p.Symbol.ToString()).ToArray()
            };
        }
    }
}
