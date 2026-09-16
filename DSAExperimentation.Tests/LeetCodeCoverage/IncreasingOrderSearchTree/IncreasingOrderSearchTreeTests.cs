using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.IncreasingOrderSearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IncreasingOrderSearchTree;

// Harness only: both strategies live in IncreasingOrderSearchTreeSolution. The
// pre-migration test only exercised the InOrderTraversal/IInOrderHooks composition;
// the recursive baseline (previously untested scaffolding inlined in
// IncreasingOrderSearchTreeBenchmarks as its [Benchmark(Baseline = true)] arm) gets
// the identical assertions here for the first time.
//
// BinaryTreeNode<int> is internal, so it cannot appear in a public TheoryData<...>
// member (CS0053). Each example therefore states its tree as a BST insertion order -
// this repo's own BinarySearchTree<int> builds the same shape the pre-migration test
// spelled out with node literals - plus the ascending right-only chain expected out.
public sealed class IncreasingOrderSearchTreeTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            {
                // LeetCode example 1: [5,3,6,2,4,null,8,1,null,null,null,7,9].
                [5, 3, 6, 2, 4, 8, 1, 7, 9],
                [1, 2, 3, 4, 5, 6, 7, 8, 9]
            },
            {
                // LeetCode example 2: [5,1,7].
                [5, 1, 7],
                [1, 5, 7]
            },
            {
                //       5
                //      / \        the pre-migration test's tree, unchanged
                //     3   8
                //    / \ / \
                //   2  4 7  9
                //  /
                // 1
                [5, 3, 8, 2, 4, 7, 9, 1],
                [1, 2, 3, 4, 5, 7, 8, 9]
            },
            {
                [1],
                [1]
            },
            {
                // Already a right-only chain, so the relink has nothing to move.
                [1, 2, 3, 4],
                [1, 2, 3, 4]
            },
            {
                // Left-only chain: every node becomes the tail's new right child.
                [4, 3, 2, 1],
                [1, 2, 3, 4]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IncreasingBstByRecursiveRelink_LeetCodeExamples_ReturnsRightOnlyChainInSortedOrder(
        int[] insertionOrder, int[] expected) =>
        Assert.Equal(
            expected,
            RightChain(IncreasingOrderSearchTreeSolution.IncreasingBstByRecursiveRelink(BuildTree(insertionOrder))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IncreasingBstByInOrderHooks_LeetCodeExamples_ReturnsRightOnlyChainInSortedOrder(
        int[] insertionOrder, int[] expected) =>
        Assert.Equal(
            expected,
            RightChain(IncreasingOrderSearchTreeSolution.IncreasingBstByInOrderHooks(BuildTree(insertionOrder))));

    private static BinaryTreeNode<int> BuildTree(int[] insertionOrder)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in insertionOrder)
        {
            tree.Insert(value);
        }

        // Every example above inserts at least one value, and BinarySearchTree.Insert
        // gives the tree its Root on the first of them.
        return tree.Root
            ?? throw new InvalidOperationException(
                "every example above inserts at least one value, so the first Insert set Root");
    }

    private static int[] RightChain(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();

        for (var node = root; node is not null; node = node.Right)
        {
            Assert.Null(node.Left);
            values.Add(node.Value);
        }

        return [.. values];
    }
}
