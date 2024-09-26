using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using TextAnalyzing.BL.Interfaces;

namespace TextAnalyzing.BL;

public class SummaryPrinter : ISummaryPrinter
{
    private readonly IPageComparer _pageComparer;

	public SummaryPrinter(IPageComparer pageComparer)
	{
		_pageComparer = pageComparer;
	}

    public void CreateSummary(string path)
    {
        var document = new Document();
        var section = document.AddSection();
        foreach (var doc in _pageComparer.Documents)
        {
            section.Add(CreateDocumentSummary(doc));
        }

        var tableSection = document.AddSection();
        tableSection.PageSetup.Orientation = Orientation.Landscape;
        var table = CreateTableSummary(document, section);
        table = FillDataTableSummary(FillHeadersTableSummary(table));
        tableSection.Add(table);

        var pdfRenderer = new PdfDocumentRenderer();
        pdfRenderer.Document = document;
        pdfRenderer.RenderDocument();
        pdfRenderer.PdfDocument.Save(path);
    }

    public async Task CreateSummaryAsync(string path)
    {
        await Task.Run(() =>
        {
            CreateSummary(path);
        });
    }

    private Paragraph CreateDocumentSummary(IAnalyzedDocument document)
    {
        var paragraph = new Paragraph();

        paragraph.AddFormattedText(document.Name, TextFormat.Bold);
        paragraph.AddLineBreak();
        var mostFrequencyWord = document.MostFrequencyWord();
        paragraph.AddText($"Most frequency word: ");
        paragraph.AddFormattedText($"{mostFrequencyWord}", TextFormat.Bold);
        paragraph.AddText($" ({document.WordCount(mostFrequencyWord)})");
        paragraph.AddLineBreak();
        paragraph.AddLineBreak();
        return paragraph;
    }

    private Table CreateTableSummary(Document document, Section section)
    {
        var table = new Table();

        table.Format.Alignment = ParagraphAlignment.Center;
        
        table.Borders.Width = 1;
        table.Borders.Color = Colors.Black;
        var width = document.DefaultPageSetup.PageHeight * 0.83;
        var height = document.DefaultPageSetup.PageWidth * 0.75;

        var countColumns = _pageComparer.Documents.Count + 1;
        for (int i = 0; i < countColumns; i++)
        {
            table.AddColumn(width/countColumns);
        }

        var countRows = countColumns;
        for (int i = 0; i < countRows; i++)
        {
            var row = table.AddRow();
            row.Height = height / countRows;
        }

        return table;
    }

    private Table FillHeadersTableSummary(Table table)
    {
        string GetHost(string name)
        {
            var hostChunks = new Uri(name).Host.Split('.');
            var host = string.Join(' ', hostChunks
                .Where((chunk, index) => chunk != "www" && index != hostChunks.Length-1));
            return host;
        }

        var rows = table.Rows.Count;
        var cols = table.Columns.Count;

        // Create headers
        for (int j = 1; j < cols; j++)
        {
            var cell = table[0, j];
            cell.VerticalAlignment = VerticalAlignment.Center;
            var text = _pageComparer.Documents.ElementAt(j - 1).Name;
            var parahraph = cell.AddParagraph();
            parahraph.AddWebLink(text).AddText(GetHost(text));
        }
        for (int i = 1; i < rows; i++)
        {
            var cell = table[i, 0];
            cell.VerticalAlignment = VerticalAlignment.Center;
            var text = _pageComparer.Documents.ElementAt(i - 1).Name;
            var parahraph = cell.AddParagraph();
            parahraph.AddWebLink(text).AddText(GetHost(text));
        }

        return table;
    }

    private Table FillDataTableSummary(Table table)
    {
        var rows = table.Rows.Count;
        var cols = table.Columns.Count;

        for (int i = 1; i < rows; i++)
        {
            for (int j = 1; j < cols; j++)
            {
                var cell = table[i, j];
                cell.VerticalAlignment = VerticalAlignment.Center;
                var doc1 = _pageComparer.Documents.ElementAt(i-1);
                var doc2 = _pageComparer.Documents.ElementAt(j-1);
                var cosineSimilarity = _pageComparer.CosineSimilarity(doc1, doc2);
                var paragraph = cell.AddParagraph();
                paragraph.AddText($"{cosineSimilarity:f2}");
            }
        }

        return table;
    }
}
