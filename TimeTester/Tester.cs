using System.Diagnostics;

namespace Tester;

public record Test(params (Action Actions, string name)[] casa)
{
    public string[,] table = new string[casa.Length, 2];
    public void Run()
    {
        int count = 0;
        foreach (var item in casa)
        {
            table[count,0] = item.Item2;
            Stopwatch sw = new();
            sw.Start();
            item.Item1();
            sw.Stop();
            table[count++,1] = sw.ElapsedMilliseconds.ToString();
        }
    }

    public override string ToString()
    {
        string temp = "";
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {   
                temp += table[i,j] + " | ";
            }
            temp+="\n" + "--------" + "\n";
        }
        return temp;
    }
}