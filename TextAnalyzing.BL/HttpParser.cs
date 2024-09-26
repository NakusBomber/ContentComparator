using HtmlAgilityPack;
using System.Text.RegularExpressions;
using TextAnalyzing.BL.Interfaces;

namespace TextAnalyzing.BL;

public class HttpParser : IHttpParser
{
    private readonly char[] _symbolsToRemove = [' ', ',', '.', 
                                                ':', '?', '!', 
                                                ';', '"', '(', 
                                                ')', '/', '^',
                                                '+', '\'', '-'];
    private readonly Regex _jsonRegex = new Regex(@"\{.*?\}|\[.*?\]|\"".*?\"":.*?(?=,|\})");
    private readonly Regex _specialSymbolsRegex = new Regex(@"&#\d+;");
    private readonly Regex _linkInTextRegex = new Regex(@"\[\d+\]");
    private readonly Regex _whitespaceRegex = new Regex(@"\s+");
    private const string _xPath = "//body//*[not(self::script or self::style)]//text()";
    private readonly List<string> _mudFrazes = new List<string>
    {
        "@media", "http"
    };
    private readonly List<string> _skipWords = new List<string>
    {
        "the", "a", "an", "to", "at", "of", "in", "and", "are", "or", "is", "s"
    };

    public IEnumerable<string> Parse(string url)
    {
        return ParseAsync(url).Result;
    }

    public async Task<IEnumerable<string>> ParseAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("Url is null or empty");
        }

        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error code: {response.StatusCode}");
            }

            var html = await response.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var text = htmlDoc.DocumentNode
                .SelectNodes(_xPath)
                .Select(node => node.InnerText.Trim())
                .Where(text => !string.IsNullOrEmpty(text));

            var allTexts = from el in text
                           select ClearText(el);

            return ToLower(RemoveOther(SplitWithSymbols(string.Join(' ', allTexts))));
        }
    }

    private string ClearText(string text)
    {
        var newText = _jsonRegex.Replace(text, string.Empty);
        newText = _specialSymbolsRegex.Replace(newText, string.Empty);
        newText = _linkInTextRegex.Replace(newText, string.Empty);
        newText = _whitespaceRegex.Replace(newText, " ").Trim();

        if (_mudFrazes.Any(m => newText.StartsWith(m)))
        {
            newText = string.Empty;
        }
        return newText;
    }

    private IEnumerable<string> SplitWithSymbols(string text)
    {
        return text.Split(_symbolsToRemove);
    }

    private IEnumerable<string> RemoveOther(IEnumerable<string> words)
    {
        return words.Where(word => word.Length > 0 && 
                                   char.IsLetter(word[0]) && 
                                   char.IsLetter(word[^1]) &&
                                   !_skipWords.Contains(word.ToLower()));
    }

    private IEnumerable<string> ToLower(IEnumerable<string> words)
    {
        return words.Select(word => word.ToLower());
    }
}
