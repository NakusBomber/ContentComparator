namespace TextAnalyzing.UI;

/// <summary>
/// Progress bar for console
/// <br></br>
/// <seealso href="https://ru.stackoverflow.com/a/1348617"/>
/// </summary>
public class ConsoleProgressBar
{
    private readonly char _progressChar = '|';
    private readonly char _noProgressChar = '.';
    public int Length { get; set; }
    public int Left { get; set; }
    public int Top { get; set; }

    public ConsoleProgressBar(int left, int top, int length)
    {
        Left = left;
        Top = top;
        Length = length;
    }

    public void ShowProgress(int progress)
    {
        if (progress > Length || progress < 0)
        {
            throw new ArgumentException($"Invalid progress value, must be between 0 and {Length} but actual {progress}.");
        }
        Console.SetCursorPosition(Left, Top);
        Console.Write($"{new string(_progressChar, progress)}{new string(_noProgressChar, Length - progress)} {progress}%");
    }
}
