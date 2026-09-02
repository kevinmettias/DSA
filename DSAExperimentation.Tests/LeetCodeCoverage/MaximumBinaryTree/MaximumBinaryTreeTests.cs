using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumBinaryTree;

// LeetCode 654. Maximum Binary Tree: the O(n) monotonic-stack construction -
// each new value pops every smaller node off this repo's own LIFO Stack<T>,
// chaining them under its own Left (each popped node was already the largest of
// everything to its left, so the chain preserves that ordering), then attaches
// under the remaining top's Right before pushing itself. This produces the same
// tree the textbook "find the max, split, recurse" definition does, without
// having to re-derive the split index explicitly - see MaximumBinaryTreeBenchmarks
// for that O(n^2)-worst-case alternative measured directly against this one.
public sealed partial class MaximumBinaryTreeTests
{
    [Fact]
    public void ConstructMaximumBinaryTree_ClassicExample_MatchesLeetCodeSample()
    {
        var root = Build([3, 2, 1, 6, 0, 5]);

        Assert.Equal(6, root!.Value);
        Assert.Equal(3, root.Left!.Value);
        Assert.Null(root.Left.Left);
        Assert.Equal(2, root.Left.Right!.Value);
        Assert.Null(root.Left.Right.Left);
        Assert.Equal(1, root.Left.Right.Right!.Value);
        Assert.Equal(5, root.Right!.Value);
        Assert.Equal(0, root.Right.Left!.Value);
        Assert.Null(root.Right.Right);
    }

    [Fact]
    public void ConstructMaximumBinaryTree_AscendingInput_ProducesLeftSkewedChain()
    {
        var root = Build([1, 2, 3]);

        Assert.Equal(3, root!.Value);
        Assert.Null(root.Right);
        Assert.Equal(2, root.Left!.Value);
        Assert.Null(root.Left.Right);
        Assert.Equal(1, root.Left.Left!.Value);
    }

    private static BinaryTreeNode<int>? Build(int[] nums)
    {
        var stack = new NodeStack();

        foreach (var num in nums)
        {
            PushValue(stack, num);
        }

        BinaryTreeNode<int>? root = null;

        while (stack.TryPop(out var remaining))
        {
            root = remaining;
        }

        return root;
    }

    private static void PushValue(NodeStack stack, int num)
    {
        var node = new BinaryTreeNode<int>(num);

        while (stack.TryPeek(out var smaller) && smaller.Value < num)
        {
            stack.TryPop(out _);
            node.Left = smaller;
        }

        if (stack.TryPeek(out var parent))
        {
            parent.Right = node;
        }

        stack.Push(node);
    }
}
