using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks (ARCHITECTURE
// 17.9): its three arms are competing strategies for the same question - two that materialize both
// trees back into grids and rebuild the OR with LC 427's builders, against the canonical direct
// recursive merge - so a harness whose arms disagree is timing two different problems. All three
// return the merged QuadTreeNode as object? (CS0050), which is not comparable by value, so each is
// projected with QuadTreeGrid.Materialize - the walk LC 558's own coverage test decodes its trees
// with - into the grid the tree denotes, and AnswerText.Of renders that jagged grid element-wise.
// Assert.Equal on int[][] itself would fall back to reference equality for the inner arrays.
//
// Setup splits grid1 top/bottom and grid2 left/right, so the OR of the two documented grids is the
// answer independently of all three arms, and the same Size must rebuild it.
public sealed partial class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarksTests
{
    private const int SmallestSize = 16;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameSplitGridsAndTheirOr()
    {
        Assert.Equal(
            AnswerText.Of(ExpectedOrGrid()),
            AnswerText.Of(Materialized(BuildHarness().BruteForceGridMaterialize())));
        Assert.Equal(
            AnswerText.Of(Materialized(BuildHarness().DirectRecursiveMerge())),
            AnswerText.Of(Materialized(BuildHarness().DirectRecursiveMerge())));
    }

    [Fact]
    public void BruteForceGridMaterialize_SplitGrids_AgreesWithDirectRecursiveMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Materialized(harness.DirectRecursiveMerge())),
            AnswerText.Of(Materialized(harness.BruteForceGridMaterialize())));
    }

    [Fact]
    public void FenwickGridMaterialize_SplitGrids_AgreesWithBruteForceGridMaterialize()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Materialized(harness.BruteForceGridMaterialize())),
            AnswerText.Of(Materialized(harness.FenwickGridMaterialize())));
    }

    [Fact]
    public void DirectRecursiveMerge_SplitGrids_AgreesWithFenwickGridMaterialize()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Materialized(harness.FenwickGridMaterialize())),
            AnswerText.Of(Materialized(harness.DirectRecursiveMerge())));
    }

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

    // The arm returns object? rather than the internal QuadTreeNode itself (CS0050), so the tree is
    // read back out of the boxed result before the decode walks it.
    private static int[][] Materialized(object? tree) =>
        QuadTreeGrid.Materialize(Assert.IsType<QuadTreeNode>(tree), SmallestSize);

    private static LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks BuildHarness()
    {
        var harness = new LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
