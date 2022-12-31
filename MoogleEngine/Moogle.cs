namespace MoogleEngine;

public static class Moogle
{
    public record Book(string Name, Trie Words);
    
    public static SearchResult Query(string query)
    {
        throw new NotImplementedException();
    }

    public static IEnumerable<Book> Scan()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "../Content");
        var files = Directory.GetFiles(path, "*.txt");

        foreach (var file in files)
        {
            var trie = new Trie('^');
            var sr = new StreamReader(file);
            var text = sr.ReadToEnd();

            var separators = text.Where(char.IsSeparator).ToArray();
            var words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                trie.Insert(word);
            }

            yield return new Book(file, trie);
        }
    }
}