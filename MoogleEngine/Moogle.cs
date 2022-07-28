using System.IO;
using System;
using System.Linq;
namespace MoogleEngine;

public static class Moogle
{
    public record Book(string Title, Dictionary<string, float> Repetitions, string FullText, string[] Words);
    public static Book[] Books = Scan().ToArray();
    public static string[] Words = Books.SelectMany(x => x.Words).Distinct().ToArray();

    public static SearchResult Query(string query)
    {
        SearchItem[] x = Search(query).OrderBy(x => x.Score).ToArray();


        return x switch
        {
            SearchItem[] h when x.Length != 0 => new SearchResult(x, null!),
            SearchItem[] h when Suggestion(query).Length != 0 => new SearchResult(Search(Suggestion(query)).ToArray(), Suggestion(query)),
            _ => new SearchResult(new SearchItem[] { new SearchItem("No se encontro ningun resultado", "", 0) }, null!)
        };
    }


    #region Search and Scan
    private static IEnumerable<SearchItem> Search(string query)
    {
        var x = query.Split(' ').ToList();
        foreach (var item in Books)
        {
            if (x.Exists(p => item.Repetitions.Keys.Contains(p)))
                if (Score(item, query) != 0)
                    yield return new(item.Title, Snippet(item, query), Score(item, query));
        }
    }
    public static IEnumerable<Book> Scan()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "../Content");
        string[] archivos = Directory.GetFiles(path, "*.txt");
        foreach (var item in archivos)
        {
            Dictionary<string,float> repetitios = new();
            StreamReader sr = new StreamReader(item);
            string text = sr.ReadToEnd();
            var separators = text.Where(x => Char.IsPunctuation(x) || Char.IsSeparator(x) || Char.IsWhiteSpace(x)).Distinct().ToArray();
            var words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (repetitios.ContainsKey(word)) repetitios[word] += 1;
                else repetitios.Add(word, 1);
            }
            yield return new Book(Path.GetFileNameWithoutExtension(item),repetitios,text,words);
        }
    }
    #endregion

    #region Utils
    private static bool Exists<T>(int i, IEnumerable<T> a) => (i >= 0 && i < a.Count());
    #endregion

    #region Main
    private static string Suggestion(string query)
    {
        string[] x = query.Split(' ');
        string temp = "";
        foreach (var item in x)
        {
            if (Words.Contains(item)) temp += item + " ";
            else
            {
                Dictionary<string, int> y = new();
                bool cumple = false;
                foreach (var item2 in Words)
                {
                    if (y.ContainsKey(item2)) continue;
                    int f = LD(item, item2);
                    if (f == 1)
                    {
                        temp += item2 + " ";
                        cumple = true;
                        break;
                    }
                    if (f < 3) y.Add(item2, f);
                }
                if (y.Count != 0)
                    if (!cumple) temp += y.MinBy(x => x.Value).Key + " ";
            }
        }
        return temp;
    }
    private static string Snippet(Book book, string query)
    {
        string[] words = query.Split(' ');
        string result = "";
        foreach (var item in words)
        {
            string temp = "";
            int x = book.Words.ToList().FindIndex(x => x == item);
            for (int i = -5; i < 5; i++)
            {
                if (Exists(x + i, book.Words)) temp += book.Words[x + i] + " ";
            }
            result += temp + " ... ";
        }
        return result;
    }
    private static float Score(Book book, string query)
    {
        string[] words = query.Split(' ');
        float count = 0;
        foreach (var item in words)
        {
            count += TF(book, item) * IDF(item);
        }
        return count;
    }
    #endregion

    #region Math
    private static int LD(string source1, string source2)
    {
        var source1Length = source1.Length;
        var source2Length = source2.Length;

        var matrix = new int[source1Length + 1, source2Length + 1];

        if (source1Length == 0)
            return source2Length;

        if (source2Length == 0)
            return source1Length;

        for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
        for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

        for (var i = 1; i <= source1Length; i++)
        {
            for (var j = 1; j <= source2Length; j++)
            {
                var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }
        return matrix[source1Length, source2Length];
    }
    private static float IDF(string word) => ((float)Math.Log((float)Books.Count(x => x.Repetitions.Keys.Contains(word)) / Books.Count()));
    private static float TF(Book book, string word) => book.Repetitions.ContainsKey(word) ? book.Repetitions[word] / book.Repetitions.Values.Sum() : 0f;
    #endregion

}