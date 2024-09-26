using TextAnalyzing.BL.Interfaces;

namespace TextAnalyzing.BL;

public class PageComparer : IPageComparer
{
    private readonly HashSet<IAnalyzedDocument> _documents;
    private readonly Dictionary<IAnalyzedDocument, Dictionary<string, double>> _tfIdfVectors = new();
    public IReadOnlySet<IAnalyzedDocument> Documents => _documents;
	public PageComparer(IEnumerable<IAnalyzedDocument> analyzedDocuments)
	{
        if(analyzedDocuments == null || analyzedDocuments.Count() < 2)
        {
            throw new ArgumentException("Count must be more than 1");
        }
		_documents = analyzedDocuments.ToHashSet();
	}

    public double InverseDocumentFrequency(string word)
    {
        int countDocumentWithWord = 0;
        foreach (IAnalyzedDocument doc in _documents)
        {
            if (doc.Contains(word))
            {
                countDocumentWithWord++;
            }
        }

        if(countDocumentWithWord == 0)
        {
            throw new ArgumentException("Documents not contains this word");
        }
        return Math.Log(_documents.Count / countDocumentWithWord);
    }

    public double TermFrequencyInverseDocumentFrequency(string word, IAnalyzedDocument document)
    {
        var tf = document.TermFrequency(word);
        var idf = InverseDocumentFrequency(word);

        if(tf == 0 || idf == 0)
        {
            return 0.0;
        }
        return tf * idf;
    }

    public double CosineSimilarity(
        IAnalyzedDocument doc1,
        IAnalyzedDocument doc2)
    {
        Dictionary<string, double> firstVector = GetTFIDFVector(doc1);
        Dictionary<string, double> secondVector = GetTFIDFVector(doc2);

        double scalarProduct = 0.0;
        double firstNorm = 0.0;
        double secondNorm = 0.0;

        foreach (var word in firstVector.Keys)
        {
            var firstValue = firstVector.ContainsKey(word) ? firstVector[word] : 0.0;
            var secondValue = secondVector.ContainsKey(word) ? secondVector[word] : 0.0;

            scalarProduct += firstValue * secondValue;
            firstNorm += Math.Pow(firstValue, 2);
            secondNorm += Math.Pow(secondValue, 2);
        }

        if(firstNorm == 0 || secondNorm == 0)
        {
            return 0.0;
        }

        return scalarProduct / (Math.Sqrt(firstNorm) * Math.Sqrt(secondNorm));
    }

    private Dictionary<string, double> ComputeTFIDFVector(IEnumerable<string> allWords, IAnalyzedDocument document)
    {
        return (from word in allWords
                select word)
                .ToDictionary(k => k, v => TermFrequencyInverseDocumentFrequency(v, document));
    }

    private Dictionary<string, double> GetTFIDFVector(IAnalyzedDocument doc)
    {
        if (!_tfIdfVectors.ContainsKey(doc))
        {
            _tfIdfVectors.Add(doc, ComputeTFIDFVector(doc.Words, doc));
        }
        return _tfIdfVectors[doc];
    }
}
