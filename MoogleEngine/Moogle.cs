﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MoogleEngine;

public static class Moogle
{
    public record Book(string Name, string Text, Trie Words);
    private static readonly Book[] Books = Scan().ToArray();
    
    public static SearchResult Query(string query)
    {
        var searchQuery = Search(query).ToArray();
        var suggestion = "";
        return new SearchResult(searchQuery, suggestion);
    }

    private static IEnumerable<SearchItem> Search(string query)
    {
        var sQuery = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var book in Books)
        {
            var score = JointScore(sQuery, book, Books);
            if(score <= 0) continue;

            var jointSnippet = StringManipulation.JointSnippet(sQuery, book);
            
            yield return new SearchItem(book.Name, jointSnippet, score);
        }
    }

    private static float JointScore(string[] sQuery, Book book, Book[] corpus)
    {

        var fQuery = sQuery.Select(StringManipulation.FormatQuery).ToArray();
        float allScore = 0;

        for (int i = 0; i < sQuery.Length; i++)
        {
            var word = sQuery[i];
            var fWord = fQuery[i];

            var score = Algorithms.Tfidf(fWord, book, corpus);

            if (word.StartsWith('^') && score == 0) return 0;
            if (word.StartsWith('!') && score > 0) return 0;
            
            allScore += score;
        }

        return allScore;
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