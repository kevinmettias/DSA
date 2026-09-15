using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MergeBSTsToCreateSingleBST;

// LeetCode 1932. Merge BSTs to Create Single BST: repeatedly replace a leaf with
// another tree whose root value equals that leaf, and return the single valid BST
// that results, or null when the forest cannot be merged into one.
//
// This repo's own BinaryTreeNode<TValue> is already a mutable reference type built
// for exactly this kind of splice - its doc comment names "mutate-in-place
// operations" as the reason Left/Right are settable - so a match is spliced by
// reassigning the parent's child pointer, with no new node type.
//
// Both strategies do the same three things and differ only in how they answer
// "which tree has THIS root value": the overall root is whichever tree's root value
// is never anyone's leaf (zero or more than one such candidate means no single
// merge exists); every remaining tree must be consumed; and because splicing alone
// cannot guarantee global ordering - two individually valid subtrees can still
// interleave badly - the merged tree is only accepted when its in-order walk is
// strictly ascending.
internal static class MergeBSTsToCreateSingleBSTSolution
{
    // The textbook answer: rescan the tree list for every leaf. Deliberately plain
    // BCL - a HashSet of leaf values, a List of not-yet-consumed trees, a recursive
    // in-order check - because this is the arm the indexed strategy has to justify
    // itself against. O(trees^2) lookups where the indexed strategy is O(trees).
    public static BinaryTreeNode<int>? CanMergeByLinearScan(IReadOnlyList<BinaryTreeNode<int>> trees)
    {
        ILeafValues leafValues = new LeafValuesInHashSet();

        foreach (var tree in trees)
        {
            CollectLeafValues(tree, leafValues);
        }

        if (FindOverallRoot(trees, leafValues) is not { } overallRoot)
        {
            return null;
        }

        var candidates = RemainingTrees(trees, overallRoot);
        var merged = Splice(overallRoot, new RootByLinearScan(candidates));

        return candidates.TrueForAll(candidate => candidate is null) && IsStrictlyAscending(merged)
            ? merged
            : null;
    }

    // Every tree but the overall root, in a slot that is emptied once the tree has
    // been spliced in - so a root can only ever be consumed once.
    private static List<BinaryTreeNode<int>?> RemainingTrees(
        IReadOnlyList<BinaryTreeNode<int>> trees, BinaryTreeNode<int> overallRoot)
    {
        var candidates = new List<BinaryTreeNode<int>?>(trees.Count);

        foreach (var tree in trees)
        {
            candidates.Add(ReferenceEquals(tree, overallRoot) ? null : tree);
        }

        return candidates;
    }

