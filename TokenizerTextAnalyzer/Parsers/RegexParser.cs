using System.Text.RegularExpressions;
using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Parsers
{
    public class RegexParser : ITextParser
    {
        // Предложение: последовательность до завершающего знака
        private static readonly Regex SentenceRegex =
            new(@"[^.!?]+[.!?]+|[^.!?]+$", RegexOptions.Multiline);

        // Токены: слова (буквы/цифры), либо одиночная пунктуация
        private static readonly Regex TokenRegex =
            new(@"[\p{L}\p{N}]+|[.,!?;:()\[\]""—\-]", RegexOptions.Multiline);

        public Text Parse(string rawText)
        {
            var text = new Text();
            var sentenceMatches = SentenceRegex.Matches(rawText);

            foreach (Match sentenceMatch in sentenceMatches)
            {
                var sentence = new Sentence();
                var tokenMatches = TokenRegex.Matches(sentenceMatch.Value);

                foreach (Match tokenMatch in tokenMatches)
                {
                    string token = tokenMatch.Value;
                    if (Regex.IsMatch(token, @"^[\p{L}\p{N}]+$"))
                        sentence.AddToken(new Word(token));
                    else
                        sentence.AddToken(new Punctuation(token[0]));
                }

                if (sentence.Tokens.Count > 0)
                    text.AddSentence(sentence);
            }

            return text;
        }
    }
}
