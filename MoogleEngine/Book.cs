namespace MoogleEngine;

public class Book
{
    public Book(string title, string fullText, Dictionary<string, float> repetitions)
    {
        Title = title;
        FullText = fullText;
        Repetitions = repetitions;
        SplittedText = FullText.Split(new char[] { ' ', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public string Title { get; set; }
    public string FullText { get; set; }
    public Dictionary<string, float> Repetitions { get; set; }
    public string[] SplittedText { get; set; }
}

public static class Helper
{
    public static string[] Words(this IEnumerable<Book> books) => books.SelectMany(x=>x.SplittedText).Distinct().ToArray();
}