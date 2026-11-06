using DSAExperimentation.Domain.Mathematics.Metrics;
using DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics;

// The L∞ norm is a maximum of sizes, so its examples are exact in whole numbers. No finite exponent
// gives it, so its link to the general form is a limit: in n dimensions, a vector's Lp length lies
// between its largest coordinate's size and n^(1/p) times that, which closes on the maximum as p
// grows.
public sealed partial class LInfinityNormTests
{
    private const double RoundingAllowance = 1e-9;

    public static TheoryData<int, int, int> PlanarLengths => new()
    {
        { 0, 0, 0 },
        { 3, -4, 4 },
        { -3, 4, 4 },
        { -5, -5, 5 },
        { 9, 0, 9 },
    };

    public static TheoryData<double> GrowingExponents => [1.0, 2.0, 8.0, 64.0];

    [Fact]
    public void Exponent_IsPositiveInfinityDirectlyAndThroughTheNorm()
    {
        Assert.True(double.IsPositiveInfinity(LInfinityNorm.Exponent));
        Assert.True(double.IsPositiveInfinity(NormFacts.ExponentOf<LInfinityNorm>()));
    }

    [Theory]
    [MemberData(nameof(PlanarLengths))]
    public void Length_PlanarVector_IsTheLargerCoordinatesSize(int first, int second, int expected)
    {
        Assert.Equal(expected, LInfinityNorm.Length(first, second));
        Assert.Equal(expected, LInfinityNorm.Length<int>([first, second]));
        Assert.Equal((long)expected, LInfinityNorm.Length((long)first, (long)second));
    }

    [Fact]
    public void Length_EmptyVector_IsZero() =>
        Assert.Equal(0, LInfinityNorm.Length(ReadOnlySpan<int>.Empty));

    [Fact]
    public void Length_FiveDimensions_IsTheLargestSizeOnAnyAxis() =>
        Assert.Equal(7, LInfinityNorm.Length<int>([1, -2, -7, 4, 5]));

    [Theory]
    [MemberData(nameof(GrowingExponents))]
    public void Length_SampledVectors_BoundsTheGeneralNormFromBelowAndCloses(double exponent)
    {
        foreach (var (dimension, points) in SampledPoints.ByDimension)
        {
            var closing = Math.Pow(dimension, 1.0 / exponent);

            Assert.All(points, point =>
            {
                var largest = LInfinityNorm.Length<double>(point);

                Assert.InRange(
                    LpNorm.Length(point, exponent),
                    largest - RoundingAllowance,
                    (closing * largest) + RoundingAllowance);
            });
        }
    }

    [Fact]
    public void Distance_ThreeDimensions_IsTheMostThePointsDifferOnOneAxis() =>
        Assert.Equal(3, LInfinityNorm.Distance<int>([1, 2, 3], [4, 0, 3]));

    [Fact]
    public void Distance_PointsOfDifferentDimensions_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => LInfinityNorm.Distance<int>([1, 2], [1, 2, 3]));

    [Fact]
    public void Distance_SampledPoints_SatisfiesTheMetricAxioms() =>
        MetricAxioms.AssertHoldFor<LInfinityNorm>();
}
