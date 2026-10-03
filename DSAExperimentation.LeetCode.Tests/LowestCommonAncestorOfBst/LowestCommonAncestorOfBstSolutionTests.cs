using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.LowestCommonAncestorOfBst;

namespace DSAExperimentation.LeetCode.Tests.LowestCommonAncestorOfBst;

// Harness only. Both strategies are LowestCommonAncestorOfBstSolution's: the
// textbook BST descent, and this repo's generic tree LCA engine composed over the
// same BinaryTreeNode<int> nodes - the same "generic engines are a free win"
// payoff phase 1 of this repo's tree work already found for
// traversal/metrics/paths. A row states its tree as LeetCode's level-order array
// and p and q by value - LeetCode guarantees every value is unique, so a value
// names exactly one node - and each run rebuilds a fresh tree from that array.
public sealed partial class LowestCommonAncestorOfBstSolutionTests
{
    private const string EmptyTreeRow = "an Examples row states an empty tree, which LeetCode's constraints rule out";

    // The BST LeetCode's examples 1 and 2 share:
    //            6
    //          /   \
    //         2     8
    //        / \   / \
    //       0   4 7   9
    //          / \
    //         3   5
    private static readonly int?[] SharedExampleTree = [6, 2, 8, 0, 4, 7, 9, null, null, 3, 5];

    public static TheoryData<int?[], int, int, int> Examples =>
        new()
        {
            { SharedExampleTree, 2, 8, 6 }, // LeetCode's example 1: nodes in different subtrees of the root
            { SharedExampleTree, 2, 4, 2 }, // LeetCode's example 2: one node is an ancestor of the other
            { [2, 1], 2, 1, 2 }, // LeetCode's example 3: the root and its only child
            { SharedExampleTree, 0, 5, 2 }, // both nodes several levels deep, diverging at 2
            { SharedExampleTree, 7, 9, 8 }, // both nodes leaves of the same immediate parent
            { SharedExampleTree, 3, 9, 6 }, // diverges only at the root
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLcaByAncestryWalk_LeetCodeExamples_ReturnsTheAncestor(
        int?[] levelOrder, int first, int second, int expected)
    {
        var root = BuildTree(levelOrder);
        var firstNode = FindNode(root, first);
        var secondNode = FindNode(root, second);

        var lca = LowestCommonAncestorOfBstSolution.FindLcaByAncestryWalk(
            root, firstNode, secondNode);

        // Both queried values come from FindNode walking this same root, so the
        // engine always has both reachable and can return null only when it has
        // neither - which cannot happen here. IsType asks for that node instead of
        // promising it.
        Assert.Equal(expected, Assert.IsType<BinaryTreeNode<int>>(lca).Value);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLcaByBstValueComparison_LeetCodeExamples_ReturnsTheAncestor(
        int?[] levelOrder, int first, int second, int expected)
    {
        var root = BuildTree(levelOrder);
        var firstNode = FindNode(root, first);
        var secondNode = FindNode(root, second);

        var lca = LowestCommonAncestorOfBstSolution.FindLcaByBstValueComparison(
            root, firstNode, secondNode);

        // Both queried values come from FindNode walking this same BST, so the
        // descent always reaches a splitting node and can return null only when a
        // value is absent - which cannot happen here. IsType asks for that node
        // instead of promising it.
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
