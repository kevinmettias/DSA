using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableBenchmarks (ARCHITECTURE
// 17.9): both arms are that solution's, competing strategies for the same question - a breadth-first
// reachability check re-run per candidate edge against one DisjointSet union-find pass - so a harness
// whose arms disagree removes two different numbers of edges. Setup draws the spanning tree and the
// extra single-owner edges from one seeded Random, so the same NodeCount must rebuild the same graph.
public sealed partial class RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BreadthFirstReachabilityCheck(),
            BuildHarness().BreadthFirstReachabilityCheck());

    [Fact]
    public void BreadthFirstReachabilityCheck_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.BreadthFirstReachabilityCheck());
    }

    [Fact]
    public void DisjointSetUnionFind_AgreesWithBreadthFirstReachabilityCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BreadthFirstReachabilityCheck(), harness.DisjointSetUnionFind());
    }

    private static RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableBenchmarks BuildHarness()
    {
        var harness = new RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableBenchmarks
        {
            NodeCount = SmallestNodeCount,
        };
        harness.Setup();

        return harness;
    }
}
