namespace TextAnalyzing.BL.Interfaces;

public interface ISummaryPrinter
{
    public void CreateSummary(string path);
    public Task CreateSummaryAsync(string path);
}
