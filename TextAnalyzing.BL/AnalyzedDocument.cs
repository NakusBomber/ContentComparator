using TextAnalyzing.BL.Interfaces;

namespace TextAnalyzing.BL;

public class AnalyzedDocument : IAnalyzedDocument
{
    private readonly Dictionary<string, int> _wordCount = new();
	private string _name;

	public string Name => _name;
	public HashSet<string> Words => _wordCount.Keys.ToHashSet();
	public int CountWordsInDocument => _wordCount.Select(w => w.Value).Sum();

	public AnalyzedDocument(string name, IEnumerable<string> words)
	{
		_name = name;
		foreach (string word in words)
		{
			if (_wordCount.ContainsKey(word))
			{
				_wordCount[word]++;
			}
			else
			{
				_wordCount.Add(word, 1);
			}
		}
	}

	public int WordCount(string word)
	{
		if (_wordCount.ContainsKey(word))
		{
			return _wordCount[word];
		}
		return 0;
	}
    public bool Contains(string word)
    {
        return _wordCount.ContainsKey(word);
    }

    public double TermFrequency(string word)
	{
		var count = CountWordsInDocument;
		if(count == 0)
		{
			return 0.0;
		}
		return (double)(WordCount(word)) / count;
	}

    public string MostFrequencyWord()
    {
        return _wordCount.MaxBy(kv => kv.Value).Key;
    }
}
