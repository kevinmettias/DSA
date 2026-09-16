using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumObstacleRemovalToReachCornerBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the fewest obstacle cells that have to be crossed
// to reach the far corner of the grid - so a harness whose arms disagree is timing two different
// problems. WeightedGridDijkstra is handed the obstacle-cost graph [GlobalSetup] already wired, so the
// comparison also pins that the hoisted graph is the one the array-scan arm's grid describes: a
// mis-wired edge would show up here as a different corner distance. Setup builds that grid from one
// fixed seed, so the same Size must rebuild the same grid.
public sealed partial class MinimumObstacleRemovalToReachCornerBenchmarksTests
{
    private const int SmallestSize = 15;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArrayScanDijkstra(), BuildHarness().ArrayScanDijkstra());

    [Fact]
    public void ArrayScanDijkstra_SameObstacleGrid_AgreesWithWeightedGridDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.WeightedGridDijkstra(), harness.ArrayScanDijkstra());
    }

    [Fact]
    public void WeightedGridDijkstra_SameObstacleGrid_AgreesWithArrayScanDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayScanDijkstra(), harness.WeightedGridDijkstra());
    }

    private static MinimumObstacleRemovalToReachCornerBenchmarks BuildHarness()
    {
        var harness = new MinimumObstacleRemovalToReachCornerBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
