using System.Collections.Generic;
using System.Linq;

namespace TokenizerTextAnalyzer.Models
{
    public class Sentence
    {
        private readonly List<object> _tokens = new(); 

        public IReadOnlyList<object> Tokens => _tokens;

        public void AddToken(object token)
        {
            if (token is Word || token is Punctuation)
                _tokens.Add(token);
        }

        public IEnumerable<Word> Words => _tokens.OfType<Word>();

        public IEnumerable<Punctuation> Punctuations => _tokens.OfType<Punctuation>();

        public int WordCount => Words.Count();

        public int Length => _tokens.Sum(t => t.ToString()?.Length ?? 0);

        public bool IsQuestion => Punctuations.Any(p => p.IsQuestionMark);

        public override string ToString()
        {
            var parts = new List<string>();
            foreach (var token in _tokens)
            {
                if (token is Word w)
                {
                    parts.Add(w.Text);
                }
                else if (token is Punctuation p)
                {
                    if (parts.Count == 0) parts.Add(p.Symbol.ToString());
                    else parts[parts.Count - 1] = parts[parts.Count - 1] + p.Symbol;
                }
            }
            return string.Join(" ", parts);
        }
    }
}
