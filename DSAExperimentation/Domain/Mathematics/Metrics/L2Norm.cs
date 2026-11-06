using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.Metrics;

// The L2 norm, p = 2: a vector's length is the square root of the sum of its coordinates' squares,
// Pythagoras's theorem in n dimensions, so the distance between two points is the straight-line
// distance of Euclidean space. No power is taken: a square is a product, the root a square root.
//
// The squared length is a sum of products, exact in any number type, so those members are generic
// over it, and comparing two lengths needs no root, since the square root keeps their order. Only
// the contract's members, which return a length, take the root, in doubles.
//
// Euclidean distance belongs to R^n, not to a grid: no set of lattice steps counts it, because a
// count of steps is a whole number and a cell's diagonal neighbour is the square root of 2 away.
/// <summary>The L2 (Euclidean) norm: the square root of the coordinates' squares summed.</summary>
public readonly struct L2Norm : ILpNorm
{
    /// <inheritdoc/>
    public static double Exponent => 2.0;

    /// <summary>The squared length of a planar vector: its coordinates' squares summed.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="firstCoordinate">The vector's first coordinate.</param>
    /// <param name="secondCoordinate">The vector's second coordinate.</param>
    /// <returns>The sum of the two coordinates' squares.</returns>
    public static TNumber LengthSquared<TNumber>(TNumber firstCoordinate, TNumber secondCoordinate)
        where TNumber : INumber<TNumber>
        => (firstCoordinate * firstCoordinate) + (secondCoordinate * secondCoordinate);

    /// <summary>The squared length of a vector: the sum of its coordinates' squares.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="vector">The vector's coordinates.</param>
    /// <returns>The sum of the coordinates' squares; zero for no coordinates.</returns>
    public static TNumber LengthSquared<TNumber>(ReadOnlySpan<TNumber> vector)
        where TNumber : INumber<TNumber>
    {
        var sum = TNumber.Zero;

        foreach (var coordinate in vector)
        {
            sum += coordinate * coordinate;
        }

        return sum;
    }

    /// <summary>The squared distance: the two points' differences squared and summed.</summary>
    /// <typeparam name="TNumber">The number type of the coordinates.</typeparam>
    /// <param name="from">The first point's coordinates.</param>
    /// <param name="to">The second point's coordinates, as many as the first's.</param>
    /// <returns>The sum of the squares of the coordinates' differences.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The points differ in dimension.</exception>
    public static TNumber DistanceSquared<TNumber>(
        ReadOnlySpan<TNumber> from, ReadOnlySpan<TNumber> to)
        where TNumber : INumber<TNumber>
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(to.Length, from.Length, nameof(to));
        var sum = TNumber.Zero;

        for (var axis = 0; axis < from.Length; axis++)
        {
            var difference = to[axis] - from[axis];
            sum += difference * difference;
        }

        return sum;
    }

    /// <inheritdoc/>
    public static double Length(ReadOnlySpan<double> vector)
    {
        var squared = LengthSquared(vector);

        return Math.Sqrt(squared);
    }

    /// <inheritdoc/>
    public static double Distance(ReadOnlySpan<double> from, ReadOnlySpan<double> to)
    {
        var squared = DistanceSquared(from, to);

        return Math.Sqrt(squared);
    }
}
