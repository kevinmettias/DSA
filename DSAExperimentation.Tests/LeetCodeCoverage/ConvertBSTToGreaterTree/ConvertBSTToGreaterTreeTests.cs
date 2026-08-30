using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertBSTToGreaterTree;

// LeetCode 538. Convert BST to Greater Tree: every node's value becomes the sum of
// itself and everything strictly greater in the tree. InOrderTraversal only walks
// ascending (left, visit, right - see its own doc comment on why it's hardwired that
// direction, not generic over it), so this composes two ascending
// InOrderTraversal/IInOrderHooks passes over the same BinaryTreeNode<int> tree
// instead of a single hand-rolled right-to-left walk: pass 1 collects values in
// ascending order (the same collect-hooks idiom DeleteNodeInABSTTests already uses),
// a suffix sum over that ascending list gives each rank's "sum of everything >= it,"
// and pass 2 walks ascending again reassigning node.Value from that precomputed sum
// by rank. This is exactly the composition BinaryTreeNode<TValue>'s own doc comment
// names as the reason Value is settable in the first place.
public sealed partial class ConvertBSTToGreaterTreeTests
{
    [Fact]
    public void ConvertToGreaterTree_LeetCodeExample_AccumulatesSumOfGreaterValues()
    {
        // [0, null, 1] -> [1, null, 1]
        var root = new BinaryTreeNode<int>(0) { Right = new(1) };

        ConvertToGreaterTree(root);

        Assert.Equal(1, root.Value);
        Assert.Equal(1, root.Right!.Value);
    }

    [Fact]
    public void ConvertToGreaterTree_LargerTree_EachNodeGetsSumOfItselfAndGreaterKeys()
    {
        //       5                  29
        //      / \                /  \
        //     3   8      ->     36    17
        //    / \ / \            / \   / \
        //   2  4 7  9         38 33  24  9
        var root = new BinaryTreeNode<int>(5)
        {
            Left = new(3) { Left = new(2), Right = new(4) },
            Right = new(8) { Left = new(7), Right = new(9) },
        };

        ConvertToGreaterTree(root);

        Assert.Equal(29, root.Value);
        Assert.Equal(36, root.Left!.Value);
        Assert.Equal(38, root.Left.Left!.Value);
        Assert.Equal(33, root.Left.Right!.Value);
        Assert.Equal(17, root.Right!.Value);
        Assert.Equal(24, root.Right.Left!.Value);
        Assert.Equal(9, root.Right.Right!.Value);
    }

    private static void ConvertToGreaterTree(BinaryTreeNode<int> root)
    {
        State.Values.Value = [];
        InOrderTraversal.Walk<int, CollectHooks>(root);
        var ascending = State.Values.Value!;

        var suffixSums = new int[ascending.Count];
        var runningSum = 0;

        for (var i = ascending.Count - 1; i >= 0; i--)
        {
            runningSum += ascending[i];
            suffixSums[i] = runningSum;
        }

        State.Index.Value = 0;
        State.GreaterSums.Value = suffixSums;
        InOrderTraversal.Walk<int, AssignHooks>(root);
    }

    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Values.Value!.Add(node.Value);
    }

    private readonly struct AssignHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            node.Value = State.GreaterSums.Value![State.Index.Value];
            State.Index.Value++;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<List<int>?> Values = new();
        public static readonly AsyncLocal<int> Index = new();
        public static readonly AsyncLocal<int[]?> GreaterSums = new();
    }
}
