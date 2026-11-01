using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ZigzagGridTraversalWithSkipBenchmarks (ARCHITECTURE 17.9): the generic check
// holds the arms to agreeing with each other, which a shared wrong traversal would pass. The grid is
// square and the skip keeps every other value of the snake order starting with the first, so the
// answer's length is a decisive value independent of both arms, pinned on the baseline.
public sealed partial class ZigzagGridTraversalWithSkipBenchmarksTests
{
    private const int SmallestGridSize = 10;
    private const int ExpectedTraversalLength = (SmallestGridSize * SmallestGridSize + 1) / 2;

    [Fact]
    public void IndexFormula_EveryOtherCellOfTheSnakeOrder_KeepsHalfTheCells() =>
        Assert.Equal(ExpectedTraversalLength, BuildHarness().IndexFormula().Length);

    private static ZigzagGridTraversalWithSkipBenchmarks BuildHarness()
    {
        var harness = new ZigzagGridTraversalWithSkipBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
