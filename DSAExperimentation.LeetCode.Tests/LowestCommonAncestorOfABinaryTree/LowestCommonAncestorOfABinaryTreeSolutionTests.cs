using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.LowestCommonAncestorOfABinaryTree;

namespace DSAExperimentation.LeetCode.Tests.LowestCommonAncestorOfABinaryTree;

// Harness only. The strategy is LowestCommonAncestorOfABinaryTreeSolution's - this
// file just pins it to LeetCode's published examples plus a couple of added cases.
// A row states its tree as LeetCode's level-order array and p and q by value -
// LeetCode guarantees every value is unique, so a value names exactly one node -
// and each run rebuilds a fresh tree from that array.
public sealed partial class LowestCommonAncestorOfABinaryTreeSolutionTests
{
    private const string EmptyTreeRow = "an Examples row states an empty tree, which LeetCode's constraints rule out";

    // The tree LeetCode's examples 1 and 2 share:
    //            3
    //          /   \
    //         5     1
    //        / \   / \
    //       6   2 0   8
    //          / \
    //         7   4
    private static readonly int?[] SharedExampleTree = [3, 5, 1, 6, 2, 0, 8, null, null, 7, 4];

    public static TheoryData<int?[], int, int, int> Examples =>
        new()
        {
            { SharedExampleTree, 5, 1, 3 }, // LeetCode's example 1: nodes in different subtrees of the root
            { SharedExampleTree, 5, 4, 5 }, // LeetCode's example 2: one node is an ancestor of the other
            { [1, 2], 1, 2, 1 }, // LeetCode's example 3: the root and its only child
            { SharedExampleTree, 7, 4, 2 }, // both nodes leaves of the same immediate parent
            { SharedExampleTree, 6, 4, 5 }, // diverges partway down, not at the root
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLcaByAncestryWalk_LeetCodeExamples_ReturnsTheAncestor(
        int?[] levelOrder, int first, int second, int expected)
    {
        var root = BuildTree(levelOrder);
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

    // LeetCode's constraints give every tree at least two nodes, so each row's array
    // has a root; an empty one could only be a mistake in that TheoryData.
    private static BinaryTreeNode<int> BuildTree(int?[] levelOrder) =>
        LeetCodeWireFormat.ToBinaryTree(levelOrder) ?? throw new InvalidOperationException(EmptyTreeRow);

    // Every pair the Examples rows above name is drawn from that row's own tree, so a
    // walk from its root always finds them; a value absent from the tree could only be
    // a mistake in that TheoryData.
    private static BinaryTreeNode<int> FindNode(BinaryTreeNode<int> root, int value) =>
        TryFindNode(root, value)
        ?? throw new InvalidOperationException(
            $"the Examples rows name only nodes of their own tree, but {value} is not in it");

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
