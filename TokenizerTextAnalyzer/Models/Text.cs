using System.Collections.Generic;

namespace TokenizerTextAnalyzer.Models
{
    public class Text
    {
        private readonly List<Sentence> _sentences = new();

        public IReadOnlyList<Sentence> Sentences => _sentences;

        public void AddSentence(Sentence sentence)
        {
            _sentences.Add(sentence);
        }

        public override string ToString() => string.Join("\n", _sentences);
    }
}
