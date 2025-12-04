public class WordInfo
{
    public int Count { get; set; } = 0;
    public SortedSet<int> LineNumbers { get; } = new();

    public override string ToString()
    {
        string lines = string.Join(" ", LineNumbers);
        return $"{Count}: {lines}";
    }
}
