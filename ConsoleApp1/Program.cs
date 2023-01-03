using System.Threading.Channels;
using MoogleEngine;

// string[] words = { "casa", "cazador", "casamiento", "azul", "azulejo", "antena", "bufalo", "hierba", "zapato" };
//
// var trie = new Trie('^');
// Array.ForEach(words, x=>trie.Insert(x));
//
// var temp = trie.SearchByTolerance("azulado",2);
// Console.WriteLine(string.Join(' ', temp));

Console.WriteLine(Algorithms.LevenshteinDistance("casamiento","cazando"));