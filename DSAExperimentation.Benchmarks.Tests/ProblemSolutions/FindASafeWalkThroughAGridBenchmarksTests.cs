using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindASafeWalkThroughAGridBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a damage array carried through the grid scan against
// a prepared cell-cost graph run through this repo's Dijkstra - so a harness whose arms disagree is
// walking two different grids, which is exactly the risk the prepared-graph arm's [GlobalSetup]
// introduces by building the graph separately from the grid. Both arms return bool. Setup hands both
// arms LeetCode 3286's own maximum health, twice the grid's side, which the class documents as what
// keeps every run's answer meaningful instead of trivially false; the grid comes from a seeded
// fixture, so the same GridSize must rebuild the same grid and the same graph.
public sealed partial class FindASafeWalkThroughAGridBenchmarksTests
{
    private const int SmallestGridSize = 10;

    // Setup's documented outcome: at LeetCode's own health cap for this grid size, the walk is safe.
    private const bool ExpectedIsSafe = true;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSamePreparedGraph()
    {
        Assert.Equal(ExpectedIsSafe, BuildHarness().IsSafeByBruteForceArrayDijkstra());

        Assert.Equal(
            BuildHarness().IsSafeByWeightedGridDijkstra(),
            BuildHarness().IsSafeByWeightedGridDijkstra());
    }

    [Fact]
    public void IsSafeByBruteForceArrayDijkstra_SeededGridAtHealthCap_AgreesWithWeightedGridDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsSafeByWeightedGridDijkstra(), harness.IsSafeByBruteForceArrayDijkstra());
    }

    [Fact]
    public void IsSafeByWeightedGridDijkstra_SeededGridAtHealthCap_AgreesWithBruteForceArrayDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsSafeByBruteForceArrayDijkstra(), harness.IsSafeByWeightedGridDijkstra());
    }

    private static FindASafeWalkThroughAGridBenchmarks BuildHarness()
    {
        var harness = new FindASafeWalkThroughAGridBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
