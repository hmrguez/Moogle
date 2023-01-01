namespace MoogleEngine;

public static class MathSearch
{
    public static float Score(string word, Moogle.Book book, Moogle.Book[] corpus)
    {
        var head = book.Words.Head(word);
        var tf = head?.Reps / book.Words.WordCount ?? 0;
        var idf = MathF.Log((float)corpus.Length / (float)(1 + corpus.Count(x => x.Words.Contains(word))));

        return tf * idf;
    }
}