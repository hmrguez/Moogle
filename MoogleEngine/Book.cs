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

    public IEnumerable<string> GetWords()
    {
        foreach (var item in SplittedText)
        {
            yield return item;
        }
    }
}

public static class Helper
{
    public static IEnumerable<string> Words(this IEnumerable<Book> books)
    {
        foreach (var item in books)
        {
            foreach (var item2 in item.GetWords())
            {
                yield return item2;
            }
        }
    }
}