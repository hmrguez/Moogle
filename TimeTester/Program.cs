using MoogleEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


internal class Program
{
    private static void Main(string[] args)
    {
        Stopwatch sw = new();
        sw.Start();
        Moogle.Query("casa");
        sw.Stop();
        System.Console.WriteLine(sw.ElapsedMilliseconds);
    }
}
