namespace TextAnalyzing.BL.Interfaces;

public interface IHttpParser
{
    public IEnumerable<string> Parse(string url);
    public Task<IEnumerable<string>> ParseAsync(string url);
}
