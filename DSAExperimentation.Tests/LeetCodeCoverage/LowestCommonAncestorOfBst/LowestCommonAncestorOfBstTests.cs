using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LowestCommonAncestorOfBst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LowestCommonAncestorOfBst;

// Harness only. The single strategy is LowestCommonAncestorOfBstSolution's,
// composing this repo's generic tree LCA engine over a BinarySearchTree<int>'s
// nodes - the same "generic engines are a free win" payoff phase 1 of this repo's
// tree work already found for traversal/metrics/paths.
public sealed class LowestCommonAncestorOfBstTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 2, 8, 6 }, // LeetCode's example 1: nodes in different subtrees of the root
            { 2, 4, 2 }, // LeetCode's example 2: one node is an ancestor of the other
            { 0, 5, 2 }, // both nodes several levels deep, diverging at 2
            { 7, 9, 8 }, // both nodes leaves of the same immediate parent
            { 3, 9, 6 }, // diverges only at the root
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLcaByAncestryWalk_LeetCodeExamples_ReturnsTheAncestor(int first, int second, int expected)
    {
        var root = BuildClassicExampleTree();
        var firstNode = FindNode(root, first);
        var secondNode = FindNode(root, second);

        var lca = LowestCommonAncestorOfBstSolution.FindLcaByAncestryWalk(
            root, firstNode, secondNode);

        // presumption: allow -- both queried values come from FindNode walking the
        // same tree root, so Find always has both nodes reachable and can only
        // return null when neither is (impossible here).
        Assert.Equal(expected, lca!.Value);
    }

    private static BinaryTreeNode<int> BuildClassicExampleTree()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in new[] { 6, 2, 8, 0, 4, 7, 9, 3, 5 })
        {
            tree.Insert(value);
        }

        // presumption: allow -- tree always has nine values inserted above before
        // returning, so Root is never null here.
        return tree.Root!;
    }

    private static BinaryTreeNode<int> FindNode(BinaryTreeNode<int> root, int value)
    {
        var node = root;

        while (node.Value != value)
        {
            node = value < node.Value ? LeftChild(node) : RightChild(node);
        }

        return node;
    }

    // presumption: allow -- BinarySearchTree.Insert only ever creates a full node
    // for a value it actually stored, and FindNode only ever descends toward a
    // value BuildClassicExampleTree is known to have inserted - so the child this
    // walk needs is always present.
    private static BinaryTreeNode<int> LeftChild(BinaryTreeNode<int> node) => node.Left!;

    private static BinaryTreeNode<int> RightChild(BinaryTreeNode<int> node) => node.Right!;
}
