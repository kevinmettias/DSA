using DSAExperimentation.Domain.Mathematics.Metrics;

namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

// What links a preset to the general form, read through the contract: its exponent as a generic
// caller sees it, and its lengths and distances against LpNorm's at that exponent on every sampled
// point. The general form takes powers, which round, so the two agree to a rounding error.
internal static class NormFacts
{
    private const double RoundingAllowance = 1e-9;

    public static double ExponentOf<TNorm>()
        where TNorm : struct, ILpNorm
        => TNorm.Exponent;

    public static void AssertAgreesWithTheGeneralForm<TNorm>()
        where TNorm : struct, ILpNorm
    {
        foreach (var (_, points) in SampledPoints.ByDimension)
        {
            foreach (var first in points)
            {
                Assert.Equal(
                    LpNorm.Length(first, TNorm.Exponent), TNorm.Length(first), RoundingAllowance);
                Assert.All(points, second => Assert.Equal(
                    LpNorm.Distance(first, second, TNorm.Exponent),
                    TNorm.Distance(first, second),
                    RoundingAllowance));
            }
        }
    }
}
