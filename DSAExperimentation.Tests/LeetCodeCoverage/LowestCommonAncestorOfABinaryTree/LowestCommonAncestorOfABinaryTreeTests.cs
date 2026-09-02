using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LowestCommonAncestorOfABinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LowestCommonAncestorOfABinaryTree;

// Harness only. The strategy is LowestCommonAncestorOfABinaryTreeSolution's - this
// file just pins it to LeetCode's published examples plus a couple of added cases,
// against a fresh copy of LeetCode's own example tree.
public sealed class LowestCommonAncestorOfABinaryTreeTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 5, 1, 3 }, // LeetCode's example 1: nodes in different subtrees of the root
            { 5, 4, 5 }, // LeetCode's example 2: one node is an ancestor of the other
            { 7, 4, 2 }, // both nodes leaves of the same immediate parent
            { 6, 4, 5 }, // diverges partway down, not at the root
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLcaByAncestryWalk_LeetCodeExamples_ReturnsTheAncestor(int first, int second, int expected)
    {
        var root = BuildClassicExampleTree();

        var lca = LowestCommonAncestorOfABinaryTreeSolution.FindLcaByAncestryWalk(
            root, FindNode(root, first), FindNode(root, second));

        // presumption: allow -- both queried values come from FindNode walking the
        // same tree root, so Find always has both nodes reachable and can only
        // return null when neither is (impossible here).
        Assert.Equal(expected, lca!.Value);
    }

    private static BinaryTreeNode<int> BuildClassicExampleTree()
    {
        // LeetCode's own example tree:
        //            3
        //          /   \
        //         5     1
        //        / \   / \
        //       6   2 0   8
        //          / \
        //         7   4
        var six = new BinaryTreeNode<int>(6);
        var seven = new BinaryTreeNode<int>(7);
        var four = new BinaryTreeNode<int>(4);
        var two = new BinaryTreeNode<int>(2) { Left = seven, Right = four };
        var five = new BinaryTreeNode<int>(5) { Left = six, Right = two };
        var zero = new BinaryTreeNode<int>(0);
        var eight = new BinaryTreeNode<int>(8);
        var one = new BinaryTreeNode<int>(1) { Left = zero, Right = eight };

        return new BinaryTreeNode<int>(3) { Left = five, Right = one };
    }

    private static BinaryTreeNode<int> FindNode(BinaryTreeNode<int> root, int value) =>
        TryFindNode(root, value)!;

    private static BinaryTreeNode<int>? TryFindNode(BinaryTreeNode<int>? node, int value)
    {
        if (node is null)
        {
            return null;
        }

        if (node.Value == value)
        {
            return node;
        }

        return TryFindNode(node.Left, value) ?? TryFindNode(node.Right, value);
    }
}
