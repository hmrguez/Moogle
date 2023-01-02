using System.Globalization;

namespace MoogleEngine;

public sealed class Trie
{
    public int WordCount { get; set; }
    public char Letter { get; set; }
    public int Reps { get; set; }
    public List<Trie> Children { get; set; } = new();

    public Trie(char letter) => Letter = letter;

    private (Trie, int) PrefixQuery(string word) => PrefixQuery(word, 0);
    private (Trie, int) PrefixQuery(string word, int pos)
    {
        if (pos >= word.Length) return (this, pos);
        var next = Next(word[pos]);
        return next?.PrefixQuery(word, pos + 1) ?? (this, pos);
    }

    public void Insert(string word)
    {
        WordCount++;
        var pQuery = PrefixQuery(word);
        pQuery.Item1.Insert(word, pQuery.Item2);
    }
    private void Insert(string word, int pos)
    {
        if (pos >= word.Length)
        {
            Reps++;
            return;
        }

        var next = Next(word[pos]);
        if (next is null)
        {
            next = new Trie(word[pos]);
            Children.Add(next);
        }
        
        next.Insert(word,pos+1);
    }

    public Trie Head(string word)
    {
        var pQuery = PrefixQuery(word);
        return pQuery.Item2 == word.Length && pQuery.Item1.Reps > 0 ? pQuery.Item1 : null;
    }

    private Trie Next(char c) => Children.Where(x => x is not null).FirstOrDefault(x => x.Letter == c);

    public bool Contains(string word)
    {
        var pQuery = PrefixQuery(word);
        return pQuery.Item2 == word.Length && pQuery.Item1.Reps > 0;
    }

    public IEnumerable<string> SearchByTolerance(string similarTo, int tolerance) 
        => SearchByTolerance("", similarTo, 0, tolerance, tolerance);

    private IEnumerable<string> SearchByTolerance(string word, string similarTo, int pos, int tolerance, int initTolerance)
    {
        if (tolerance == -1) yield break;
        
        if (Reps != 0 && (tolerance >= Math.Abs(similarTo.Length - word.Length)))
            yield return word;
            
        foreach (var child in Children)
        {
            if (pos < similarTo.Length && similarTo[pos] == child.Letter)
                foreach (var item in child.SearchByTolerance(word+child.Letter,similarTo,pos+1,tolerance, initTolerance))
                    yield return item;
            else
                foreach (var item in child.SearchByTolerance(word+child.Letter,similarTo,pos+1,tolerance-1,initTolerance))
                    yield return item;
        }
    }

    public override string ToString() => Letter + " reps:" + Reps;
}