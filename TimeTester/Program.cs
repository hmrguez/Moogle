using MoogleEngine;
using Tester;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


internal class Program
{

    public static void Main()
    {
        var x = new Test(new(Wrapper.Scan,"Scan"),new(Wrapper.Search,"Search"));
        x.Run();
        System.Console.WriteLine(x);
    }
}

static class Wrapper
{
    public static void Scan() { Moogle.Scan(); }
    public static void Search() { Moogle.ParseSeparators("casa,string,qwooowe+qwe.qewpo"); }

}

