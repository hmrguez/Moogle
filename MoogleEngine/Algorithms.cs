namespace MoogleEngine;

public static class Algorithms
{
    public static IEnumerable<int> KnuthMorrisPratt(string t, string p)
    {
        var pi = PrefixFunction(p);
        var q = 0;

        for (int i = 0; i < t.Length; i++)
        {
            while (p[q]!=t[i] && q>0)
            {
                q = pi[q - 1];
            }

            if (p[q] == t[i])
            {
                q++;
            }
            if (q == p.Length)
            {
                q = pi[q - 1];

                if(i-p.Length >= 0 && char.IsLetterOrDigit(t[i-p.Length])) continue;
                if(i+1 < t.Length && char.IsLetterOrDigit(t[i+1])) continue;
                
                yield return i - p.Length + 1;
            }
        }
    }

    private static int[] PrefixFunction(string word)
    {
        var pFunction = new int[word.Length];
        var q = 0;

        for (var i = 1; i < pFunction.Length; i++)
        {
            while (word[i] != word[q] && q > 0)
                q = pFunction[q - 1];

            if (word[i] == word[q])
                q++;

            pFunction[i] = q;
        }

        return pFunction;
    }

    public static int LevenshteinDistance(string src1, string src2)
    {
        throw new NotImplementedException();
    }

    public static float Tfidf(string word, Moogle.Book book, Moogle.Book[] corpus)
    {
        var head = book.Words.Head(word);
        if (head is null) return 0;
        float tf = (float)head.Reps / (float)book.Words.WordCount;
        var idf = MathF.Log((float)corpus.Length / (float)(1 + corpus.Count(x => x.Words.Contains(word))));

        return tf * idf;
    }
}