using System.Globalization;
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
    
    private static string Snippet(string word, Moogle.Book book)
    {
        if(word=="~") return string.Empty;
        ReadOnlySpan<char> text = book.Text;
        var firstPosition = Algorithms.KnuthMorrisPratt(book.LowerText, word).FirstOrDefault();
        if (firstPosition==0) return string.Empty;

        int startIndex = int.MaxValue;
        int endIndex = int.MinValue;
        
        for (int i = -40; i < 40; i++)
        {
            int currentPosition = firstPosition + i;
            if (currentPosition < startIndex)
                startIndex = currentPosition;
            if (currentPosition > endIndex)
                endIndex = currentPosition;
        }

        return text.Slice(startIndex, endIndex-startIndex-1).ToString();
    }
    
    public static string FormatQuery(string word)
    {
        if (word.StartsWith('*'))
            return word[GetLastCharOfStar(word)..];
        if (word.StartsWith('^') || word.StartsWith('!'))
            return word[1..];
        else
            return word;
    }
    
    public static string RemoveDiacritics(this string text)
    {
        ReadOnlySpan<char> normalizedString = text.Normalize(NormalizationForm.FormD);
        int i = 0;

        Span<char> span = text.Length < 1000 ? stackalloc char[text.Length] : new char[text.Length];

        foreach (char c in normalizedString)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                span[i++] = c;

        return new string(span).Normalize(NormalizationForm.FormC);
    }

    public static int GetLastCharOfStar(string word)
    {
        for (int i = 0; i < word.Length; i++)
            if (word[i] != '*')
                return i;
        
        throw new Exception("This shouldn't Happen");
    }
    
    
}