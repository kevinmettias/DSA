using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumPointsActivatedWithOneAdditionBenchmarks (ARCHITECTURE 17.9): both
// arms are MaximumPointsActivatedWithOneAdditionSolution's competing strategies for one question -
// union-find rebuilt from scratch per candidate coordinate against the keyed disjoint set over the
// same x/y rows and columns - so a harness whose arms disagree is timing two different problems.
// Both answer with a single activated-point count, compared directly.
public sealed partial class MaximumPointsActivatedWithOneAdditionBenchmarksTests
{
    private const int SmallestPointCount = 200;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSameDistinctPointSet()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The point set is private, so the rebuild is pinned through the count it produces: the
        // same PointCount must draw the same seeded distinct coordinates and merge them
        // identically.
        Assert.Equal(first.BruteForceUnionFind(), second.BruteForceUnionFind());
        Assert.Equal(first.KeyedDisjointSet(), second.KeyedDisjointSet());
    }

    [Fact]
    public void BruteForceUnionFind_SeededDistinctPoints_AgreesWithKeyedDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KeyedDisjointSet(), harness.BruteForceUnionFind());
    }

    [Fact]
    public void KeyedDisjointSet_SeededDistinctPoints_AgreesWithBruteForceUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceUnionFind(), harness.KeyedDisjointSet());
    }

    private static MaximumPointsActivatedWithOneAdditionBenchmarks BuildHarness()
    {
        var harness = new MaximumPointsActivatedWithOneAdditionBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
