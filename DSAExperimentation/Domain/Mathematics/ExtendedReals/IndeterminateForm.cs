using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Mathematics.ExtendedReals;

// An indeterminate form: two functions tend to these limits, and the limit of their combination is
// not fixed by them. Both functions tending to 0 tells nothing of their quotient's limit: x / x
// tends to 1, x^2 / x to 0 and x / x^2 to ∞. The limits are points of the affinely extended reals.
//
// The seven indeterminate forms of calculus, each a named value. The four built from arithmetic,
// 0/0, ∞/∞, 0·∞ and ∞ - ∞, are the operations the affine extension leaves undefined. The three
// built from powers, 0^0, 1^∞ and ∞^0, are forms of limits only: a power of fixed numbers 0^0 is 1
// by the convention algebra and combinatorics use, and IEEE 754 gives all three the value 1.
/// <summary>
/// An indeterminate form: an operation on two limits whose result's limit they do not fix.
/// </summary>
/// <param name="Operation">How the two limits are combined.</param>
/// <param name="Left">The first function's limit.</param>
/// <param name="Right">The second function's limit.</param>
public readonly record struct IndeterminateForm(
    LimitOperation Operation, AffinelyExtendedReal<long> Left, AffinelyExtendedReal<long> Right)
{
    private static readonly AffinelyExtendedReal<long> Zero = new(0);
    private static readonly AffinelyExtendedReal<long> One = new(1);
    private static readonly AffinelyExtendedReal<long> Infinity =
        AffinelyExtendedReal<long>.PositiveInfinity;

    /// <summary>Gets 0/0.</summary>
    public static IndeterminateForm ZeroOverZero { get; } =
        new(LimitOperation.Quotient, Zero, Zero);

    /// <summary>Gets ∞/∞.</summary>
    public static IndeterminateForm InfinityOverInfinity { get; } =
        new(LimitOperation.Quotient, Infinity, Infinity);

    /// <summary>Gets 0·∞.</summary>
    public static IndeterminateForm ZeroTimesInfinity { get; } =
        new(LimitOperation.Product, Zero, Infinity);

    /// <summary>Gets ∞ - ∞.</summary>
    public static IndeterminateForm InfinityMinusInfinity { get; } =
        new(LimitOperation.Difference, Infinity, Infinity);

    /// <summary>Gets 0^0.</summary>
    public static IndeterminateForm ZeroToTheZero { get; } = new(LimitOperation.Power, Zero, Zero);

    /// <summary>Gets 1^∞.</summary>
    public static IndeterminateForm OneToTheInfinity { get; } =
        new(LimitOperation.Power, One, Infinity);

    /// <summary>Gets ∞^0.</summary>
    public static IndeterminateForm InfinityToTheZero { get; } =
        new(LimitOperation.Power, Infinity, Zero);

    /// <summary>Gets the seven forms, the four of arithmetic first.</summary>
    public static ImmutableArray<IndeterminateForm> All { get; } =
    [
        ZeroOverZero, InfinityOverInfinity, ZeroTimesInfinity, InfinityMinusInfinity,
        ZeroToTheZero, OneToTheInfinity, InfinityToTheZero,
    ];
}
