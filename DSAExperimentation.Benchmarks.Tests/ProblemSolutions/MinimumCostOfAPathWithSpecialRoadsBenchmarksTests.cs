using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostOfAPathWithSpecialRoadsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - Dijkstra over a dense point graph with a linear
// scan of the unsettled distance table against the same search behind this repo's heap - so a
// harness whose arms disagree is timing two different graphs. Setup draws the start, the target and
// the special roads from one seeded stream, so the same SpecialRoadCount must rebuild the same
// instance.
public sealed partial class MinimumCostOfAPathWithSpecialRoadsBenchmarksTests
{
    private const int SmallestSpecialRoadCount = 20;

    [Fact]
    public void Setup_SameSpecialRoadCount_RebuildsTheSameRoads() =>
        Assert.Equal(BuildHarness().ArrayDijkstra(), BuildHarness().ArrayDijkstra());

    [Fact]
    public void ArrayDijkstra_RandomSpecialRoads_AgreesWithRepoHeapDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepoHeapDijkstra(), harness.ArrayDijkstra());
    }

    [Fact]
    public void RepoHeapDijkstra_RandomSpecialRoads_AgreesWithArrayDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayDijkstra(), harness.RepoHeapDijkstra());
    }

    private static MinimumCostOfAPathWithSpecialRoadsBenchmarks BuildHarness()
    {
        var harness = new MinimumCostOfAPathWithSpecialRoadsBenchmarks { SpecialRoadCount = SmallestSpecialRoadCount };
        harness.Setup();

        return harness;
    }
}
