namespace DSAExperimentation.Domain.Mathematics.Metrics;

// The Lp norm for any exponent p of at least 1, by powers: (|x1|^p + ... + |xn|^p)^(1/p). This is
// the general form, which serves an exponent no preset fixes, and the one the tests hold each
// preset to. The presets are this norm at their exponents with code of their own, so a caller that
// knows its exponent reads the preset and pays for no power.
//
// Positive infinity is the family's limit, the largest coordinate's size, which no power reaches,
// so for it the general form reads L∞'s code. An exponent below 1 is refused: there the triangle
// inequality fails, so the formula gives no norm.
/// <summary>The Lp norm on real coordinate space for any exponent of at least 1.</summary>
public static class LpNorm
{
    // The smallest exponent for which the formula is a norm.
    private const double SmallestExponent = 1.0;

    private const string ExponentBelowOneMessage = "An Lp norm needs an exponent of at least 1.";

    /// <summary>The length of a vector under the Lp norm of the given exponent.</summary>
    /// <param name="vector">The vector's coordinates.</param>
    /// <param name="exponent">The exponent p: at least 1, or positive infinity.</param>
    /// <returns>The p-th root of the sum of the coordinates' sizes to the p-th power.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="exponent"/> is below 1 or not a number.
    /// </exception>
    public static double Length(ReadOnlySpan<double> vector, double exponent)
    {
        RequireNormExponent(exponent);

        if (double.IsPositiveInfinity(exponent))
        {
            return LInfinityNorm.Length(vector);
        }

        var sum = 0.0;

        foreach (var coordinate in vector)
        {
            sum += Math.Pow(Math.Abs(coordinate), exponent);
        }

        return Math.Pow(sum, 1.0 / exponent);
    }

    /// <summary>The distance between two points under the Lp norm of the given exponent.</summary>
    /// <param name="from">The first point's coordinates.</param>
    /// <param name="to">The second point's coordinates, as many as the first's.</param>
    /// <param name="exponent">The exponent p: at least 1, or positive infinity.</param>
    /// <returns>The length of the difference between the two points.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="exponent"/> is below 1 or not a number, or the points differ in dimension.
    /// </exception>
    public static double Distance(
        ReadOnlySpan<double> from, ReadOnlySpan<double> to, double exponent)
    {
        RequireNormExponent(exponent);

        if (double.IsPositiveInfinity(exponent))
        {
            return LInfinityNorm.Distance(from, to);
        }

        ArgumentOutOfRangeException.ThrowIfNotEqual(to.Length, from.Length, nameof(to));
        var sum = 0.0;

        for (var axis = 0; axis < from.Length; axis++)
        {
            sum += Math.Pow(Math.Abs(to[axis] - from[axis]), exponent);
        }

        return Math.Pow(sum, 1.0 / exponent);
    }

    // NaN compares false with everything, so it fails this test as an exponent below 1 does.
    private static void RequireNormExponent(double exponent)
    {
        var isNormExponent = exponent >= SmallestExponent;

        if (!isNormExponent)
        {
            throw new ArgumentOutOfRangeException(
                nameof(exponent), exponent, ExponentBelowOneMessage);
        }
    }
}
