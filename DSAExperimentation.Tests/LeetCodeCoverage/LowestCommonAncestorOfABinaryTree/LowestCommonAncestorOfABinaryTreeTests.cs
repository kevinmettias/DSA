using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LowestCommonAncestorOfABinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LowestCommonAncestorOfABinaryTree;

// Harness only. The strategy is LowestCommonAncestorOfABinaryTreeSolution's - this
// file just pins it to LeetCode's published examples plus a couple of added cases,
// against a fresh copy of LeetCode's own example tree.
public sealed partial class LowestCommonAncestorOfABinaryTreeTests
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
        var firstNode = FindNode(root, first);
        var secondNode = FindNode(root, second);

        var lca = LowestCommonAncestorOfABinaryTreeSolution.FindLcaByAncestryWalk(
            root, firstNode, secondNode);

        // Both queried values come from FindNode walking this same root, so the
        // walk below always has both reachable and can return null only when it
        // has neither - which cannot happen here. IsType asks for that node
        // instead of promising it, and pins it to the tree's own node type.
        Assert.Equal(expected, Assert.IsType<BinaryTreeNode<int>>(lca).Value);
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

    // Every pair the Examples rows above name is drawn from
    // BuildClassicExampleTree's tree, so a walk from its root always finds them; a
    // value absent from the tree could only be a mistake in that TheoryData.
    private static BinaryTreeNode<int> FindNode(BinaryTreeNode<int> root, int value) =>
        TryFindNode(root, value)
        ?? throw new InvalidOperationException(
            $"the Examples rows name only nodes of the example tree, but {value} is not in it");

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
