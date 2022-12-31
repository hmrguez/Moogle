namespace MoogleEngine;

public class Trie
{
    public char Value { get; init; }
    public Trie[] Children { get; init; }
    public int Repetitions { get; set; }

    public Trie(char value)
    {
        Value = value;
        Children = new Trie[26];
    }
    
    public (Trie, int) PrefixQuery(string word) => PrefixQuery(this, word);
    private (Trie, int) PrefixQuery(Trie trie, string word)
    {
        var node = trie;
        var c = 0;

        while (c < word.Length && node?.Children[Utils.CharToIntParse(word[c])] != null)
        {
            node = node.Children[Utils.CharToIntParse(word[c])];
            c++;
        }
        
        return (node, c);
    }

    public void Insert(string word)
    {
        var pQuery = PrefixQuery(word);
        pQuery.Item1.Insert(word,pQuery.Item2);
    }
    private void Insert(string word, int pos)
    {
        if (word.Length >= pos)
        {
            Repetitions++;
            return;
        }

        var charToIntParse = Utils.CharToIntParse(word[pos]);
        Children[charToIntParse] = new Trie(word[pos]);
        
        Children[charToIntParse].Insert(word,pos+1);
    }

    public bool Contains(string word) => PrefixQuery(word).Item2 == word.Length - 1;
}