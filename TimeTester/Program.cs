using MoogleEngine;
using Tester;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


internal class Program
{

    public static void Main()
    {
        var x = new Test((System.Console.WriteLine, "CW"),
                         (Wrapper.Search, "Search"),
                         (Wrapper.Snippet, "Snippet"));
        x.Run();
        System.Console.WriteLine(x);
    }
}

static class Wrapper
{
    public static void Scan() { Moogle.Scan(); }
    public static void Search() { Moogle.Search("never ! prenda").ToArray(); }
    public static void Snippet() {Moogle.Snippet(Moogle.Books[0],"casa");}

}

