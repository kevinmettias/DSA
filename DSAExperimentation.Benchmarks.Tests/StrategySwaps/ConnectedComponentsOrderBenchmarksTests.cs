using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for ConnectedComponentsOrderBenchmarks (ARCHITECTURE 17.9): the island count
// follows from the grid's construction alone. Along a 100-cell axis, every third cell (offsets
// 2, 5, ..., 98) is water, splitting the rest into 34 land runs - 33 of two cells and a final single
// cell at offset 99 - so the grid holds 34 * 34 islands, whatever order they are walked in.
public sealed partial class ConnectedComponentsOrderBenchmarksTests
{
    private const int SmallestSide = 100;
    private const int IslandsAtSmallestSide = 1_156;

    [Fact]
    public void DepthFirst_IslandGrid_CountsEveryIsland() =>
        Assert.Equal(IslandsAtSmallestSide, BuildHarness().DepthFirst());

    [Fact]
    public void BreadthFirst_IslandGrid_CountsEveryIsland() =>
        Assert.Equal(IslandsAtSmallestSide, BuildHarness().BreadthFirst());

    private static ConnectedComponentsOrderBenchmarks BuildHarness()
    {
        var harness = new ConnectedComponentsOrderBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
