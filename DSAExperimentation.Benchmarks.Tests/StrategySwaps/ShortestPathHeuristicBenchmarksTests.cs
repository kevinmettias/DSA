using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for ShortestPathHeuristicBenchmarks (ARCHITECTURE 17.9): the generic check holds
// the arms to agreeing with each other, which a shared wrong distance would pass. The workload is an
// open unit-weight grid, so the far corner sits at exactly the Manhattan distance - a decisive value
// the fixture pins on the baseline independently of both arms.
public sealed partial class ShortestPathHeuristicBenchmarksTests
{
    private const int SmallestGridSize = 20;
    private const int CornerDistance = (SmallestGridSize - 1) * 2;

    [Fact]
    public void Dijkstra_OpenGrid_ReachesTheFarCornerAtTheManhattanDistance() =>
        Assert.Equal(CornerDistance, BuildHarness().Dijkstra());

    private static ShortestPathHeuristicBenchmarks BuildHarness()
    {
        var harness = new ShortestPathHeuristicBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
