using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for ShortestPathHeuristicBenchmarks (ARCHITECTURE 17.9): its two arms are one
// algorithm with the heuristic axis swapped, so they are competing strategies for one question and
// must answer it identically. The workload is an open unit-weight grid, so the far corner sits at
// exactly the Manhattan distance - a decisive value the seeded fixture pins independently of both
// arms.
public sealed partial class ShortestPathHeuristicBenchmarksTests
{
    private const int SmallestGridSize = 20;
    private const int CornerDistance = (SmallestGridSize - 1) * 2;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().Dijkstra()),
            AnswerText.Of(BuildHarness().Dijkstra()));

    [Fact]
    public void Dijkstra_AgreesWithAStar()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Dijkstra(), harness.AStar());
    }

    [Fact]
    public void AStar_AgreesWithDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AStar(), harness.Dijkstra());
    }

    [Fact]
    public void Dijkstra_OpenGrid_ReachesTheFarCornerAtTheManhattanDistance() =>
        Assert.Equal(CornerDistance, BuildHarness().Dijkstra());

    [Fact]
    public void AStar_OpenGrid_ReachesTheFarCornerAtTheManhattanDistance() =>
        Assert.Equal(CornerDistance, BuildHarness().AStar());

    private static ShortestPathHeuristicBenchmarks BuildHarness()
    {
        var harness = new ShortestPathHeuristicBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
