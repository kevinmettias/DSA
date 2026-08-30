using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IncreasingOrderSearchTree;

// LeetCode 897. Increasing Order Search Tree: an in-order walk of a BST visits
// values in ascending order, so relinking each visited node's Left to null and
// chaining it onto the previously-visited node's Right builds the right-only
// increasing chain in a single pass - this repo's own InOrderTraversal/IInOrderHooks
// over BinaryTreeNode<int>, the same rewrite-during-walk composition
// ConvertBSTToGreaterTreeTests and KthSmallestElementInABSTTests already use.
public sealed partial class IncreasingOrderSearchTreeTests
{
    [Fact]
    public void IncreasingBst_ClassicExample_ReturnsRightOnlyChainInSortedOrder()
    {
        // [5,3,8,2,4,7,9] -> 1,2,3,4,5,7,8,9 chained right-only
        var root = new BinaryTreeNode<int>(5)
        {
            Left = new(3) { Left = new(2) { Left = new(1) }, Right = new(4) },
            Right = new(8) { Left = new(7), Right = new(9) },
        };

        var result = IncreasingBst(root);

        Assert.Equal([1, 2, 3, 4, 5, 7, 8, 9], RightChain(result));
    }

    [Fact]
    public void IncreasingBst_SingleNode_ReturnsThatNode()
    {
        var root = new BinaryTreeNode<int>(1);

        var result = IncreasingBst(root);

        Assert.Equal([1], RightChain(result));
    }

    private static BinaryTreeNode<int>? IncreasingBst(BinaryTreeNode<int>? root)
    {
        var dummy = new BinaryTreeNode<int>(0);
        State.Tail.Value = dummy;

        InOrderTraversal.Walk<int, RelinkHooks>(root);

        return dummy.Right;
    }

    private readonly struct RelinkHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            node.Left = null;
            State.Tail.Value!.Right = node;
            State.Tail.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Tail = new();
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
