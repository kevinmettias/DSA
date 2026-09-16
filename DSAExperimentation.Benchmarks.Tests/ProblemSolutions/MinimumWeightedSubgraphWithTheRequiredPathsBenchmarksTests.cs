using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumWeightedSubgraphWithTheRequiredPathsBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - three fresh point-to-point searches per
// candidate meeting vertex against the reverse-graph Dijkstra over the RequiredPathsGraph built in
// [GlobalSetup] - so a harness whose arms disagree is timing two different problems. Both arms only read
// the prepared graph and the seeded edge array, so one harness instance is safe to call twice in either
// order. Setup draws the edges from one fixed seed, so the same NodeCount must rebuild the same graph;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumWeightedSubgraphWithTheRequiredPathsBenchmarksTests
{
    private const int SmallestNodeCount = 30;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().PerNodePointToPointSearch(),
            BuildHarness().PerNodePointToPointSearch());

    [Fact]
    public void PerNodePointToPointSearch_SeededRequiredPaths_AgreesWithReverseGraphDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReverseGraphDijkstra(), harness.PerNodePointToPointSearch());
    }

    [Fact]
    public void ReverseGraphDijkstra_SeededRequiredPaths_AgreesWithPerNodePointToPointSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PerNodePointToPointSearch(), harness.ReverseGraphDijkstra());
    }

    private static MinimumWeightedSubgraphWithTheRequiredPathsBenchmarks BuildHarness()
    {
        var harness = new MinimumWeightedSubgraphWithTheRequiredPathsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
