using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfOperationsToMakeNetworkConnectedBenchmarks (ARCHITECTURE 17.9): both
// arms return the same minimum number of cable moves - a DFS flood fill over a freshly built
// adjacency list against the DisjointSet union-find - so a harness whose arms disagree is timing two
// different questions. The count (or -1 when there are too few cables) is the problem's whole answer,
// not a proxy, and the seeded spanning tree plus extra edges makes the -1 branch reachable. Setup
// builds the cable list, so the same ComputerCount must rebuild the same one.
public sealed partial class NumberOfOperationsToMakeNetworkConnectedBenchmarksTests
{
    private const int SmallestComputerCount = 50;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DepthFirstFloodFill(), BuildHarness().DepthFirstFloodFill());

    [Fact]
    public void DepthFirstFloodFill_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.DepthFirstFloodFill());
    }

    [Fact]
    public void DisjointSetUnionFind_AgreesWithDepthFirstFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstFloodFill(), harness.DisjointSetUnionFind());
    }

    private static NumberOfOperationsToMakeNetworkConnectedBenchmarks BuildHarness()
    {
        var harness = new NumberOfOperationsToMakeNetworkConnectedBenchmarks { ComputerCount = SmallestComputerCount };
        harness.Setup();

        return harness;
    }
}
