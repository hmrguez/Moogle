using System.IO;
using System;
using System.Linq;
namespace MoogleEngine;

public static class Moogle
{
    public record Book(string Title, Dictionary<string, float> Repetitions, string FullText, string[] Words)
    {
        public bool Contains(string word) => Words.Contains(word);
    }

    public static Book[] Books = Scan().ToArray();
    public static string[] Words = Books.SelectMany(x => x.Words).Distinct().ToArray();

    public static SearchResult Query(string query)
    {
        SearchItem[] x = Search(query).OrderByDescending(x => x.Score).ToArray();


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
        var x = query.ParseSeparators();
        foreach (var item in Books)
        {
            if (!Array.Exists(x, p => item.Contains(p))) continue;
            if (Score(item, query) != 0) yield return new(item.Title, Snippet(item, query), Score(item, query));
        }
    }
    public static IEnumerable<Book> Scan()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "../Content");
        string[] archivos = Directory.GetFiles(path, "*.txt");
        foreach (var item in archivos)
        {
            Dictionary<string, float> repetitios = new();
            StreamReader sr = new StreamReader(item);
            string text = sr.ReadToEnd();
            var separators = text.Where(x => Char.IsPunctuation(x) || Char.IsSeparator(x) || Char.IsWhiteSpace(x)).Distinct().ToArray();
            var words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (repetitios.ContainsKey(word)) repetitios[word] += 1;
                else repetitios.Add(word, 1);
            }
            yield return new Book(Path.GetFileNameWithoutExtension(item), repetitios, text, words);
        }
    }
    #endregion

    #region Utils
    private static bool Exists<T>(int i, IEnumerable<T> a) => (i >= 0 && i < a.Count());
    public static float TFIDF(Book book, string word) => IDF(word) * TF(book, word);
    public static string[] ParseSeparators(this string query)
    {
        var separators = query.Where(x => !Char.IsLetterOrDigit(x)).ToArray();
        string[] words = query.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        return words;
    }
    #endregion

    #region Main
    private static string Suggestion(string query)
    {
        string[] x = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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
        var words = query.ParseSeparators();
        string result = "";
        foreach (var item in words)
            if (book.Words.Contains(item))
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
        string[] words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        float count = 0;
        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            switch (word)
            {
                case "!" when book.Contains(words[i + 1]): return 0;
                case "^" when !book.Contains(words[i + 1]): return 0;
                case "^" when book.Contains(words[i + 1]):
                    {
                        count += TFIDF(book, words[i + 1]);
                        i += 1;
                        break;
                    }
                case string a when a.StartsWith('!') && book.Contains(a.Substring(1)): return 0;
                case string a when a.StartsWith('^') && !book.Contains(a.Substring(1)): return 0;
                case string a when a.StartsWith('^') && book.Contains(a.Substring(1)): count += TFIDF(book, a.Substring(1)); break;
                case string a when a.All(x => x == '+') && book.Contains(words[i + 1]): count += a.Length; break;
                case string a when a.All(x => x == '-') && book.Contains(words[i + 1]): count -= a.Length; break;
                case string a when a.StartsWith('+') && book.Contains(a.Substring(a.LastIndexOf('+') + 1)):
                    count += a.LastIndexOf('+') + 1 + TFIDF(book, a.Substring(a.LastIndexOf('+') + 1));
                    break;
                case string a when a.StartsWith('-') && book.Contains(a.Substring(a.LastIndexOf('-') + 1)):
                    count += -a.LastIndexOf('-') - 1 + TFIDF(book, a.Substring(a.LastIndexOf('-') + 1)); break;
                case "~":
                    {
                        int w1 = book.Words.ToList().IndexOf(words[i - 1]);
                        int w2 = book.Words.ToList().IndexOf(words[i + 1]);
                        count += (TFIDF(book, words[i - 1]) + TFIDF(book, words[i + 1])) * Math.Abs(w1 - w2);
                        i += 1;
                        break;
                    }
                case string a when a.Contains('~'):
                    {
                        string[] split = a.Split('~');
                        for (int j = 0; j < split.Length - 1; j++)
                        {
                            int w1 = Array.IndexOf(book.Words, split[i]);
                            int w2 = Array.IndexOf(book.Words, split[i + 1]);
                            count += (TFIDF(book, split[i]) + TFIDF(book, split[i + 1])) * Math.Abs(w1 - w2);
                        }
                        break;
                    }
                default: count += TFIDF(book, words[i]); break;
            }
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
    private static float IDF(string word)
    {

        float x = Books.Count(x => x.Words.Contains(word));
        if (x == 0) return 0;
        float y = Books.Length / x;
        return (float)Math.Log(y);
    }
    private static float TF(Book book, string word)
    {
        if (book.Contains(word)) return book.Repetitions[word] / book.Repetitions.Values.Sum();
        return 0;
    }
    #endregion

}