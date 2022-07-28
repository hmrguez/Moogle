using MoogleEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


internal class Program
{
    private static void Main(string[] args)
    {
        var x = Moogle.Scan();
        x.ForAll(System.Console.WriteLine);
    }
}
static class Utils{
    public static void ForAll<T>(this IEnumerable<T> enumerable, Action<T> action){
        foreach (var item in enumerable)
        {
            action(item);
        }
    }
}
