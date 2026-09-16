using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToMakeAtLeastOneValidPathInAGridBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the textbook O(V^2) Dijkstra that
// linear-scans the unsettled distance table each round against the same search behind this repo's
// Heap - so a harness whose arms disagree is timing two different grids. Both arms answer with an int
// minimum cost, which they compare directly. Setup draws the arrow directions from one seeded stream,
// so the same Size must rebuild the same grid.
public sealed partial class MinimumCostToMakeAtLeastOneValidPathInAGridBenchmarksTests
{
    private const int SmallestSize = 15;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameGrid() =>
        Assert.Equal(BuildHarness().NaiveDijkstra(), BuildHarness().NaiveDijkstra());

    [Fact]
    public void NaiveDijkstra_RandomArrowGrid_AgreesWithHeapDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapDijkstra(), harness.NaiveDijkstra());
    }

    [Fact]
    public void HeapDijkstra_RandomArrowGrid_AgreesWithNaiveDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveDijkstra(), harness.HeapDijkstra());
    }

    private static MinimumCostToMakeAtLeastOneValidPathInAGridBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToMakeAtLeastOneValidPathInAGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
