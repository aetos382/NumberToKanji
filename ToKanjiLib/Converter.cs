﻿using System;

namespace ToKanjiLib;

public static class Converter
{
    private const string Chars1 = "一二三四五六七八九";
    private const string Chars2 = "十百千";
    private const string Chars3 = "万億兆";

    public static int ToKanji(
        long number,
        Span<char> destination)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, 9999_9999_9999_9999);

        if (number == 0)
        {
            destination[0] = '〇';
            return 1;
        }

        var length = 0;

        var numberOfDigitsMinus1 = long.LeadingZeroCount(number) switch
        {
            <= 14 => 15,
            <= 17 => 14,
            <= 20 => 13,
            <= 24 => 12,
            <= 27 => 11,
            <= 30 => 10,
            <= 34 => 9,
            <= 37 => 8,
            <= 40 => 7,
            <= 44 => 6,
            <= 47 => 5,
            <= 50 => 4,
            <= 54 => 3,
            <= 57 => 2,
            <= 60 => 1,
            _ => 0
        };

        var weight = (long)Math.Pow(10, numberOfDigitsMinus1);

        var (man, ju) = Math.DivRem(numberOfDigitsMinus1, 4);

        while (weight != 0)
        {
            var (div, rem) = Math.DivRem(number, weight);
            if (div == 0)
            {
                goto NEXT;
            }

            if (div > 1 || ju == 0 || man > 0)
            {
                destination[length++] = Chars1[(int)div - 1];
            }

            if (ju > 0)
            {
                destination[length++] = Chars2[ju - 1];
            }

        NEXT:

            if (man > 0 && ju == 0)
            {
                destination[length++] = Chars3[man - 1];
            }

            number = rem;
            weight /= 10;
            --ju;

            if (ju < 0)
            {
                ju = 3;
                --man;
            }
        }

        return length;
    }
}
