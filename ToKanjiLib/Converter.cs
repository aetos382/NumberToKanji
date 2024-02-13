using System;

namespace ToKanjiLib;

public static class Converter
{
    private const string Chars1 = "〇一二三四五六七八九";
    private const string Chars2 = "一十百千";
    private const string Chars3 = "一万億兆";

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

#pragma warning disable IDE0055

        ReadOnlySpan<byte> indexTable =
        [
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 14,
            14, 14, 13, 13, 13, 12, 12, 12, 12, 11, 11, 11, 10, 10, 10,  9,
             9,  9,  9,  8,  8,  8,  7,  7,  7,  6,  6,  6,  6,  5,  5,  5,
             4,  4,  4,  3,  3,  3,  3,  2,  2,  2,  1,  1,  1,  0,  0,  0,
             0
        ];

        ReadOnlySpan<long> powersOf10 =
        [
                              1,
                             10,
                            100,
                           1000,
                         1_0000,
                        10_0000,
                       100_0000,
                      1000_0000,
                    1_0000_0000,
                   10_0000_0000,
                  100_0000_0000,
                 1000_0000_0000,
               1_0000_0000_0000,
              10_0000_0000_0000,
             100_0000_0000_0000,
            1000_0000_0000_0000
        ];

#pragma warning restore IDE0055

        var count = (int)indexTable[unchecked((int)long.LeadingZeroCount(number))];

        var (man, ju) = Math.DivRem(count, 4);

        var length = 0;

        for (var i = count; i >= 0; --i)
        {
            (var div, number) = Math.DivRem(number, powersOf10[i]);

            if (div != 0)
            {
                if (div > 1 || ju == 0 || man != 0)
                {
                    destination[length++] = Chars1[(int)div];
                }

                if (ju != 0)
                {
                    destination[length++] = Chars2[ju];
                }
            }

            if (man != 0 && ju == 0)
            {
                destination[length++] = Chars3[man];
            }

            --ju;

            if (ju < 0)
            {
                ju = 3;
                --man;
            }
        }

        return length;
    }

    public static string ToKanji(long number)
    {
        Span<char> buffer = stackalloc char[50];

        var filledChars = ToKanji(number, buffer);

        return buffer[..filledChars].ToString();
    }
}
