using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ToKanjiLib;

public static class Converter
{
    private const string Chars1 = "〇一二三四五六七八九";
    private const string Chars2 = "一十百千";
    private const string Chars3 = "一万億兆";

    public static int ToKanji<T>(
        T number,
        Span<char> destination)
        where T :
            IBinaryInteger<T>
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, T.Zero);

        if (!ValueHolder<T>.Supported)
        {
            throw new NotSupportedException();
        }

        if (number == T.Zero)
        {
            destination[0] = '〇';
            return 1;
        }

        #pragma warning disable IDE0055

        ReadOnlySpan<byte> table =
        [
             0,  0,  0,  1,  1,  1,  2,  2,  2,  3,  3,  3,  3,  4,  4,  4,
             5,  5,  5,  6,  6,  6,  6,  7,  7,  7,  8,  8,  8,  9,  9,  9,
             9, 10, 10, 10, 11, 11, 11, 12, 12, 12, 12, 13, 13, 13, 14, 14,
            14, 15, 15, 15, 15, 16, 16, 16, 17, 17, 17, 18, 18, 18, 18, 19
        ];

        var q = ValueHolder<T>.MaxPowerOf10;
        var p = new List<T>(q);
        var v = T.One;

        for (int i = 0; i < q; ++i)
        {
            p.Add(v);

            v *= ValueHolder<T>.Ten;
        }

        ReadOnlySpan<T> powersOf10 = CollectionsMarshal.AsSpan(p);

        ReadOnlySpan<char> digits = ['〇', '一', '二', '三', '四', '五', '六', '七', '八', '九'];
        ReadOnlySpan<char> units1 = ['一', '十', '百', '千'];
        ReadOnlySpan<string> units2 = [
            "一", "万", "億", "兆",
            "京", "垓", "𥝱", "穣",
            "溝", "澗", "正", "載",
            "極", "恒河沙", "阿僧祇", "那由他",
            "不可思議", "無量大数"
        ];

