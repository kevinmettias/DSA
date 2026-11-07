using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.ExtendedReals;

// The projectively extended reals: the real line with one point added, ∞, which both ends meet at,
// so the line closes into a circle. ∞ is its own negative and has no sign, so there is no order,
// but division by zero becomes defined: a nonzero number over 0 is ∞, and a finite number over ∞ is
// 0. Undefined, as in the affine extension (AffinelyExtendedReal), are 0 times ∞ and ∞ over ∞,
// and here also 0 over 0, ∞ + ∞ and ∞ - ∞, since the two ends' limits are one point but their sum
// could come from either.
//
// The finite numbers are any number type's, so the arithmetic is exact when the type's is. A
// non-finite value of the type, a double's own infinity or NaN, is refused.
/// <summary>A number of the projectively extended reals: finite, or the one unsigned ∞.</summary>
/// <typeparam name="TNumber">The number type of the finite values.</typeparam>
public readonly record struct ProjectivelyExtendedReal<TNumber>
    where TNumber : INumber<TNumber>
{
    private const string NotFiniteMessage =
        "A projectively extended real's finite value must be finite.";

    private readonly TNumber _finite;

    // Whether the value is ∞ rather than finite.
    private readonly bool _isInfinity;

    private bool IsZero => IsFinite && TNumber.IsZero(_finite);

    /// <summary>Gets ∞, the one point both ends of the line meet at.</summary>
    public static ProjectivelyExtendedReal<TNumber> Infinity { get; } = new(true);

    /// <summary>Gets a value indicating whether the value is finite.</summary>
    public bool IsFinite => !_isInfinity;

    /// <summary>A finite value.</summary>
    /// <param name="finite">The value, finite in its own type.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="finite"/> is infinite or NaN.
    /// </exception>
    public ProjectivelyExtendedReal(TNumber finite)
    {
        if (!TNumber.IsFinite(finite))
        {
            throw new ArgumentOutOfRangeException(
                nameof(finite), finite, NotFiniteMessage);
        }

        _finite = finite;
        _isInfinity = false;
    }

    private ProjectivelyExtendedReal(bool isInfinity)
    {
        _finite = TNumber.Zero;
        _isInfinity = isInfinity;
    }

    /// <summary>Reads the finite value.</summary>
    /// <param name="finite">The value, when finite; zero otherwise.</param>
    /// <returns>True when the value is finite.</returns>
    public bool TryGetFinite(out TNumber finite)
    {
        finite = _finite;
        return IsFinite;
    }

    /// <summary>The negative: ∞ is its own.</summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>The value's negative.</returns>
    public static ProjectivelyExtendedReal<TNumber> Negate(ProjectivelyExtendedReal<TNumber> value)
    {
        if (value.IsFinite)
        {
            return new(-value._finite);
        }

        return Infinity;
    }

    /// <summary>Adds two values; ∞ + ∞ is undefined.</summary>
    /// <param name="augend">The first value.</param>
    /// <param name="addend">The second value.</param>
    /// <param name="sum">The sum, when defined.</param>
    /// <returns>False when both are ∞; true otherwise.</returns>
    public static bool TryAdd(
        ProjectivelyExtendedReal<TNumber> augend,
        ProjectivelyExtendedReal<TNumber> addend,
        out ProjectivelyExtendedReal<TNumber> sum)
    {
        sum = default;

        if (augend._isInfinity && addend._isInfinity)
        {
            return false;
        }

        sum = Infinity;

        if (augend.IsFinite && addend.IsFinite)
        {
            sum = new(augend._finite + addend._finite);
        }

        return true;
    }

    /// <summary>Subtracts one value from another; ∞ - ∞ is undefined.</summary>
    /// <param name="minuend">The value subtracted from.</param>
    /// <param name="subtrahend">The value subtracted.</param>
    /// <param name="difference">The difference, when defined.</param>
    /// <returns>False when both are ∞; true otherwise.</returns>
    public static bool TrySubtract(
        ProjectivelyExtendedReal<TNumber> minuend,
        ProjectivelyExtendedReal<TNumber> subtrahend,
        out ProjectivelyExtendedReal<TNumber> difference)
        => TryAdd(minuend, Negate(subtrahend), out difference);

    /// <summary>Multiplies two values; 0 times ∞ is undefined.</summary>
    /// <param name="multiplicand">The first value.</param>
    /// <param name="multiplier">The second value.</param>
    /// <param name="product">The product, when defined.</param>
    /// <returns>False when one is zero and the other ∞; true otherwise.</returns>
    public static bool TryMultiply(
        ProjectivelyExtendedReal<TNumber> multiplicand,
        ProjectivelyExtendedReal<TNumber> multiplier,
        out ProjectivelyExtendedReal<TNumber> product)
    {
        product = default;

        if (multiplicand.IsFinite && multiplier.IsFinite)
        {
            product = new(multiplicand._finite * multiplier._finite);
            return true;
        }

        if (multiplicand.IsZero || multiplier.IsZero)
        {
            return false;
        }

        product = Infinity;
        return true;
    }

    /// <summary>
    /// Divides one value by another; a nonzero value over 0 is ∞, and 0 over 0 and ∞ over ∞
    /// undefined.
    /// </summary>
    /// <param name="dividend">The value divided.</param>
    /// <param name="divisor">The value divided by.</param>
    /// <param name="quotient">The quotient, when defined.</param>
    /// <returns>False for 0 over 0 and ∞ over ∞; true otherwise.</returns>
    public static bool TryDivide(
        ProjectivelyExtendedReal<TNumber> dividend,
        ProjectivelyExtendedReal<TNumber> divisor,
        out ProjectivelyExtendedReal<TNumber> quotient)
    {
        quotient = default;

        if (IsIndeterminateQuotient(dividend, divisor))
        {
            return false;
        }

        quotient = QuotientOfDefined(dividend, divisor);
        return true;
    }

    // 0 over 0 and ∞ over ∞, the two quotients whose limits could be anything.
    private static bool IsIndeterminateQuotient(
        ProjectivelyExtendedReal<TNumber> dividend, ProjectivelyExtendedReal<TNumber> divisor)
    {
        if (dividend.IsZero)
        {
            return divisor.IsZero;
        }

        return dividend._isInfinity && divisor._isInfinity;
    }

    // A quotient the caller has ruled defined: over 0 or from ∞ it is ∞, over ∞ it is 0.
    private static ProjectivelyExtendedReal<TNumber> QuotientOfDefined(
        ProjectivelyExtendedReal<TNumber> dividend, ProjectivelyExtendedReal<TNumber> divisor)
    {
        if (divisor.IsZero || dividend._isInfinity)
        {
            return Infinity;
        }

        if (divisor._isInfinity)
        {
            return new(TNumber.Zero);
        }

        return new(dividend._finite / divisor._finite);
    }
}
