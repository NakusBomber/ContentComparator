using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using TextAnalyzing.BL;
using TextAnalyzing.BL.Interfaces;
using TextAnalyzing.UI;

static partial class Program
{
    private static string _pathToMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private static readonly HttpParser _parser = new HttpParser();
    private static DateTime _timeBefore;
    private static int _maxProgress = 100;
    private static int _progress = 0;
    private static int _currentUrl = 0;
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private static readonly ConsoleProgressBar _progressBar = new ConsoleProgressBar(0, 0, _maxProgress);
    private static string _processText = "Process";
    private static string _successText = "Success";
    private static Dictionary<string, string>? _urls;

    private static async Task Main(string[] args)
    {
        try
        {
            var urls = GetAllPaths(new List<string>()).ToList();
            while (urls.Count < 2)
            {
                Console.WriteLine("Urls must be more than 1");
                Console.ReadKey();
                urls = GetAllPaths(urls).ToList();
            }
            _urls = (from url in urls
                     select url).ToDictionary(url => url, url => _processText);

            Console.Clear();
            SetSettings();
            var taskButton = ActionButton(_cancellationTokenSource.Token);
            var tasks = GetAllTasks();
            var docs = (await Task.WhenAll(tasks))
                .ToList()
                .Where(doc => doc != null);
            _cancellationTokenSource.Cancel();
            FinishProgress();
            WriteDeltaTime("Elapsed time on get and analyzing texts");

            SetSettings();
            var comparer = new PageComparer(docs!);
            var printer = new SummaryPrinter(comparer);

            var fileName = $"Summary {DateTime.Now}".Replace('.', '-').Replace(':', '-');
            var path = Path.Combine(_pathToMyDocuments, fileName+".pdf");
            await printer.CreateSummaryAsync(path);
            WriteDeltaTime("Elapsed time on create summary");
            await OpenSummary(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static IEnumerable<string> GetAllPaths(List<string> urls)
    {
        while (true)
        {
            Console.Write("Input path to webpage: ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line) || !line.StartsWith(Uri.UriSchemeHttp))
            {
                Console.WriteLine("Path must be not empty and valid");
                Console.ReadKey();
                continue;
            }

            if(line.ToLower() == "q")
            {
                break;
            }

            urls.Add(line);
        }

        return urls;
    }

    private static void SetSettings()
    {
        Console.CursorVisible = false;
        _timeBefore = DateTime.Now;
    }

    private static void ShowProgress()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        _progressBar.ShowProgress(_progress);

        Console.CursorLeft = 0;
        Console.CursorTop = 1;
        Console.WriteLine(" ".PadRight(Console.WindowWidth));
        var url = _urls!.Keys.ElementAt(_currentUrl);
        ShowUrl(url, _urls[url]);

        Console.ResetColor();
        Console.WriteLine($"{_currentUrl + 1} / {_urls.Count}".PadRight(Console.WindowWidth));
        Console.WriteLine(" ".PadRight(Console.WindowWidth));
    }
    private static void ShowAll()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        _progressBar.ShowProgress(_progress);
        Console.WriteLine();
        Console.WriteLine();

        foreach (var url in _urls!.Keys)
        {
            ShowUrl(url, _urls[url]);
        }
    }
    private static void ShowUrl(string url, string code)
    {
        Console.ResetColor();
        Console.WriteLine(url.PadRight(Console.WindowWidth));
        var color = code == _processText
            ? ConsoleColor.Yellow
            : code == _successText
                ? ConsoleColor.Green
                : ConsoleColor.Red;
        Console.ForegroundColor = color;
        Console.WriteLine(code.PadRight(Console.WindowWidth));
        Console.WriteLine(" ".PadRight(Console.WindowWidth));
    }

    private static void FinishProgress()
    {
        _progress = _maxProgress;
        Console.Clear();
        ShowAll();
    }

    private static void WriteDeltaTime(string text)
    {
        Console.ResetColor();
        Console.WriteLine($"{text}: {GetDeltaTime().TotalSeconds:f3}s");
    }
    private static TimeSpan GetDeltaTime()
    {
        return DateTime.Now - _timeBefore;
    }
    private static List<Task<AnalyzedDocument?>> GetAllTasks()
    {
        return (from url in _urls!.Keys
                select ActionTask(url, _urls.Count)).ToList();
    }

    private static Task<AnalyzedDocument?> ActionTask(string url, int countUrls)
    {
        var increaseProgress = _maxProgress / countUrls;
        var task = Task.Run(async () =>
        {
            try
            {
                var content = await _parser.ParseAsync(url);
                var doc = new AnalyzedDocument(url, content);
                _progress += increaseProgress;
                _urls![url] = _successText;
                return doc;
            }
            catch (Exception e)
            {
                _urls![url] = e.Message;
                return null;
            }
            finally
            {
                ShowProgress();
            }
        });
        return task;
    }

    private static Task ActionButton(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    _currentUrl++;
                    if (_currentUrl == _urls!.Count)
                    {
                        _currentUrl = 0;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    _currentUrl--;
                    if (_currentUrl < 0)
                    {
                        _currentUrl = _urls!.Count - 1;
                    }
                }

                ShowProgress();
            }
        }, cancellationToken);
    }

    private static async Task OpenSummary(string path)
    {
        await Task.Run(() =>
        {
            Process.Start("explorer.exe", path);
        });
    }
}