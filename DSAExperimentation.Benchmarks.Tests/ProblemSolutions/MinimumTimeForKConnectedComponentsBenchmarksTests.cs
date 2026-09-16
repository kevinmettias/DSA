using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeForKConnectedComponentsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the earliest time at which the graph still has the
// required number of connected components - so a harness whose arms disagree is timing two different
// problems. The binary-search arm walks the edge times upward while the descending arm walks them
// downward, so agreement pins that the predicate's monotonic boundary is the same from both
// directions. Both arms read the one edge list [GlobalSetup] built from a fixed seed, so the same
// NodeCount must rebuild the same edges and the same required component count.
public sealed partial class MinimumTimeForKConnectedComponentsBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DescendingUnionFind(), BuildHarness().DescendingUnionFind());

    [Fact]
    public void BinarySearchUnionFind_SameEdgeList_AgreesWithDescendingUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DescendingUnionFind(), harness.BinarySearchUnionFind());
    }

    [Fact]
    public void DescendingUnionFind_SameEdgeList_AgreesWithBinarySearchUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchUnionFind(), harness.DescendingUnionFind());
    }

    private static MinimumTimeForKConnectedComponentsBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeForKConnectedComponentsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
