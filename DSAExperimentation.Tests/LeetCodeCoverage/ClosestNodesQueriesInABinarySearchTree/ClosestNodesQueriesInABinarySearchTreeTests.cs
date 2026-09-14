using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ClosestNodesQueriesInABinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestNodesQueriesInABinarySearchTree;

// Harness only. Both strategies are ClosestNodesQueriesInABinarySearchTreeSolution's -
// this file states LeetCode's examples once and asserts each strategy against them,
// including the linear per-query rescan that was previously a benchmark-only arm and
// so was never checked against an expected answer at all.
//
// Each tree is given as its BST insertion order rather than as a node graph:
// BinaryTreeNode<TValue> is internal, so a public MemberData member cannot name it,
// and the insertion order pins the same shape LeetCode draws.
public sealed class ClosestNodesQueriesInABinarySearchTreeTests
{
    public static TheoryData<int[], int[], int[][]> Examples =>
        new()
        {
            // root = [6,2,13,1,4,9,15,null,null,null,null,null,null,14], queries = [2,5,16]
            {
                [6, 2, 13, 1, 4, 9, 15, 14],
                [2, 5, 16],
                [[2, 2], [4, 6], [15, -1]]
            },

            // root = [4,null,9], queries = [3]
            { [4, 9], [3], [[-1, 4]] },

            // Queries past both ends of the tree, and one landing exactly on a node.
            { [5, 3, 8], [1, 10, 5], [[-1, 3], [8, -1], [5, 5]] },

            // A single-node tree: the one value is its own floor and ceiling, and
            // is the only answer either side can ever report.
            { [7], [7, 6, 8], [[7, 7], [-1, 7], [7, -1]] },

            // A deeper tree, with the four cases interleaved in one query list so a
            // strategy that resets state per query is exercised in both directions.
            {
                [10, 5, 15, 3, 7, 13, 18],
                [4, 10, 20, 1],
                [[3, 5], [10, 10], [18, -1], [-1, 3]]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestNodesByLinearScan_LeetCodeExamples_ReturnsFloorAndCeilingPerQuery(
        int[] insertionOrder,
        int[] queries,
        int[][] expected) =>
        Assert.Equal(
            expected,
            ClosestNodesQueriesInABinarySearchTreeSolution.ClosestNodesByLinearScan(
                BuildTree(insertionOrder),
                queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestNodesByInOrderBinarySearch_LeetCodeExamples_ReturnsFloorAndCeilingPerQuery(
        int[] insertionOrder,
        int[] queries,
        int[][] expected) =>
        Assert.Equal(
            expected,
            ClosestNodesQueriesInABinarySearchTreeSolution.ClosestNodesByInOrderBinarySearch(
                BuildTree(insertionOrder),
                queries));

    private static BinaryTreeNode<int>? BuildTree(int[] insertionOrder)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in insertionOrder)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }
}
