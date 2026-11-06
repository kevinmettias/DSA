using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.Metrics;

// The L1 norm, p = 1: a vector's length is the sum of its coordinates' sizes, so the distance
// between two points is how far apart they are along each axis, added up. On a grid it is the
// Manhattan or taxicab distance, the fewest orthogonal steps from one cell to another (Lattices'
// GridDistances). No power is taken: the sum is the norm.
//
// The sum is exact in any number type, so the members are generic over it and serve a grid's whole
// numbers as well as the reals; the contract's members over doubles read them. The planar member is
// the two-coordinate case written out, with no span to build, for a caller that holds a
// displacement's two coordinates, as GridDistances does.
/// <summary>The L1 (Manhattan) norm: a vector's length is its coordinates' sizes summed.</summary>
public readonly struct L1Norm : ILpNorm
{
    /// <inheritdoc/>
    public static double Exponent => 1.0;

    /// <summary>The length of a vector on the plane: its two coordinates' sizes summed.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="firstCoordinate">The vector's first coordinate.</param>
    /// <param name="secondCoordinate">The vector's second coordinate.</param>
    /// <returns>The sum of the two coordinates' absolute values.</returns>
    public static TNumber Length<TNumber>(TNumber firstCoordinate, TNumber secondCoordinate)
        where TNumber : INumber<TNumber>
        => TNumber.Abs(firstCoordinate) + TNumber.Abs(secondCoordinate);

    /// <summary>The length of a vector: the sum of its coordinates' sizes.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="vector">The vector's coordinates.</param>
    /// <returns>The sum of the coordinates' absolute values; zero for no coordinates.</returns>
    public static TNumber Length<TNumber>(ReadOnlySpan<TNumber> vector)
        where TNumber : INumber<TNumber>
    {
        var sum = TNumber.Zero;

        foreach (var coordinate in vector)
        {
            sum += TNumber.Abs(coordinate);
        }

        return sum;
    }

    /// <summary>The distance between two points: how far apart they are on each axis.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="from">The first point's coordinates.</param>
    /// <param name="to">The second point's coordinates, as many as the first's.</param>
    /// <returns>The sum of the coordinates' absolute differences.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The points differ in dimension.</exception>
    public static TNumber Distance<TNumber>(ReadOnlySpan<TNumber> from, ReadOnlySpan<TNumber> to)
        where TNumber : INumber<TNumber>
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(to.Length, from.Length, nameof(to));
        var sum = TNumber.Zero;

        for (var axis = 0; axis < from.Length; axis++)
        {
            sum += TNumber.Abs(to[axis] - from[axis]);
        }

        return sum;
    }

    static double ILpNorm.Length(ReadOnlySpan<double> vector) => Length(vector);

    static double ILpNorm.Distance(ReadOnlySpan<double> from, ReadOnlySpan<double> to) =>
        Distance(from, to);
}
