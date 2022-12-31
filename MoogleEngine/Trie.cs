namespace MoogleEngine;

public sealed class Trie
{
    public char Letter { get; set; }
    public int Reps { get; set; }
    public Trie[] Children { get; set; } = new Trie[27];

    public Trie(char letter) => Letter = letter;

    public (Trie, int) PrefixQuery(string word)
    {
        return PrefixQuery(word, 0);
    }

    public (Trie, int) PrefixQuery(string word, int pos)
    {
        if (pos >= word.Length) return (this, pos);
        var next = Utils.CharToIntParse(word[pos]);
        if (Children[next] is null) return (this, pos);
        return Children[next].PrefixQuery(word, pos + 1);
    }

    public void Insert(string word)
    {
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
        
        var next = Utils.CharToIntParse(word[pos]);
        Children[next] = new Trie(word[pos]);
        Children[next].Insert(word,pos+1);
    }
}