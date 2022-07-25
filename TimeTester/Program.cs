using MoogleEngine;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Stopwatch sw = new();
        sw.Start();
        var words = Moogle.Words;
        sw.Stop();
        System.Console.WriteLine(sw.ElapsedMilliseconds);
    }
}