#pragma warning restore IDE0055

        var count = (int)table.Ref(T.Log2(number));

        if (number < powersOf10.Ref(count))
        {
            --count;
        }

        Debug.Assert((count + 1) == number.ToString().Length);

        var (man, ju) = Math.DivRem(count, 4);

        var length = 0;
        var addMan = false;

        for (var i = count; i >= 0; --i)
        {
            var (div, rem) = T.DivRem(number, powersOf10.Ref(i));

            if (div != T.Zero)
            {
                if (div > T.One || ju == 0 || man > 0)
                {
                    destination[length++] = digits.Ref(div);
                    // addMan = number >= 10000;
                    addMan = true;
                }

                if (ju > 0)
                {
                    destination[length++] = units1.Ref(ju);
                    // addMan |= number >= 10000;
                    addMan |= true;
                }
            }

            number = rem;

            if (ju > 0)
            {
                --ju;
            }
            else
            {
                if (man > 0 && addMan)
                {
                    var u = units2.Ref(man);
                    u.CopyTo(destination[length..]);
                    length += u.Length;

                    addMan = false;
                }

                if (number == T.Zero && !addMan)
                {
                    break;
                }

                ju = 3;
                --man;
            }
        }

        return length;
    }

    private static class ValueHolder<T>
        where T : IBinaryInteger<T>
    {
        public static bool Supported;
        public static T Ten;
        public static T MaxValue;
        public static int MaxPowerOf10;
    }

    static Converter()
    {
        ValueHolder<byte>.Supported = true;
        ValueHolder<byte>.Ten = 10;
        ValueHolder<byte>.MaxValue = byte.MaxValue;
        ValueHolder<byte>.MaxPowerOf10 = 2;

        ValueHolder<sbyte>.Supported = true;
        ValueHolder<sbyte>.Ten = 10;
        ValueHolder<sbyte>.MaxValue = sbyte.MaxValue;
        ValueHolder<sbyte>.MaxPowerOf10 = 2;

        ValueHolder<char>.Supported = true;
        ValueHolder<char>.Ten = (char)10;
        ValueHolder<char>.MaxValue = char.MaxValue;
        ValueHolder<char>.MaxPowerOf10 = 4;

        ValueHolder<short>.Supported = true;
        ValueHolder<short>.Ten = 10;
        ValueHolder<short>.MaxValue = short.MaxValue;
        ValueHolder<short>.MaxPowerOf10 = 4;

        ValueHolder<ushort>.Supported = true;
        ValueHolder<ushort>.Ten = 10;
        ValueHolder<ushort>.MaxValue = ushort.MaxValue;
        ValueHolder<ushort>.MaxPowerOf10 = 4;

        ValueHolder<int>.Supported = true;
        ValueHolder<int>.Ten = 10;
        ValueHolder<int>.MaxValue = int.MaxValue;
        ValueHolder<int>.MaxPowerOf10 = 9;

        ValueHolder<uint>.Supported = true;
        ValueHolder<uint>.Ten = 10;
        ValueHolder<uint>.MaxValue = uint.MaxValue;
        ValueHolder<uint>.MaxPowerOf10 = 9;

        ValueHolder<long>.Supported = true;
        ValueHolder<long>.Ten = 10;
        ValueHolder<long>.MaxValue = long.MaxValue;
        ValueHolder<long>.MaxPowerOf10 = 19;

        ValueHolder<ulong>.Supported = true;
        ValueHolder<ulong>.Ten = 10;
        ValueHolder<ulong>.MaxValue = ulong.MaxValue;
        ValueHolder<ulong>.MaxPowerOf10 = 19;

        ValueHolder<nint>.Supported = true;
        ValueHolder<nint>.Ten = 10;
        ValueHolder<nint>.MaxValue = nint.MaxValue;
        ValueHolder<nint>.MaxPowerOf10 = Environment.Is64BitProcess
            ? ValueHolder<long>.MaxPowerOf10
            : ValueHolder<int>.MaxPowerOf10;

        ValueHolder<nuint>.Supported = true;
        ValueHolder<nuint>.Ten = 10;
        ValueHolder<nuint>.MaxValue = nuint.MaxValue;
        ValueHolder<nint>.MaxPowerOf10 = Environment.Is64BitProcess
            ? ValueHolder<ulong>.MaxPowerOf10
            : ValueHolder<uint>.MaxPowerOf10;

        ValueHolder<Int128>.Supported = true;
        ValueHolder<Int128>.Ten = 10;
        ValueHolder<Int128>.MaxValue = Int128.MaxValue;
        ValueHolder<Int128>.MaxPowerOf10 = 38;

        ValueHolder<UInt128>.Supported = true;
        ValueHolder<UInt128>.Ten = 10;
        ValueHolder<UInt128>.MaxValue = UInt128.MaxValue;
        ValueHolder<UInt128>.MaxPowerOf10 = 38;

        ValueHolder<BigInteger>.Supported = true;
        ValueHolder<BigInteger>.Ten = 10;
        ValueHolder<BigInteger>.MaxValue = UInt128.MaxValue;
        ValueHolder<BigInteger>.MaxPowerOf10 = 68;
    }

    public static int ToKanjiX(
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

        ReadOnlySpan<byte> table =
        [
             0,  0,  0,  1,  1,  1,  2,  2,  2,  3,  3,  3,  3,  4,  4,  4,
             5,  5,  5,  6,  6,  6,  6,  7,  7,  7,  8,  8,  8,  9,  9,  9,
             9, 10, 10, 10, 11, 11, 11, 12, 12, 12, 12, 13, 13, 13, 14, 14,
            14, 15, 15, 15, 15, 16, 16, 16, 17, 17, 17, 18, 18, 18, 18, 19
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
              1000_0000_0000_0000,
            1_0000_0000_0000_0000
        ];

#pragma warning restore IDE0055

        var count = (int)table[BitOperations.Log2(unchecked((ulong)number))];

        if (number < powersOf10[count])
        {
            --count;
        }

        Debug.Assert((count + 1) == number.ToString(CultureInfo.InvariantCulture).Length);

        var (man, ju) = Math.DivRem(count, 4);

        var length = 0;
        var addMan = false;

        for (var i = count; i >= 0; --i)
        {
            var (div, rem) = Math.DivRem(number, powersOf10[i]);

            if (div != 0)
            {
                if (div > 1 || ju == 0 || man > 0)
                {
                    destination[length++] = Chars1[(int)div];
                    addMan = number >= 10000;
                }

                if (ju > 0)
                {
                    destination[length++] = Chars2[ju];
                    addMan |= number >= 10000;
                }
            }

            number = rem;

            if (ju > 0)
            {
                --ju;
            }
            else
            {
                if (man > 0 && addMan)
                {
                    destination[length++] = Chars3[man];
                    addMan = false;
                }

                if (number == 0 && !addMan)
                {
                    break;
                }

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
