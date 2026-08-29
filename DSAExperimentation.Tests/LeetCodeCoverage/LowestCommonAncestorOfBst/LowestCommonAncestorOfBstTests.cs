using DSAExperimentation.Algorithms.Ancestry;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LowestCommonAncestorOfBst;

// LeetCode 235. Lowest Common Ancestor of a Binary Search Tree: proves this repo's
// generic tree LCA engine composes directly over a BinarySearchTree<T>'s nodes with
// zero BST-specific code of its own - the same "generic engines are a free win"
// payoff phase 1 of this repo's tree work already found for traversal/metrics/paths.
public sealed partial class LowestCommonAncestorOfBstTests
{
    [Fact]
    public void FindLca_TwoNodesInDifferentSubtrees_ReturnsRoot()
    {
        var root = BuildClassicExampleTree();

        var lca = FindLca(root, first: 2, second: 8);

        // presumption: allow -- both queried values come from FindNode walking the
        // same tree root roots, so Find always has both nodes reachable and can
        // only return null when neither is (impossible here).
        Assert.Equal(6, lca!.Value);
    }

    [Fact]
    public void FindLca_OneNodeIsAncestorOfTheOther_ReturnsTheAncestor()
    {
        var root = BuildClassicExampleTree();

        var lca = FindLca(root, first: 2, second: 4);

        // presumption: allow -- see FindLca_TwoNodesInDifferentSubtrees_ReturnsRoot's
        // own comment; the same reachability guarantee holds here.
        Assert.Equal(2, lca!.Value);
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

    private static BinaryTreeNode<int>? FindLca(BinaryTreeNode<int> root, int first, int second)
    {
        var firstNode = FindNode(root, first);
        var secondNode = FindNode(root, second);

        return LowestCommonAncestor.Find<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(
            root, firstNode, secondNode);
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
