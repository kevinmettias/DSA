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

    private static void AssertReconstructsExpectedGrid(QuadTreeNode result, int[][] expected)
    {
        var rebuilt = expected.Select(row => new int[row.Length]).ToArray();
        Fill(result, rebuilt, new Region(0, 0, expected.Length));

        Assert.Equal(expected, rebuilt);
    }

    private readonly record struct Region(int Row, int Col, int Size);

    private static void Fill(QuadTreeNode node, int[][] grid, Region region)
    {
        if (node.IsLeaf)
        {
            var value = node.Val ? 1 : 0;
            for (var r = region.Row; r < region.Row + region.Size; r++)
            {
                for (var c = region.Col; c < region.Col + region.Size; c++)
                {
                    grid[r][c] = value;
                }
            }

            return;
        }

        var half = region.Size / 2;
        Fill(Quadrant(node.TopLeft), grid, new Region(region.Row, region.Col, half));
        Fill(Quadrant(node.TopRight), grid, new Region(region.Row, region.Col + half, half));
        Fill(Quadrant(node.BottomLeft), grid, new Region(region.Row + half, region.Col, half));
        Fill(Quadrant(node.BottomRight), grid, new Region(region.Row + half, region.Col + half, half));
    }

    // A QuadTreeNode carries all four quadrants exactly when it is not a leaf. Every
    // IsLeaf: false node these cases decode is built by an initializer that fills all
    // four: ConstructQuadTreeSolution does that for the inputs Build makes, and the
    // strategy under test does it for the merged tree that comes back.
    private static QuadTreeNode Quadrant(QuadTreeNode? quadrant) =>
        quadrant ?? throw new InvalidOperationException(
            "a non-leaf QuadTreeNode carries all four children, and ConstructQuadTreeSolution builds one only that way.");
}
