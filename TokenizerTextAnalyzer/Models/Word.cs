namespace TokenizerTextAnalyzer.Models
{
    public class Word
    {
        public string Text { get; }

        public Word(string text)
        {
            Text = text;
        }

        public int Length => Text.Length;

        public char FirstChar => Text.Length > 0 ? Text[0] : '\0';

        public override string ToString() => Text;
    }
}
