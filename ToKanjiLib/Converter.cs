using System;

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

        var leadingZeros = long.LeadingZeroCount(number);

        var digits = leadingZeros switch
        {
            <= 14 => 16,
            <= 17 => 15,
            <= 20 => 14,
            <= 24 => 13,
            <= 27 => 12,
            <= 30 => 11,
            <= 34 => 10,
            <= 37 => 9,
            <= 40 => 8,
            <= 44 => 7,
            <= 47 => 6,
            <= 50 => 5,
            <= 54 => 4,
            <= 57 => 3,
            <= 60 => 2,
            _ => 1
        };

        var weight = (long)Math.Pow(10, digits - 1);

        var (man, ju) = Math.DivRem(digits - 1, 4);
        var hasMan = false;

        while (weight != 0)
        {
            var (div, rem) = Math.DivRem(number, weight);
            if (div == 0)
            {
                goto NEXT;
            }

            hasMan = true;

            if (div > 1 || ju == 0 || man > 0)
            {
                destination[length++] = Chars1[(int)div - 1];
            }

            if (ju > 0)
            {
                destination[length++] = Chars2[ju - 1];
            }

        NEXT:

            if (hasMan && man > 0 && ju == 0)
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
                hasMan = false;
            }
        }

        return length;
    }
}
