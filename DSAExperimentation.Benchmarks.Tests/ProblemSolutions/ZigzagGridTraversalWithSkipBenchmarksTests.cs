using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ZigzagGridTraversalWithSkipBenchmarks (ARCHITECTURE 17.9): both arms are
// competing traversals for one grid, so a harness whose arms disagree is timing two different
// problems. The flattened snake order is read sequentially, so the answer's order is pinned and the
// comparison is order-sensitive. The grid is square and the skip keeps every other value of that
// order starting with the first, so the answer's length is a decisive value independent of both
// arms.
public sealed partial class ZigzagGridTraversalWithSkipBenchmarksTests
{
    private const int SmallestGridSize = 10;
    private const int ExpectedTraversalLength = (SmallestGridSize * SmallestGridSize + 1) / 2;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().IndexFormula()),
            AnswerText.Of(BuildHarness().IndexFormula()));

    [Fact]
    public void IndexFormula_AgreesWithRowStack()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.IndexFormula()), AnswerText.Of(harness.RowStack()));
    }

    [Fact]
    public void RowStack_AgreesWithIndexFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.RowStack()), AnswerText.Of(harness.IndexFormula()));
    }

    [Fact]
    public void IndexFormula_EveryOtherCellOfTheSnakeOrder_KeepsHalfTheCells() =>
        Assert.Equal(ExpectedTraversalLength, BuildHarness().IndexFormula().Length);

    [Fact]
    public void RowStack_EveryOtherCellOfTheSnakeOrder_KeepsHalfTheCells() =>
        Assert.Equal(ExpectedTraversalLength, BuildHarness().RowStack().Length);

    private static ZigzagGridTraversalWithSkipBenchmarks BuildHarness()
    {
        var harness = new ZigzagGridTraversalWithSkipBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
