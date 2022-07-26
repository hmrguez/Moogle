namespace MoogleEngine;

public class Book
{
    public Book(string title, string fullText, Dictionary<string, float> repetitions)
    {
        Title = title;
        FullText = fullText;
        Repetitions = repetitions;
        var array = FullText.Where(x=> Char.IsPunctuation(x) || Char.IsSeparator(x) || Char.IsWhiteSpace(x)).Distinct().ToArray();
        SplittedText = FullText.Split(array, StringSplitOptions.RemoveEmptyEntries);
    }

    public string Title { get; }
    public string FullText { get; }
    public Dictionary<string, float> Repetitions { get; }
    public string[] SplittedText { get; }
}