﻿using System.Diagnostics.CodeAnalysis;

namespace MoogleEngine;

public static class Moogle
{
    public record Book(string Name, string Text, Trie Words);

    private static Book[] _books = Scan().ToArray();
    
    public static SearchResult Query(string query)
    {
        var temp = _books.Select(x => new SearchItem(x.Name, "12", 1));
        var temp2 = new SearchResult(temp.ToArray(), "as");
        return temp2;
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