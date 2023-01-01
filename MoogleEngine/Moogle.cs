using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MoogleEngine;

public static class Moogle
{
    private record Book(string Name, string Text, Trie Words);
    private static readonly Book[] Books = Scan().ToArray();
    
    public static SearchResult Query(string query)
    {
        var temp = Books.Select(x => new SearchItem(x.Name, "12", 1));
        var temp2 = new SearchResult(temp.ToArray(), "as");
        return temp2;
    }

    public static IEnumerable<SearchItem> Search(string query)
    {
        var sQuery = query.Split(' ');

        foreach (var book in Books)
        {
            var score = sQuery.Sum(word => Score(word, book, Books));
            if(score < 0) continue;
            
            var snippets = sQuery.Select(word => Snippet(word, book)).Where(x=>x!=string.Empty);
            var jointSnippet = string.Join(" ", snippets);
            yield return new SearchItem(book.Name, jointSnippet, score);
        }
    }
    
    private static float Score(string word, Book book, Book[] corpus)
    {
        var head = book.Words.Head(word);
        var tf = head?.Reps / book.Words.WordCount ?? 0;
        var idf = MathF.Log((float)corpus.Length / (float)(1 + corpus.Count(x => x.Words.Contains(word))));

        return tf * idf;
    }

    private static string Snippet(string word, Book book)
    {
        var temp = book.Text;
        var firstPosition = Algorithms.KnuthMorrisPratt(book.Text, word).FirstOrDefault();
        if (firstPosition==0) return string.Empty;
        var sb = new StringBuilder();
        
        for (int i = -40; i < 40; i++)
        {
            int positionInText = firstPosition + i;
            if (positionInText >= 0 && positionInText < temp.Length)
                sb.Append(temp[positionInText]);
        }

        return sb.ToString();
    }

    private static IEnumerable<Book> Scan()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../Content");
        var files = Directory.GetFiles(path, "*.txt");
        
        foreach (var file in files)
        {
            var trie = new Trie('^');
            var sr = new StreamReader(file);
            var text = sr.ReadToEnd();
            var lowerText = text.ToLower(); 
                
            var separators = lowerText
                .Where(x=>char.IsSeparator(x) || char.IsPunctuation(x) || char.IsWhiteSpace(x))
                .Distinct()
                .ToArray();
            
            var words = lowerText.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                trie.Insert(word);
            }

            yield return new Book(Path.GetFileNameWithoutExtension(file), text, trie);
        }
    }
}