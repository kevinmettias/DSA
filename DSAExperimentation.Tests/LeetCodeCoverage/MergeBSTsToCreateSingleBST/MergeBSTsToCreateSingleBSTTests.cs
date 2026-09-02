using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeBSTsToCreateSingleBST;

// LeetCode 1932. Merge BSTs to Create Single BST: this repo's own BinaryTreeNode
// <TValue> is already a mutable reference type built for exactly this kind of
// splice (its own doc comment names "mutate-in-place operations" as the reason
// Left/Right are settable), so a leaf whose value matches another tree's root gets
// spliced by reassigning the parent's Left/Right pointer, no new node type needed.
// A HashMap<int,BinaryTreeNode<int>> indexes every candidate tree by its root value
// (removed once consumed, so a root can only be spliced in once) and a Set<int> of
// every leaf value across all trees finds the one root value that is never anyone's
// leaf - the overall merged root (zero or more than one such candidate means no
// valid merge exists). Splicing alone doesn't guarantee a valid BST - two spliced-
// together subtrees can still violate global ordering - so the merged tree is
// validated the same way AllElementsInTwoBinarySearchTreesTests/
// ValidateBinarySearchTreeTests already do: an in-order walk (this repo's own
// InOrderTraversal/IInOrderHooks) must produce strictly ascending values.
public sealed partial class MergeBSTsToCreateSingleBSTTests
{
    [Fact]
    public void CanMerge_LeafMatchesOtherTreesRoot_SplicesIntoOneValidBst()
    {
        // treeA: 2 -> left=1(leaf), right=4(leaf). treeB: 1 -> left=0(leaf).
        // treeA's leaf "1" is where treeB's root "1" attaches.
        var treeA = new BinaryTreeNode<int>(2) { Left = new(1), Right = new(4) };
        var treeB = new BinaryTreeNode<int>(1) { Left = new(0) };

        var merged = CanMerge([treeA, treeB]);

        Assert.NotNull(merged);
        Assert.Equal(2, merged!.Value);
        Assert.Equal(4, merged.Right!.Value);
        Assert.Equal(1, merged.Left!.Value);
        Assert.Equal(0, merged.Left.Left!.Value);
        Assert.Null(merged.Left.Right);
    }

    [Fact]
    public void CanMerge_NoTreesLeafMatchesAnotherRoot_ReturnsNull()
    {
        var treeA = new BinaryTreeNode<int>(10) { Left = new(5) };
        var treeB = new BinaryTreeNode<int>(20) { Left = new(15) };

        Assert.Null(CanMerge([treeA, treeB]));
    }

    [Fact]
    public void CanMerge_SpliceProducesInvalidBstOrder_ReturnsNull()
    {
        // treeA: 2 -> left=1(leaf). treeB: 1 -> right=5(leaf). Splicing treeB into
        // treeA's leaf puts 5 under 2's LEFT subtree, breaking BST order (5 > 2).
        var treeA = new BinaryTreeNode<int>(2) { Left = new(1) };
        var treeB = new BinaryTreeNode<int>(1) { Right = new(5) };

        Assert.Null(CanMerge([treeA, treeB]));
    }

    private static BinaryTreeNode<int>? CanMerge(List<BinaryTreeNode<int>> trees)
    {
        var rootByValue = BuildRootIndex(trees);
        var leafValues = CollectAllLeafValues(trees);
        var overallRoot = FindOverallRoot(trees, leafValues);

        if (overallRoot is null)
        {
            return null;
        }

        rootByValue.TryRemove(overallRoot.Value);
        var merged = Splice(overallRoot, rootByValue);

        return rootByValue.Count == 0 && IsStrictlyAscending(merged) ? merged : null;
    }

    private static HashMap<int, BinaryTreeNode<int>> BuildRootIndex(List<BinaryTreeNode<int>> trees)
    {
        var rootByValue = new HashMap<int, BinaryTreeNode<int>>();
        foreach (var tree in trees)
        {
            rootByValue.Set(tree.Value, tree);
        }

        return rootByValue;
    }

    private static Set<int> CollectAllLeafValues(List<BinaryTreeNode<int>> trees)
    {
        var leafValues = new Set<int>();
        foreach (var tree in trees)
        {
            CollectLeafValues(tree, leafValues);
        }

        return leafValues;
    }

    // The overall merged root is whichever tree's own root value is never
    // anyone's leaf; zero or more than one such candidate means no valid
    // single merge exists.
    private static BinaryTreeNode<int>? FindOverallRoot(List<BinaryTreeNode<int>> trees, Set<int> leafValues)
    {
        BinaryTreeNode<int>? overallRoot = null;
        foreach (var tree in trees)
        {
            if (leafValues.Has(tree.Value))
            {
                continue;
            }

            if (overallRoot is not null)
            {
                return null;
            }

            overallRoot = tree;
        }

        return overallRoot;
    }

    private static void CollectLeafValues(BinaryTreeNode<int> node, Set<int> leafValues)
    {
        if (node.Left is null && node.Right is null)
        {
            leafValues.TryAdd(node.Value);
            return;
        }

        if (node.Left is { } left)
        {
            CollectLeafValues(left, leafValues);
        }

        if (node.Right is { } right)
        {
            CollectLeafValues(right, leafValues);
        }
    }

    private static BinaryTreeNode<int> Splice(BinaryTreeNode<int> node, HashMap<int, BinaryTreeNode<int>> rootByValue)
    {
        if (node.Left is null && node.Right is null && rootByValue.TryGetValue(node.Value, out var mergeRoot))
        {
            rootByValue.TryRemove(node.Value);
            return Splice(mergeRoot, rootByValue);
        }

        if (node.Left is { } left)
        {
            node.Left = Splice(left, rootByValue);
        }

        if (node.Right is { } right)
        {
            node.Right = Splice(right, rootByValue);
        }

        return node;
    }

    private static bool IsStrictlyAscending(BinaryTreeNode<int> root)
    {
        State.Previous.Value = null;
        State.IsAscending.Value = true;
        InOrderTraversal.Walk<int, ValidateAscendingHooks>(root);
        return State.IsAscending.Value;
    }

    private readonly struct ValidateAscendingHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Previous.Value is { } previous && node.Value <= previous)
            {
                State.IsAscending.Value = false;
            }

            State.Previous.Value = node.Value;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<int?> Previous = new();
        public static readonly AsyncLocal<bool> IsAscending = new();
    }
}
