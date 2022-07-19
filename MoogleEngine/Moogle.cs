using System.IO;
using System;
using System.Linq;
namespace MoogleEngine;

public static class Moogle
{
    public static IEnumerable<Book> Books = Scan();
    public static SearchResult Query(string query)
    {
        return new SearchResult(Search(query).ToArray(), " ");
    }
    public static IEnumerable<SearchItem> Search(string query)
    {
        var x = query.Split(' ').ToList();
        foreach (var item in Books)
        {

            if(x.Exists(p=>!item.Repetitions.Keys.Contains(p))) continue;

            if (Score(item, query) != 0) yield return new SearchItem(item.Title, Snippet(item,query), Score(item, query));
        }
    }
    public static bool Exists<T>(int i, IEnumerable<T> a) => (i>=0 && i<a.Count());
    public static string Snippet(Book book, string query){
        string[] words = query.Split(' ');
        string result = "";
        foreach (var item in words)
        {
            string temp = "";
            int x = book.SplittedText.ToList().FindIndex(x=>x==item);
            for (int i = -5; i < 5; i++)
            {
                if(Exists(x+i,book.SplittedText)) temp+= book.SplittedText[x+i] + " ";
            }
            result += temp + " ... ";
        }
        return result;
    }
    public static float Score(Book book, string query)
    {
        string[] words = query.Split(' ');
        float count = 0;
        foreach (var item in words)
        {
            count += TF(book, item)*IDF(item);
        }
        return count;
    }
    public static float IDF(string word) => (float)Books.Count(x=>x.Repetitions.Keys.Contains(word))/Books.Count();
    public static float TF(Book book, string word) => book.Repetitions.ContainsKey(word) ? book.Repetitions[word] / book.Repetitions.Values.Sum() : 0f;
    public static IEnumerable<Book> Scan()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "../Content");
        string[] archivos = Directory.GetFiles(path, "*.txt");
        foreach (var item in archivos)
        {
            Dictionary<string, float> rep = new();
            StreamReader sr = new StreamReader(item);
            string text = sr.ReadToEnd();
            string[] words = text.Split(new char[] { ' ', ',', '.', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (!rep.ContainsKey(word)) rep.Add(word, 1);
                else rep[word] += 1;
            }
            yield return new Book(Path.GetFileNameWithoutExtension(item), text, rep);
        }
    }
}