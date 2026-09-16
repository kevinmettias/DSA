using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for TriangleSideWorkloads (ARCHITECTURE 17.7): LC 3899 has no size axis - the input is
// always exactly three sides - so one valid triangle is the whole workload. "Valid" is the fixture's own
// rejection-sampling promise, so the strict triangle inequality is the property asserted here; an
// out-of-contract triple would have both strategies answer a question the problem never asks.
public sealed partial class TriangleSideWorkloadsTests
{
    private const int Seed = 3899; // LC problem number
    private const int SideCount = 3;
    private const int SmallestSide = 1;
    private const int LargestSide = 1_000;
    private const int ShortestPairCount = 2;

    [Fact]
    public void BuildValidTriangle_SeededTriple_ReturnsThreeSidesInsideTheDocumentedBound()
    {
        var sides = TriangleSideWorkloads.BuildValidTriangle(Seed);

        Assert.Equal(SideCount, sides.Length);
        Assert.All(sides, side => Assert.InRange(side, SmallestSide, LargestSide));
    }

    // The two shorter sides must sum above the longest one, or the triple is not a triangle and LC 3899's
    // angle formulas answer nothing.
    [Fact]
    public void BuildValidTriangle_SeededTriple_SatisfiesTheStrictTriangleInequality()
    {
        var ascending = TriangleSideWorkloads.BuildValidTriangle(Seed).Order().ToList();
        var twoShorterSides = ascending.Take(ShortestPairCount).Sum();
        var longestSide = ascending[^1];

        Assert.True(twoShorterSides > longestSide);
    }

    [Fact]
    public void BuildValidTriangle_SameSeed_ReturnsTheSameTriple() =>
        Assert.Equal(
            TriangleSideWorkloads.BuildValidTriangle(Seed),
            TriangleSideWorkloads.BuildValidTriangle(Seed));
}
