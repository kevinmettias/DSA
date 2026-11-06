using DSAExperimentation.Domain.Mathematics.Metrics;
using DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics;

// The L1 norm is a sum of sizes, so its examples are exact in whole numbers, and its link to the
// general form is its exponent: the Lp norm at p = 1 is the same sum.
public sealed partial class L1NormTests
{
    private const double RoundingAllowance = 1e-9;

    public static TheoryData<int, int, int> PlanarLengths => new()
    {
        { 0, 0, 0 },
        { 3, -4, 7 },
        { -3, 4, 7 },
        { -5, -5, 10 },
        { 0, 9, 9 },
    };

    [Fact]
    public void Exponent_IsOneDirectlyAndThroughTheNorm()
    {
        Assert.Equal(1.0, L1Norm.Exponent, RoundingAllowance);
        Assert.Equal(1.0, NormFacts.ExponentOf<L1Norm>(), RoundingAllowance);
    }

    [Theory]
    [MemberData(nameof(PlanarLengths))]
    public void Length_PlanarVector_IsTheSumOfTheCoordinatesSizes(
        int first, int second, int expected)
    {
        Assert.Equal(expected, L1Norm.Length(first, second));
        Assert.Equal(expected, L1Norm.Length<int>([first, second]));
        Assert.Equal((long)expected, L1Norm.Length((long)first, (long)second));
    }

    [Fact]
    public void Length_EmptyVector_IsZero() =>
        Assert.Equal(0, L1Norm.Length(ReadOnlySpan<int>.Empty));

    [Fact]
    public void Length_FiveDimensions_AddsEveryAxis() =>
        Assert.Equal(15, L1Norm.Length<int>([1, -2, 3, -4, 5]));

    [Fact]
    public void Length_SampledVectors_IsTheGeneralNormAtExponentOne() =>
        NormFacts.AssertAgreesWithTheGeneralForm<L1Norm>();

    [Fact]
    public void Distance_ThreeDimensions_AddsHowFarApartThePointsAreOnEachAxis() =>
        Assert.Equal(5, L1Norm.Distance<int>([1, 2, 3], [4, 0, 3]));

    [Fact]
    public void Distance_PointsOfDifferentDimensions_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => L1Norm.Distance<int>([1, 2], [1, 2, 3]));

    [Fact]
    public void Distance_SampledPoints_SatisfiesTheMetricAxioms() =>
        MetricAxioms.AssertHoldFor<L1Norm>();
}
