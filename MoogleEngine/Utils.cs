﻿namespace MoogleEngine;

public static class Utils
{
    public static int CharToIntParse(char c) =>
        c switch
        {
            'a' => 0,
            'b' => 1,
            'c' => 2,
            'd' => 3,
            'e' => 4,
            'f' => 5,
            'g' => 6,
            'h' => 7,
            'i' => 8,
            'j' => 9,
            'k' => 10,
            'l' => 11,
            'm' => 12,
            'n' => 13,
            'ñ' => 14,
            'o' => 15,
            'p' => 16,
            'q' => 17,
            'r' => 18,
            's' => 19,
            't' => 20,
            'u' => 21,
            'v' => 22,
            'w' => 23,
            'x' => 24,
            'y' => 25,
            'z' => 26,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
}