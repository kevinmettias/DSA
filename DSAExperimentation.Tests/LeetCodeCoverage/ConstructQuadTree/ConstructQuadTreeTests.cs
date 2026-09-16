using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConstructQuadTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstructQuadTree;

// LeetCode 427. Construct Quad Tree: harness only. Both strategies are
// ConstructQuadTreeSolution's - this file pins them to LeetCode's published
// examples, checking both that construction finds the expected compression (via
// leaf count - an implementation that never merges uniform regions would still
// rebuild the grid correctly, but with far more leaves than expected) and that
// walking the resulting tree back into a grid (QuadTreeGrid.Materialize, the walk
// LC 558's harness decodes with too) reproduces the original input exactly.
public sealed partial class ConstructQuadTreeTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0]], 1 },
            { [[1, 1], [1, 1]], 1 },
            { [[0, 1], [1, 0]], 4 },
            {
                [
                    [1, 1, 1, 1, 0, 0, 0, 0],
                    [1, 1, 1, 1, 0, 0, 0, 0],
                    [1, 1, 1, 1, 0, 0, 0, 0],
                    [1, 1, 1, 1, 0, 0, 0, 0],
                    [1, 1, 1, 1, 1, 1, 1, 1],
                    [1, 1, 1, 1, 1, 1, 1, 1],
                    [1, 1, 1, 1, 1, 1, 1, 1],
                    [1, 1, 1, 1, 1, 1, 1, 1],
                ],
                4
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByBruteForceCellScan_LeetCodeExamples_RebuildsGridWithExpectedCompression(
        int[][] grid, int expectedLeafCount) =>
        AssertReconstructsGrid(ConstructQuadTreeSolution.BuildByBruteForceCellScan(grid), grid, expectedLeafCount);

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByRowFenwickTree_LeetCodeExamples_RebuildsGridWithExpectedCompression(
        int[][] grid, int expectedLeafCount) =>
        AssertReconstructsGrid(ConstructQuadTreeSolution.BuildByRowFenwickTree(grid), grid, expectedLeafCount);

    private static void AssertReconstructsGrid(QuadTreeNode root, int[][] grid, int expectedLeafCount)
    {
        Assert.Equal(expectedLeafCount, CountLeaves(root));

        Assert.Equal(grid, QuadTreeGrid.Materialize(root, grid.Length));
    }

    // ConstructQuadTreeSolution marks a node IsLeaf false only in the object
    // initializer that fills all four quadrants from its own recursive builds - in
    // both strategies, so neither the cell scan nor the Fenwick sum can reach a
    // non-leaf node with a quadrant missing. The leaf count below reads those
    // children, and asks for them here rather than promising them.
    private static QuadTreeNode Child(QuadTreeNode? child) =>
        child ?? throw new InvalidOperationException(
            "a non-leaf QuadTreeNode carries all four children - the solution sets IsLeaf false only after building them");

    // Both arms are values: 1 for a leaf, and a call naming the sum over a non-leaf's
    // four quadrants. The non-leaf arm stays a call rather than a hoisted local because
    // the condition guards it - a local above the expression would ask a leaf for the
    // four quadrants the solution never builds for one, which Child rejects.
    private static int CountLeaves(QuadTreeNode node) =>
        node.IsLeaf
            ? 1
            : CountQuadrantLeaves(node);

    private static int CountQuadrantLeaves(QuadTreeNode node) =>
        CountLeaves(Child(node.TopLeft)) + CountLeaves(Child(node.TopRight)) +
        CountLeaves(Child(node.BottomLeft)) + CountLeaves(Child(node.BottomRight));
}
