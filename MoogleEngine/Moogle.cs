namespace MoogleEngine;

public static class Moogle
{
    public record Book(string Name, string Text, string LowerText, Trie Words);
    private static readonly Book[] Books = Scan().ToArray();
    
    public static SearchResult Query(string query)
    {
        var searchQuery = Search(query).ToArray();
        var suggestion = searchQuery.Length > 0 
            ? string.Empty 
            : Suggestion(query.ToLower().RemoveDiacritics().Split(' ', StringSplitOptions.RemoveEmptyEntries), 3);
        searchQuery = searchQuery.Length > 0
            ? searchQuery
            : Search(suggestion).ToArray();
        return new SearchResult(searchQuery, suggestion);
    }

    private static IEnumerable<SearchItem> Search(string query)
    {
        var sQuery = query.ToLower().RemoveDiacritics().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var book in Books)
        {
            var score = Score(sQuery, book, Books);
            if(score <= 0) continue;

            var jointSnippet = StringManipulation.JointSnippet(sQuery, book);
            
            yield return new SearchItem(book.Name, jointSnippet, score);
        }
    }

    private static float Score(string[] sQuery, Book book, Book[] corpus)
    {

        var fQuery = sQuery.Select(StringManipulation.FormatQuery).ToArray();
        float allScore = 0;

        for (int i = 0; i < sQuery.Length; i++)
        {
            var word = sQuery[i];
            var fWord = fQuery[i];
            
            if(i+1 < sQuery.Length && sQuery[i+1]=="~") continue;
            
            if (word == "~")
            {
                var sumScore = Algorithms.Tfidf(fQuery[i - 1], book, corpus) +
                               Algorithms.Tfidf(fQuery[i + 1], book, corpus);
                var tildeDistance = Algorithms.TildeDistance(fQuery[i - 1], fQuery[i + 1], book);
                
                if (tildeDistance < 0) return 0;
                
                allScore += sumScore / tildeDistance;

                i++;
                continue;
            }

            var score = Algorithms.Tfidf(fWord, book, corpus);
            
            if (word.StartsWith('^') && score == 0) return 0;
            if (word.StartsWith('!') && score > 0) return 0;
            if (word.StartsWith('*')) score*= 1+StringManipulation.GetLastCharOfStar(word);
            
            allScore += score;
        }

        return allScore;
    }

    private static string Suggestion(IEnumerable<string> sQuery, int tolerance)
    {
        var suggestions = sQuery.Select(StringManipulation.FormatQuery).Select(x=>Suggestion(x,tolerance));
        var result = string.Join(' ', suggestions);
        return result;
    }

    private static string Suggestion(string word, int tolerance)
    {
        return Books
            .SelectMany(book => book.Words.SearchByTolerance(word, tolerance))
            .MinBy(x => Algorithms.LevenshteinDistance(x, word));
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
            var lowerText = text.ToLower().RemoveDiacritics();
            
            var separators = lowerText
                .Where(x=>char.IsSeparator(x) || char.IsPunctuation(x) || char.IsWhiteSpace(x))
                .Distinct()
                .ToArray();
            
            var words = lowerText.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
                trie.Insert(word);

            yield return new Book(Path.GetFileNameWithoutExtension(file), text, lowerText, trie);
        }
    }
}