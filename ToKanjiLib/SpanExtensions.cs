using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ToKanjiLib;

internal static class SpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T Ref<T>(this ReadOnlySpan<T> span, nuint offset)
    {
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Ref<T>(this Span<T> span, nuint offset)
    {
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T Ref<T, TOffset>(this ReadOnlySpan<T> span, TOffset offset)
        where TOffset : IBinaryInteger<TOffset>
    {
        return ref Ref(span, nuint.CreateChecked(offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Ref<T, TOffset>(this Span<T> span, TOffset offset)
        where TOffset : IBinaryInteger<TOffset>
    {
        return ref Ref(span, nuint.CreateChecked(offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T Ref<T>(this ReadOnlySpan<T> span, int offset)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, 0);

        return ref Ref(span, unchecked((nuint)offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Ref<T>(this Span<T> span, uint offset)
    {
        return ref Ref(span, (nuint)offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T Ref<T>(this ReadOnlySpan<T> span, long offset)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(offset, 0);

        return ref Ref(span, checked((nuint)offset));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Ref<T>(this Span<T> span, ulong offset)
    {
        return ref Ref(span, checked((nuint)offset));
    }
}
