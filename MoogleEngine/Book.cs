namespace MoogleEngine;

public class Book
{
    public Book(string title, string fullText, Dictionary<string, float> repetitions)
    {
        Title = title;
        FullText = fullText;
        Repetitions = repetitions;
        SplittedText = FullText.Split(new char[] { ' ', ',', ';', ':', '/', '?', '!', ']', '[', '}', '{' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public string Title { get; }
    public string FullText { get; }
    public Dictionary<string, float> Repetitions { get; }
    public string[] SplittedText { get; }
}