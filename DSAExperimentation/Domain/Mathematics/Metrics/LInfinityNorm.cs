using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.Metrics;

// The L∞ norm, p = ∞: a vector's length is its largest coordinate's size, the limit of its Lp
// lengths as p grows, so the distance between two points is the most they differ on any one axis.
// On a grid it is the Chebyshev distance, the fewest steps from one cell to another when a step may
// also go diagonally (Lattices' GridDistances). No power is taken, nor could one be, since no
// finite exponent gives the limit: the maximum is the norm.
//
// The maximum is exact in any number type, so the members are generic over it, and the contract's
// members over doubles read them. The planar member is the two-coordinate case written out, with
// no span to build, for a caller holding a displacement's two coordinates, as GridDistances does.
/// <summary>The L∞ (Chebyshev) norm: a vector's length is its largest coordinate's size.</summary>
public readonly struct LInfinityNorm : ILpNorm
{
    /// <inheritdoc/>
    public static double Exponent => double.PositiveInfinity;

    /// <summary>The length of a planar vector: the larger of its two coordinates' sizes.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="firstCoordinate">The vector's first coordinate.</param>
    /// <param name="secondCoordinate">The vector's second coordinate.</param>
    /// <returns>The larger of the two coordinates' absolute values.</returns>
    public static TNumber Length<TNumber>(TNumber firstCoordinate, TNumber secondCoordinate)
        where TNumber : INumber<TNumber>
        => TNumber.Max(TNumber.Abs(firstCoordinate), TNumber.Abs(secondCoordinate));

    /// <summary>The length of a vector: its largest coordinate's size.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="vector">The vector's coordinates.</param>
    /// <returns>The largest of the coordinates' absolute values; zero for no coordinates.</returns>
    public static TNumber Length<TNumber>(ReadOnlySpan<TNumber> vector)
        where TNumber : INumber<TNumber>
    {
        var largest = TNumber.Zero;

        foreach (var coordinate in vector)
        {
            largest = TNumber.Max(largest, TNumber.Abs(coordinate));
        }

        return largest;
    }

    /// <summary>The distance between two points: the most they differ on any one axis.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="from">The first point's coordinates.</param>
    /// <param name="to">The second point's coordinates, as many as the first's.</param>
    /// <returns>The largest of the coordinates' absolute differences.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The points differ in dimension.</exception>
    public static TNumber Distance<TNumber>(ReadOnlySpan<TNumber> from, ReadOnlySpan<TNumber> to)
        where TNumber : INumber<TNumber>
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(to.Length, from.Length, nameof(to));
        var largest = TNumber.Zero;

        for (var axis = 0; axis < from.Length; axis++)
        {
            largest = TNumber.Max(largest, TNumber.Abs(to[axis] - from[axis]));
        }

        return largest;
    }

    static double ILpNorm.Length(ReadOnlySpan<double> vector) => Length(vector);

    static double ILpNorm.Distance(ReadOnlySpan<double> from, ReadOnlySpan<double> to) =>
        Distance(from, to);
}
