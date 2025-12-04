using System.Text;
using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Parsers
{
    public class CharByCharParser : ITextParser
    {
        private static readonly char[] SentenceEndings = { '.', '!', '?' };
        private static readonly char[] PunctuationMarks = { '.', ',', ';', ':', '!', '?', '-', '(', ')', '[', ']', '"' };

        public Text Parse(string rawText)
        {
            var text = new Text();
            var sentence = new Sentence();
            var wordBuffer = new StringBuilder();

            foreach (char c in rawText)
            {
                if (char.IsLetterOrDigit(c))
                {
                    wordBuffer.Append(c);
                }
                else
                {
                    if (wordBuffer.Length > 0)
                    {
                        sentence.AddToken(new Word(wordBuffer.ToString()));
                        wordBuffer.Clear();
                    }

                    if (PunctuationMarks.Contains(c))
                    {
                        sentence.AddToken(new Punctuation(c));

                        if (SentenceEndings.Contains(c))
                        {
                            text.AddSentence(sentence);
                            sentence = new Sentence();
                        }
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        continue;
                    }
                }
            }

            if (wordBuffer.Length > 0)
                sentence.AddToken(new Word(wordBuffer.ToString()));

            if (sentence.Tokens.Count > 0)
                text.AddSentence(sentence);

            return text;
        }
    }
}
