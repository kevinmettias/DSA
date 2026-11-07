using System.Globalization;

namespace DSAExperimentation.Domain.Mathematics.SetTheory;

// A cardinal number, the size of a set: two sets have the same size when a one-to-one pairing
// matches them up. A finite set's size is a count. The infinite sizes stated here are the beth
// numbers: ℶ0 is ℵ0, the size of the naturals, and so of the integers and the rationals, which can
// be listed one by one; each next beth is the size of the previous one's subsets, 2 to its power,
// which Cantor's theorem makes larger, so ℶ1, the continuum, is the size of the reals. Whether the
// continuum is ℵ1, the first size after ℵ0, is the continuum hypothesis, which the usual axioms
// (ZFC) neither prove nor refute (Gödel 1940, Cohen 1963), so the continuum is named as a beth,
// where it is exact, and not as an aleph.
//
// The arithmetic is ZFC's, with choice: a sum or product of two sizes, either infinite, is the
// larger of them (a product with zero is zero), so ℵ0 + 1 = 1 + ℵ0 = ℵ0. Adding one element to an
// infinite set does not change its size, which is where cardinals part from ordinals (Ordinal): ω +
// 1 is a longer order than ω, though both have ℵ0 elements.
/// <summary>
/// A cardinal number: a finite count, or an infinite beth number such as ℵ0 or the continuum.
/// </summary>
public readonly record struct Cardinal : IComparable<Cardinal>
{
    private const string InfiniteCountMessage = "An infinite cardinal has no count.";
    private const string FiniteBethMessage = "A finite cardinal is no beth number.";
    private const string PowerTooLargeMessage = "2 to this count does not fit a long.";
    private const string FiniteFormat = "Cardinal {{ Count = {0} }}";
    private const string BethFormat = "Cardinal {{ BethIndex = {0} }}";

    // The finite count, when the cardinal is finite; zero otherwise.
    private readonly long _count;

    // The beth index plus one, so that zero, the default, is a finite cardinal.
    private readonly int _bethPlusOne;

    // The largest power of two a long holds, read from long's own largest value: 2^62.
    private static readonly int LargestPowerOfTwoExponent = (int)long.Log2(long.MaxValue);

    private bool IsZero => IsFinite && _count == 0;

    /// <summary>Gets ℵ0, the size of the naturals, the smallest infinite cardinal.</summary>
    public static Cardinal AlephNull { get; } = Beth(0);

    /// <summary>Gets the continuum, 2 to the power ℵ0, the size of the reals.</summary>
    public static Cardinal Continuum { get; } = Beth(1);

    /// <summary>Gets a value indicating whether the cardinal is a finite count.</summary>
    public bool IsFinite => _bethPlusOne == 0;

    /// <summary>Gets the count of a finite cardinal.</summary>
    /// <exception cref="InvalidOperationException">The cardinal is infinite.</exception>
    public long Count
    {
        get
        {
            if (!IsFinite)
            {
                throw new InvalidOperationException(InfiniteCountMessage);
            }

            return _count;
        }
    }

    /// <summary>
    /// Gets the beth index of an infinite cardinal: 0 for ℵ0, 1 for the continuum.
    /// </summary>
    /// <exception cref="InvalidOperationException">The cardinal is finite.</exception>
    public int BethIndex
    {
        get
        {
            if (IsFinite)
            {
                throw new InvalidOperationException(FiniteBethMessage);
            }

            return _bethPlusOne - 1;
        }
    }

    private Cardinal(long count, int bethPlusOne)
    {
        _count = count;
        _bethPlusOne = bethPlusOne;
    }

    /// <summary>The size of a finite set.</summary>
    /// <param name="count">The number of elements, at least zero.</param>
    /// <returns>The finite cardinal.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="count"/> is negative.
    /// </exception>
    public static Cardinal Finite(long count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return new(count, 0);
    }

    /// <summary>
    /// The beth number of an index: ℵ0 at 0, and each next one 2 to the previous one's power.
    /// </summary>
    /// <param name="index">The beth index, at least zero.</param>
    /// <returns>The infinite cardinal.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is negative.
    /// </exception>
    public static Cardinal Beth(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        return new(0, index + 1);
    }

    /// <summary>The size of the union of two disjoint sets of these sizes.</summary>
    /// <param name="other">The other set's size.</param>
    /// <returns>The finite sum, or the larger size when either is infinite.</returns>
    public Cardinal Plus(Cardinal other)
    {
        if (IsFinite && other.IsFinite)
        {
            return Finite(checked(_count + other._count));
        }

        return Max(this, other);
    }

    /// <summary>The size of the set of pairs drawn from two sets of these sizes.</summary>
    /// <param name="other">The other set's size.</param>
    /// <returns>The finite product; zero when either is zero; otherwise the larger size.</returns>
    public Cardinal Times(Cardinal other)
    {
        if (IsFinite && other.IsFinite)
        {
            return Finite(checked(_count * other._count));
        }

        if (IsZero || other.IsZero)
        {
            return Finite(0);
        }

        return Max(this, other);
    }

    /// <summary>The size of the set of all subsets of a set of this size, 2 to its power.</summary>
    /// <returns>
    /// 2 to the count for a finite cardinal; the next beth number for an infinite one.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// 2 to a finite count does not fit a long.
    /// </exception>
    public Cardinal PowerSet()
    {
        if (IsFinite)
        {
            if (_count > LargestPowerOfTwoExponent)
            {
                throw new InvalidOperationException(PowerTooLargeMessage);
            }

            return Finite(1L << (int)_count);
        }

        return Beth(_bethPlusOne);
    }

    /// <summary>Compares two sizes: every finite cardinal is below every infinite one.</summary>
    /// <param name="other">The size to compare with.</param>
    /// <returns>Negative, zero or positive as this size is smaller, equal or larger.</returns>
    public int CompareTo(Cardinal other)
    {
        if (_bethPlusOne != other._bethPlusOne)
        {
            return _bethPlusOne.CompareTo(other._bethPlusOne);
        }

        return _count.CompareTo(other._count);
    }

    // A record prints every public property, and one of Count and BethIndex throws, so the cardinal
    // prints the one that holds.
    /// <inheritdoc/>
    public override string ToString()
    {
        if (IsFinite)
        {
            return string.Format(CultureInfo.InvariantCulture, FiniteFormat, _count);
        }

        return string.Format(CultureInfo.InvariantCulture, BethFormat, _bethPlusOne - 1);
    }

    private static Cardinal Max(Cardinal first, Cardinal second)
    {
        var isFirstLarger = first.CompareTo(second) >= 0;
        return isFirstLarger ? first : second;
    }
}
