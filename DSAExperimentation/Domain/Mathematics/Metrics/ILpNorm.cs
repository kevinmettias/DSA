namespace DSAExperimentation.Domain.Mathematics.Metrics;

// A norm of the Lp family on R^n, the space of n real coordinates. A vector's length is
// (|x1|^p + ... + |xn|^p)^(1/p), the p-th root of the sum of its coordinates' sizes raised to the
// p-th power, and the distance between two points is the length of their difference. For every p of
// at least 1 that distance is a metric: zero only from a point to itself, the same both ways, and
// never longer than a detour through a third point, the triangle inequality, which for these norms
// is Minkowski's inequality. Below 1 the triangle inequality fails, so the family starts there. As
// p grows the length tends to the largest coordinate's size, which is the L∞ norm, p = ∞.
//
// The exponent is each preset's one fact, and each preset computes with code of its own: L1 and L∞
// take no power, and L2 is the square root of a sum of squares. LpNorm is the general form, which
// reaches any p with powers, and the tests hold each preset to it at the preset's exponent. A norm
// of another fixed p is a struct of the caller's that implements this contract and reads LpNorm.
//
// The members are static abstract members of a struct, so a caller generic over the norm compiles
// the preset's own arithmetic, and an exponent folds to its number. A preset states its exponent
// as a property rather than a public constant, because a public double constant would be read as
// the one source of every solution's own 1.0 or 2.0, whatever it means (SharedValueCopyTests).
/// <summary>A norm of the Lp family on real coordinate space.</summary>
public interface ILpNorm
{
    /// <summary>Gets the exponent p: at least 1, or positive infinity.</summary>
    static abstract double Exponent { get; }

    /// <summary>The length of a vector under this norm.</summary>
    /// <param name="vector">The vector's coordinates.</param>
    /// <returns>The vector's length.</returns>
    static abstract double Length(ReadOnlySpan<double> vector);

    /// <summary>The distance between two points: the length of their difference.</summary>
    /// <param name="from">The first point's coordinates.</param>
    /// <param name="to">The second point's coordinates, as many as the first's.</param>
    /// <returns>The distance between the two points.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The points differ in dimension.</exception>
    static abstract double Distance(ReadOnlySpan<double> from, ReadOnlySpan<double> to);
}
