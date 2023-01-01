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

    public override string ToString() => Letter + " reps:" + Reps;
}