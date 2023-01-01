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
                q++;
            if (q == p.Length)
            {
                yield return i - p.Length + 1;
                q = pi[q - 1];
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
}