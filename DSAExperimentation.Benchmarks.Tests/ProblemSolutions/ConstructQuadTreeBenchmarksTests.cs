using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructQuadTreeBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to building the same tree on a rebuilt workload, so this pins the baseline's tree
// to the grid Setup builds, derived independently of either arm. The arm returns the built
// QuadTreeNode tree, so it is projected with QuadTreeGrid.Materialize - the walk LC 427's own
// coverage test decodes its trees with - into the grid the tree denotes: LC 427's answer is a
// compression of the input grid, so the tree must materialize back to it.
//
// AnswerGraphText.Of renders the materialized jagged grid element-wise; Assert.Equal on int[][] itself
// would fall back to reference equality for the inner arrays and pass every time.
//
// Setup builds a grid split top/bottom into halves of 0s and 1s, so all four top-level quadrants are
// genuinely uniform and must be confirmed either way; the materialized tree must reproduce that
// grid.
public sealed partial class ConstructQuadTreeBenchmarksTests
{
    private const int SmallestSize = 16;

    // Setup's split is top/bottom, so the two halves of the grid are its row count over this.
    private const int TopBottomSplitDivisor = 2;

    [Fact]
    public void BruteForceCellScan_TopBottomSplitGrid_MaterializesTheSplitGrid() =>
        Assert.Equal(AnswerGraphText.Of(SplitGrid()), AnswerGraphText.Of(Materialized(BuildHarness().BruteForceCellScan())));

    private static int[][] SplitGrid()
    {
        var grid = QuadTreeGrid.Allocate(SmallestSize);

        for (var row = SmallestSize / TopBottomSplitDivisor; row < SmallestSize; row++)
        {
            Array.Fill(grid[row], 1);
        }

        return grid;
    }

    private static int[][] Materialized(QuadTreeNode tree) =>
        QuadTreeGrid.Materialize(tree, SmallestSize);

    private static ConstructQuadTreeBenchmarks BuildHarness()
    {
        var harness = new ConstructQuadTreeBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
