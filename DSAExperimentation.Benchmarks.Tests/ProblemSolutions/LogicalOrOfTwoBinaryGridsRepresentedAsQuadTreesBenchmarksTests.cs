using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks (ARCHITECTURE
// 17.9): Setup splits grid1 top/bottom and grid2 left/right, so the OR of the two documented grids is
// the answer independently of all three arms - an expected value the generic agreement check, which
// only compares arms with each other, cannot supply. The arm returns the merged QuadTreeNode, so it is
// projected with QuadTreeGrid.Materialize - the walk LC 558's own coverage test decodes its trees
// with - into the grid the tree denotes, and AnswerGraphText.Of renders that jagged grid
// element-wise. Assert.Equal on int[][] itself would fall back to reference equality for the inner
// arrays.
public sealed partial class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarksTests
{
    private const int SmallestSize = 16;

    [Fact]
    public void BruteForceGridMaterialize_SplitGrids_DenotesTheOrOfBothGrids() =>
        Assert.Equal(
            AnswerGraphText.Of(ExpectedOrGrid()),
            AnswerGraphText.Of(Materialized(BuildHarness().BruteForceGridMaterialize())));

    // grid1 is 0 in its top half and 1 below it, grid2 is 0 in its left half and 1 to the right of
    // it, so their OR is 0 exactly where both are 0 - the top-left quadrant.
    private static int[][] ExpectedOrGrid()
    {
        var grid = QuadTreeGrid.Allocate(SmallestSize);

        for (var row = 0; row < SmallestSize; row++)
        {
            for (var col = 0; col < SmallestSize; col++)
            {
                grid[row][col] = IsTopHalf(row) && IsLeftHalf(col) ? 0 : 1;
            }
        }

        return grid;
    }

    // A quad-tree region splits into 2x2 quadrants, so each dimension halves at every recursion
    // level - AlgorithmConstants.HalvingFactor, the same divisor the harness's own split helpers read.
    private static bool IsTopHalf(int row) => row < SmallestSize / AlgorithmConstants.HalvingFactor;

    private static bool IsLeftHalf(int col) => col < SmallestSize / AlgorithmConstants.HalvingFactor;

    private static int[][] Materialized(QuadTreeNode tree) =>
        QuadTreeGrid.Materialize(tree, SmallestSize);

    private static LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks BuildHarness()
    {
        var harness = new LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
