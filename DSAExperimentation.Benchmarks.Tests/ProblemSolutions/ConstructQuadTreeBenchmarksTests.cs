using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructQuadTreeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a raw cell scan against one Fenwick tree per row - so a harness
// whose arms disagree is timing two different problems. Both arms return the built QuadTreeNode tree
// as object?, which is not comparable by value, so each is projected with QuadTreeGrid.Materialize -
// the walk LC 427's own coverage test decodes its trees with - into the grid the tree denotes. That
// projection preserves the claim: LC 427's answer is a compression of the input grid, so two trees
// that materialize to the same grid are the same answer to the question being timed.
//
// AnswerText.Of renders the materialized jagged grid element-wise; Assert.Equal on int[][] itself
// would fall back to reference equality for the inner arrays and pass every time.
//
// Setup builds a grid split top/bottom into halves of 0s and 1s, so all four top-level quadrants are
// genuinely uniform and must be confirmed either way; the materialized tree must reproduce that
// grid, and the same Size must rebuild it.
public sealed partial class ConstructQuadTreeBenchmarksTests
{
    private const int SmallestSize = 16;

    // Setup's split is top/bottom, so the two halves of the grid are its row count over this.
    private const int TopBottomSplitDivisor = 2;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameTopBottomSplitGrid()
    {
        Assert.Equal(AnswerText.Of(SplitGrid()), AnswerText.Of(Materialized(BuildHarness().BruteForceCellScan())));
        Assert.Equal(
            AnswerText.Of(Materialized(BuildHarness().BruteForceCellScan())),
            AnswerText.Of(Materialized(BuildHarness().BruteForceCellScan())));
    }

    [Fact]
    public void BruteForceCellScan_TopBottomSplitGrid_AgreesWithRowFenwickTreeQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Materialized(harness.RowFenwickTreeQuery())),
            AnswerText.Of(Materialized(harness.BruteForceCellScan())));
    }

    [Fact]
    public void RowFenwickTreeQuery_TopBottomSplitGrid_AgreesWithBruteForceCellScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Materialized(harness.BruteForceCellScan())),
            AnswerText.Of(Materialized(harness.RowFenwickTreeQuery())));
    }

    private static int[][] SplitGrid()
    {
        var grid = QuadTreeGrid.Allocate(SmallestSize);

        for (var row = SmallestSize / TopBottomSplitDivisor; row < SmallestSize; row++)
        {
            Array.Fill(grid[row], 1);
        }

        return grid;
    }

    // The arm returns object? rather than the internal QuadTreeNode itself (CS0050), so the tree is
    // read back out of the boxed result before the decode walks it.
    private static int[][] Materialized(object? tree) =>
        QuadTreeGrid.Materialize((QuadTreeNode)tree!, SmallestSize);

    private static ConstructQuadTreeBenchmarks BuildHarness()
    {
        var harness = new ConstructQuadTreeBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
