using DSAExperimentation.Domain.Mathematics.Metrics;
using DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics;

// The general form is held to examples at the presets' exponents and between them, to the metric
// axioms at an exponent no preset fixes (the test-only CubicNorm), and to the family's own order: a
// vector's length never grows as the exponent does. Below 1 the triangle inequality fails, so such
// an exponent, and NaN, are refused.
public sealed partial class LpNormTests
{
    private const double RoundingAllowance = 1e-9;

    private static readonly double[] AscendingExponents =
        [1.0, 1.5, 2.0, 3.0, 8.0, double.PositiveInfinity];

    public static TheoryData<double> RefusedExponents =>
        [0.999, 0.5, 0.0, -1.0, double.NaN, double.NegativeInfinity];

    // The vector (3, -4) at each exponent: 7, 5, the cube root of 27 + 64, and 4.
    public static TheoryData<double, double> LengthsOfThreeMinusFour => new()
    {
        { 1.0, 7.0 },
        { 2.0, 5.0 },
        { 3.0, Math.Cbrt(91.0) },
        { double.PositiveInfinity, 4.0 },
    };

    [Theory]
    [MemberData(nameof(LengthsOfThreeMinusFour))]
    public void Length_ThreeMinusFour_IsTheRootOfTheSummedPowers(
        double exponent, double expected) =>
        Assert.Equal(expected, LpNorm.Length([3.0, -4.0], exponent), RoundingAllowance);

    [Theory]
    [MemberData(nameof(RefusedExponents))]
    public void Length_ExponentBelowOneOrNotANumber_ThrowsForTheDistanceToo(double exponent)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LpNorm.Length([3.0, -4.0], exponent));
        Assert.Throws<ArgumentOutOfRangeException>(() => LpNorm.Distance([1.0], [2.0], exponent));
    }

    [Fact]
    public void Length_SampledVectors_NeverGrowsAsTheExponentDoes()
    {
        foreach (var (_, points) in SampledPoints.ByDimension)
        {
            Assert.All(points, point => Assert.All(
                AscendingExponents.Zip(AscendingExponents.Skip(1)),
                pair => Assert.True(
                    LpNorm.Length(point, pair.Second)
                        <= LpNorm.Length(point, pair.First) + RoundingAllowance,
                    $"p = {pair.First}, {pair.Second}")));
        }
    }

    [Theory]
    [MemberData(nameof(LengthsOfThreeMinusFour))]
    public void Distance_PointsThreeMinusFourApart_IsThatVectorsLength(
        double exponent, double expected) =>
        Assert.Equal(
            expected, LpNorm.Distance([1.0, 6.0], [4.0, 2.0], exponent), RoundingAllowance);

    [Fact]
    public void Distance_PointsOfDifferentDimensions_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => LpNorm.Distance([1.0, 2.0], [1.0], 3.0));

    [Fact]
    public void Distance_ExponentThree_SatisfiesTheMetricAxioms() =>
        MetricAxioms.AssertHoldFor<CubicNorm>();
}
