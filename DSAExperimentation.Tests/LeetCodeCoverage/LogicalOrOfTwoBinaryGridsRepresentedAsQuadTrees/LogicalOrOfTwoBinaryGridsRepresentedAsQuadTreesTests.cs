using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructQuadTree;
using DSAExperimentation.LeetCode.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LogicalOrOfTwoBinaryGridsRepresentedAsQuadTrees;

// LeetCode 558. Logical Or of Two Binary Grids Represented as Quad-Trees: harness
// only. All three strategies are
// LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution's; each example builds the
// two input quad-trees from grids via ConstructQuadTreeSolution (LC 427), runs one OR
// strategy, and checks that decoding the result tree back to a grid reproduces the
// expected elementwise OR exactly.
public sealed class LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesTests
{
    public static TheoryData<int[][], int[][], int[][]> Examples =>
        new()
        {
            { [[1, 1], [1, 1]], [[0, 0], [0, 0]], [[1, 1], [1, 1]] },
            { [[1, 0], [0, 1]], [[0, 1], [1, 0]], [[1, 1], [1, 1]] },
            {
                [
                    [1, 1, 0, 0],
                    [1, 1, 0, 0],
                    [0, 0, 0, 0],
                    [0, 0, 0, 0],
                ],
                [
                    [0, 0, 0, 0],
                    [0, 0, 0, 0],
                    [0, 0, 1, 0],
                    [0, 0, 0, 1],
                ],
                [
                    [1, 1, 0, 0],
                    [1, 1, 0, 0],
                    [0, 0, 1, 0],
                    [0, 0, 0, 1],
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrByDirectRecursiveMerge_LeetCodeExamples_ReconstructsElementwiseOrOfBothGrids(
        int[][] grid1, int[][] grid2, int[][] expected)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByDirectRecursiveMerge(Build(grid1), Build(grid2));

        AssertReconstructsExpectedGrid(merged, expected);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrByBruteForceGridMaterialize_LeetCodeExamples_ReconstructsElementwiseOrOfBothGrids(
        int[][] grid1, int[][] grid2, int[][] expected)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByBruteForceGridMaterialize(
            Build(grid1), Build(grid2), grid1.Length);

        AssertReconstructsExpectedGrid(merged, expected);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrByFenwickGridMaterialize_LeetCodeExamples_ReconstructsElementwiseOrOfBothGrids(
        int[][] grid1, int[][] grid2, int[][] expected)
    {
        var merged = LogicalOrOfTwoBinaryGridsRepresentedAsQuadTreesSolution.OrByFenwickGridMaterialize(
            Build(grid1), Build(grid2), grid1.Length);

        AssertReconstructsExpectedGrid(merged, expected);
    }

    private static QuadTreeNode Build(int[][] grid) => ConstructQuadTreeSolution.BuildByBruteForceCellScan(grid);

    private static void AssertReconstructsExpectedGrid(QuadTreeNode result, int[][] expected) =>
        Assert.Equal(expected, QuadTreeGrid.Materialize(result, expected.Length));
}
