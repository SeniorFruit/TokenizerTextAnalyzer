using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Services
{
    public class SentenceTransformer
    {
        // Заменяет слова заданной длины во всем тексте
        public Text ReplaceWordsByLength(Text text, int targetLength, string replacement)
        {
            var newText = new Text();

            foreach (var sentence in text.Sentences)
            {
                var newSentence = new Sentence();

                foreach (var token in sentence.Tokens)
                {
                    if (token is Word w && w.Length == targetLength)
                        newSentence.AddToken(new Word(replacement));
                    else
                        newSentence.AddToken(token);
                }

                newText.AddSentence(newSentence);
            }

            return newText;
        }
    }
}
