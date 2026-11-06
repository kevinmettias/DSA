using DSAExperimentation.Domain.Mathematics.Metrics;
using DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics;

// The squared length is a sum of products, exact in whole numbers, and the length its square root,
// so Pythagorean triples make exact examples of both. The squared form is offered for comparisons,
// so the tests check it orders vectors as their lengths do.
public sealed partial class L2NormTests
{
    private const double RoundingAllowance = 1e-9;

    public static TheoryData<int, int, int> PythagoreanTriples => new()
    {
        { 0, 0, 0 },
        { 3, -4, 5 },
        { -5, 12, 13 },
        { 8, 15, 17 },
        { 0, -7, 7 },
    };

    [Fact]
    public void Exponent_IsTwoDirectlyAndThroughTheNorm()
    {
        Assert.Equal(2.0, L2Norm.Exponent, RoundingAllowance);
        Assert.Equal(2.0, NormFacts.ExponentOf<L2Norm>(), RoundingAllowance);
    }

    [Theory]
    [MemberData(nameof(PythagoreanTriples))]
    public void LengthSquared_PlanarVector_IsTheSumOfTheCoordinatesSquares(
        int first, int second, int length)
    {
        Assert.Equal(length * length, L2Norm.LengthSquared(first, second));
        Assert.Equal(length * length, L2Norm.LengthSquared<int>([first, second]));
    }

    [Fact]
    public void LengthSquared_EmptyVector_IsZero() =>
        Assert.Equal(0, L2Norm.LengthSquared(ReadOnlySpan<int>.Empty));

    [Fact]
    public void LengthSquared_SampledVectors_KeepsTheOrderOfTheirLengths()
    {
        var vectors = SampledPoints.ByDimension.Single(sample => sample.Dimension == 3).Points;

        Assert.All(vectors, first => Assert.All(vectors, second => Assert.Equal(
            L2Norm.Length(first).CompareTo(L2Norm.Length(second)),
            L2Norm.LengthSquared<double>(first).CompareTo(L2Norm.LengthSquared<double>(second)))));
    }

    [Theory]
    [MemberData(nameof(PythagoreanTriples))]
    public void Length_PlanarVector_IsTheSquareRootOfTheSquaredLength(
        int first, int second, int length) =>
        Assert.Equal(length, L2Norm.Length([first, second]), RoundingAllowance);

    // The presets' order on R^n: L∞ <= L2 <= L1 <= n · L∞. Only L2's root rounds.
    [Fact]
    public void Length_SampledVectors_LiesBetweenTheOtherPresetsLengths()
    {
        foreach (var (dimension, points) in SampledPoints.ByDimension)
        {
            Assert.All(points, point =>
            {
                var largest = LInfinityNorm.Length<double>(point);
                var straight = L2Norm.Length(point);
                var summed = L1Norm.Length<double>(point);

                Assert.InRange(straight, largest - RoundingAllowance, summed + RoundingAllowance);
                Assert.InRange(summed, largest, dimension * largest);
            });
        }
    }

    [Fact]
    public void Length_SampledVectors_IsTheGeneralNormAtExponentTwo() =>
        NormFacts.AssertAgreesWithTheGeneralForm<L2Norm>();

    [Fact]
    public void DistanceSquared_ThreeDimensions_SumsTheSquaredDifferences() =>
        Assert.Equal(13, L2Norm.DistanceSquared<int>([1, 2, 3], [4, 0, 3]));

    [Fact]
    public void DistanceSquared_PointsOfDifferentDimensions_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => L2Norm.DistanceSquared<int>([1, 2], [1, 2, 3]));

    [Fact]
    public void Distance_ThreeDimensions_IsTheStraightLineDistance() =>
        Assert.Equal(7.0, L2Norm.Distance([1.0, 2.0, 3.0], [3.0, 5.0, 9.0]), RoundingAllowance);

    [Fact]
    public void Distance_SampledPoints_SatisfiesTheMetricAxioms() =>
        MetricAxioms.AssertHoldFor<L2Norm>();
}