    // Textbook in-order check: collect the values into a BCL List by plain
    // recursion, then confirm the list ascends.
    private static bool IsStrictlyAscending(BinaryTreeNode<int> root)
    {
        var values = new List<int>();
        CollectInOrder(root, values);

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] <= values[i - 1])
            {
                return false;
            }
        }

        return true;
    }

    // This repo's own primitives: HashMap<int,BinaryTreeNode<int>> indexes every
    // candidate tree by its root value so a leaf resolves in one lookup instead of a
    // rescan (removed once consumed, and an empty index at the end is what proves
    // every tree was used), Set<int> collects the leaf values the overall root is
    // found against, and the ordering check is InOrderTraversal + IInOrderHooks -
    // the same walk AllElementsInTwoBinarySearchTrees already uses rather than a
    // second hand-rolled in-order recursion.
    public static BinaryTreeNode<int>? CanMergeByHashMapIndex(IReadOnlyList<BinaryTreeNode<int>> trees)
    {
        if (FindOverallRootByLeafSet(trees) is not { } overallRoot)
        {
            return null;
        }

        var rootByValue = BuildRootIndex(trees, overallRoot);
        var merged = Splice(overallRoot, new RootByIndex(rootByValue));

        return rootByValue.Count == 0 && IsStrictlyAscendingByInOrderWalk(merged) ? merged : null;
    }

    private static BinaryTreeNode<int>? FindOverallRootByLeafSet(IReadOnlyList<BinaryTreeNode<int>> trees)
    {
        ILeafValues leafValues = new LeafValuesInRepoSet();

        foreach (var tree in trees)
        {
            CollectLeafValues(tree, leafValues);
        }

        return FindOverallRoot(trees, leafValues);
    }

    // Every tree but the overall root, keyed by its root value and removed once
    // consumed - so a root can only ever be spliced in once, and an empty index at
    // the end is what proves every tree was used. The indexed counterpart of the
    // linear scan's RemainingTrees.
    private static HashMap<int, BinaryTreeNode<int>> BuildRootIndex(
        IReadOnlyList<BinaryTreeNode<int>> trees, BinaryTreeNode<int> overallRoot)
    {
        var rootByValue = new HashMap<int, BinaryTreeNode<int>>();

        foreach (var tree in trees)
        {
            if (!ReferenceEquals(tree, overallRoot))
            {
                rootByValue.Set(tree.Value, tree);
            }
        }

        return rootByValue;
    }

    private static bool IsStrictlyAscendingByInOrderWalk(BinaryTreeNode<int> root)
    {
        State.Previous.Value = null;
        State.IsAscending.Value = true;
        InOrderTraversal.Walk<int, ValidateAscendingHooks>(root);

        return State.IsAscending.Value;
    }

    // The plain recursive in-order walk IsStrictlyAscending collects with, kept here
    // rather than beside its caller to satisfy the repo's call-order convention.
    private static void CollectInOrder(BinaryTreeNode<int> node, List<int> values)
    {
        if (node.Left is { } left)
        {
            CollectInOrder(left, values);
        }

        values.Add(node.Value);

        if (node.Right is { } right)
        {
            CollectInOrder(right, values);
        }
    }

    // The overall merged root is whichever tree's own root value is never anyone's
    // leaf; zero or more than one such candidate means no valid single merge exists.
    private static BinaryTreeNode<int>? FindOverallRoot(
        IReadOnlyList<BinaryTreeNode<int>> trees, ILeafValues leafValues)
    {
        BinaryTreeNode<int>? overallRoot = null;

        foreach (var tree in trees)
        {
            if (leafValues.Contains(tree.Value))
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

    private static void CollectLeafValues(BinaryTreeNode<int> node, ILeafValues leafValues)
    {
        if (IsLeaf(node))
        {
            leafValues.Collect(node.Value);
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

    // Shared by both strategies, so the only thing they differ in is how takeRoot
    // finds the tree to splice in.
    private static BinaryTreeNode<int> Splice(BinaryTreeNode<int> node, IRootLookup takeRoot)
    {
        if (IsLeaf(node) && takeRoot.Take(node.Value) is { } mergeRoot)
        {
            return Splice(mergeRoot, takeRoot);
        }

        if (node.Left is { } left)
        {
            node.Left = Splice(left, takeRoot);
        }

        if (node.Right is { } right)
        {
            node.Right = Splice(right, takeRoot);
        }

        return node;
    }

    // A leaf is where another tree's root may be spliced in, and the only place a
    // leaf value is recorded.
    private static bool IsLeaf(BinaryTreeNode<int> node) => node.Left is null && node.Right is null;

    // The record of every leaf value the forest holds, and the only thing the
    // overall-root search reads: a tree is the overall root exactly when its own root
    // value was never collected here. Recording and reading share one type because they
    // share one record, and the two strategies differ only in what
    // backs it - a BCL HashSet<int> for the plain arm, this repo's Set<int> for the
    // composed one - which neither the walk nor the search has to know.
    private interface ILeafValues
    {
        // Records one leaf value. Called once per leaf of every tree, so the same
        // value can arrive more than once.
        void Collect(int value);

        // Whether this value was ever recorded, i.e. whether some tree has it as a
        // leaf.
        bool Contains(int value);
    }

    // The baseline's record: a BCL HashSet<int>.
    private sealed class LeafValuesInHashSet : ILeafValues
    {
        private readonly HashSet<int> _values = [];

        public void Collect(int value) => _values.Add(value);

        public bool Contains(int value) => _values.Contains(value);
    }

    // This repo's own Set<int> over HashMap<int,bool>, the same membership primitive
    // AccountsMerge and Finding3DigitEvenNumbers use for their own dedupe.
    private sealed class LeafValuesInRepoSet : ILeafValues
    {
        private readonly Set<int> _values = new();

        public void Collect(int value) => _values.TryAdd(value);

        public bool Contains(int value) => _values.Has(value);
    }

    // The one question the two splice arms answer differently: which not-yet-consumed
    // tree, if any, is rooted at this value. Taking one consumes it, so no tree can be
    // spliced in twice, and null means the leaf stays a leaf.
    private interface IRootLookup
    {
        BinaryTreeNode<int>? Take(int value);
    }

    // The baseline's answer: rescan the remaining-tree slots linearly.
    private sealed class RootByLinearScan(List<BinaryTreeNode<int>?> candidates) : IRootLookup
    {
        public BinaryTreeNode<int>? Take(int value)
        {
            for (var i = 0; i < candidates.Count; i++)
            {
                if (candidates[i] is { } candidate && candidate.Value == value)
                {
                    candidates[i] = null;
                    return candidate;
                }
            }

            return null;
        }
    }

    // The composed answer: one HashMap<int,BinaryTreeNode<int>> lookup.
    private sealed class RootByIndex(HashMap<int, BinaryTreeNode<int>> rootByValue) : IRootLookup
    {
        public BinaryTreeNode<int>? Take(int value)
        {
            if (!rootByValue.TryGetValue(value, out var tree))
            {
                return null;
            }

            rootByValue.TryRemove(value);

            return tree;
        }
    }

    // Hooks are static, so the running comparison state lives in AsyncLocal
    // alongside the walk - the same arrangement AllElementsInTwoBinarySearchTrees
    // uses for its collected buffer.
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
