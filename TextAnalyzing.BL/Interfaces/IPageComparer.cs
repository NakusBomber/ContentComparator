namespace TextAnalyzing.BL.Interfaces;

public interface IPageComparer
{
    public IReadOnlySet<IAnalyzedDocument> Documents { get; }

    /// <summary>
    /// IDF
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public double InverseDocumentFrequency(string word);

    /// <summary>
    /// TF-IDF
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public double TermFrequencyInverseDocumentFrequency(string word, IAnalyzedDocument document);

    /// <summary>
    /// Cosine similarity
    /// </summary>
    /// <param name="firstVector"></param>
    /// <param name="secondVector"></param>
    /// <returns></returns>
    public double CosineSimilarity(IAnalyzedDocument doc1, IAnalyzedDocument doc2);

}
