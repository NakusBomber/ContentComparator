namespace TextAnalyzing.BL.Interfaces;

public interface IAnalyzedDocument
{
    public string Name { get; }
    public HashSet<string> Words { get; }
    public int CountWordsInDocument { get; }
    public string MostFrequencyWord();
    public bool Contains(string word);
    public int WordCount(string word);

    /// <summary>
    /// TF
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public double TermFrequency(string word);
}
