using System.Text;

namespace MoogleEngine;

public static class StringManipulation
{
    public static string JointSnippet(string[] sQuery, Moogle.Book book)
    {
        var fQuery = sQuery.Select(FormatQuery);
        
        var snippets = fQuery.Select(word => Snippet(word, book)).Where(x=>x!=string.Empty);
        var jointSnippet = string.Join(" ... ", snippets);

        return jointSnippet;
    }
    
    public static string Snippet(string word, Moogle.Book book)
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
    
    public static string FormatQuery(string word) => word.StartsWith('^') || word.StartsWith('!') ? word[1..] : word;
}