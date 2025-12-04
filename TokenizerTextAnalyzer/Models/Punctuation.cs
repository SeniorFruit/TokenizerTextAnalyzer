namespace TokenizerTextAnalyzer.Models
{
    public class Punctuation
    {
        public char Symbol { get; }

        public Punctuation(char symbol)
        {
            Symbol = symbol;
        }

        public bool IsQuestionMark => Symbol == '?';

        public override string ToString() => Symbol.ToString();
    }
}
