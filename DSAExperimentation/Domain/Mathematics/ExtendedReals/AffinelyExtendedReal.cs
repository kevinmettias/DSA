using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.ExtendedReals;

// The affinely extended reals: the real line with two ends added, -∞ below every number and +∞
// above, so every set has a least upper bound and an increasing sequence a limit. The order
// extends, and so does the arithmetic wherever the limits agree: a finite number plus +∞ is +∞, a
// nonzero number times ±∞ takes the sign rule, a finite number over ±∞ is 0. Where they do not
// agree, the operation is left undefined, and the Try methods answer false: +∞ + -∞, 0 times ±∞, ±∞
// over ±∞, and anything over 0, whose limit is +∞ or -∞ by the side 0 is approached from. Measure
// theory's 0 times ∞ = 0 is a convention for integrals, not this arithmetic. IEEE 754 floating
// point (Computing/FloatingPoint) adds the same two infinities, but answers 1/0 with +∞ through its
// signed zero, so it is a neighbour of this system, not an implementation of it.
//
// The finite numbers are any number type's, so the arithmetic is exact when the type's is, as a
// rational type's or the integers' is when the result is an integer. A non-finite value of the
// type, a double's own infinity or NaN, is refused: the ends are this type's.
/// <summary>A number of the affinely extended reals: finite, +∞ or -∞.</summary>
/// <typeparam name="TNumber">The number type of the finite values.</typeparam>
public readonly record struct AffinelyExtendedReal<TNumber>
    : IComparable<AffinelyExtendedReal<TNumber>>
    where TNumber : INumber<TNumber>
{
    private const string NotFiniteMessage =
        "An affinely extended real's finite value must be finite.";

    private readonly TNumber _finite;

    // -1 for -∞, +1 for +∞, 0 for a finite value.
    private readonly int _end;

    // -1, 0 or +1: the sign of a finite value, or of the end.
    private int Sign => IsFinite ? TNumber.Sign(_finite) : _end;

    /// <summary>Gets +∞, above every finite value.</summary>
    public static AffinelyExtendedReal<TNumber> PositiveInfinity { get; } = new(1);

    /// <summary>Gets -∞, below every finite value.</summary>
    public static AffinelyExtendedReal<TNumber> NegativeInfinity { get; } = new(-1);

    /// <summary>Gets a value indicating whether the value is finite.</summary>
    public bool IsFinite => _end == 0;

    /// <summary>A finite value.</summary>
    /// <param name="finite">The value, finite in its own type.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="finite"/> is infinite or NaN.
    /// </exception>
    public AffinelyExtendedReal(TNumber finite)
    {
        if (!TNumber.IsFinite(finite))
        {
            throw new ArgumentOutOfRangeException(
                nameof(finite), finite, NotFiniteMessage);
        }

        _finite = finite;
        _end = 0;
    }

    private AffinelyExtendedReal(int end)
    {
        _finite = TNumber.Zero;
        _end = end;
    }

    /// <summary>Reads the finite value.</summary>
    /// <param name="finite">The value, when finite; zero otherwise.</param>
    /// <returns>True when the value is finite.</returns>
    public bool TryGetFinite(out TNumber finite)
    {
        finite = _finite;
        return IsFinite;
    }

    /// <summary>The negative: -(+∞) is -∞ and -(-∞) is +∞.</summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>The value's negative.</returns>
    public static AffinelyExtendedReal<TNumber> Negate(AffinelyExtendedReal<TNumber> value)
    {
        if (value.IsFinite)
        {
            return new(-value._finite);
        }

        return new(-value._end);
    }

    /// <summary>Adds two values; +∞ + -∞ is undefined.</summary>
    /// <param name="augend">The first value.</param>
    /// <param name="addend">The second value.</param>
    /// <param name="sum">The sum, when defined.</param>
    /// <returns>False for +∞ + -∞ and -∞ + +∞; true otherwise.</returns>
    public static bool TryAdd(
        AffinelyExtendedReal<TNumber> augend,
        AffinelyExtendedReal<TNumber> addend,
        out AffinelyExtendedReal<TNumber> sum)
    {
        sum = default;

        if (augend.IsFinite && addend.IsFinite)
        {
            sum = new(augend._finite + addend._finite);
            return true;
        }

        if (augend._end + addend._end == 0)
        {
            return false;
        }

        sum = new(Math.Sign(augend._end + addend._end));
        return true;
    }

    /// <summary>Subtracts one value from another; ∞ - ∞ of either sign is undefined.</summary>
    /// <param name="minuend">The value subtracted from.</param>
    /// <param name="subtrahend">The value subtracted.</param>
    /// <param name="difference">The difference, when defined.</param>
    /// <returns>False when both are the same infinity; true otherwise.</returns>
    public static bool TrySubtract(
        AffinelyExtendedReal<TNumber> minuend,
        AffinelyExtendedReal<TNumber> subtrahend,
        out AffinelyExtendedReal<TNumber> difference)
        => TryAdd(minuend, Negate(subtrahend), out difference);

    /// <summary>Multiplies two values; 0 times ±∞ is undefined.</summary>
    /// <param name="multiplicand">The first value.</param>
    /// <param name="multiplier">The second value.</param>
    /// <param name="product">The product, when defined.</param>
    /// <returns>False when one is zero and the other infinite; true otherwise.</returns>
    public static bool TryMultiply(
        AffinelyExtendedReal<TNumber> multiplicand,
        AffinelyExtendedReal<TNumber> multiplier,
        out AffinelyExtendedReal<TNumber> product)
    {
        product = default;

        if (multiplicand.IsFinite && multiplier.IsFinite)
        {
            product = new(multiplicand._finite * multiplier._finite);
            return true;
        }

        var sign = multiplicand.Sign * multiplier.Sign;

        if (sign == 0)
        {
            return false;
        }

        product = new(sign);
        return true;
    }

    /// <summary>
    /// Divides one value by another; anything over 0, and ±∞ over ±∞, is undefined.
    /// </summary>
    /// <param name="dividend">The value divided.</param>
    /// <param name="divisor">The value divided by.</param>
    /// <param name="quotient">The quotient, when defined.</param>
    /// <returns>False when the divisor is zero or both are infinite; true otherwise.</returns>
    public static bool TryDivide(
        AffinelyExtendedReal<TNumber> dividend,
        AffinelyExtendedReal<TNumber> divisor,
        out AffinelyExtendedReal<TNumber> quotient)
    {
        quotient = default;

        if (IsIndeterminateQuotient(dividend, divisor))
        {
            return false;
        }

        if (!divisor.IsFinite)
        {
            quotient = new(TNumber.Zero);
            return true;
        }

        if (dividend.IsFinite)
        {
            quotient = new(dividend._finite / divisor._finite);
            return true;
        }

        quotient = new(dividend._end * divisor.Sign);
        return true;
    }

    // Anything over 0, and ±∞ over ±∞: the quotients whose limits the operands do not fix.
    private static bool IsIndeterminateQuotient(
        AffinelyExtendedReal<TNumber> dividend, AffinelyExtendedReal<TNumber> divisor)
    {
        if (divisor.Sign == 0)
        {
            return true;
        }

        return !dividend.IsFinite && !divisor.IsFinite;
    }

    /// <summary>Compares two values: -∞ is below every finite value and +∞ above.</summary>
    /// <param name="other">The value to compare with.</param>
    /// <returns>Negative, zero or positive as this value is smaller, equal or larger.</returns>
    public int CompareTo(AffinelyExtendedReal<TNumber> other)
    {
        if (_end != other._end)
        {
            return _end.CompareTo(other._end);
        }

        return _finite.CompareTo(other._finite);
    }
}
