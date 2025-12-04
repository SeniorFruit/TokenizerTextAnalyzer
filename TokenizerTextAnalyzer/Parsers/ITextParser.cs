using TokenizerTextAnalyzer.Models;

namespace TokenizerTextAnalyzer.Parsers
{
    public interface ITextParser
    {
        Text Parse(string rawText);
    }
}
