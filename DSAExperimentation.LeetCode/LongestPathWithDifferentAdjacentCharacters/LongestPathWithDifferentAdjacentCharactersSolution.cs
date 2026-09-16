using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.LongestPathWithDifferentAdjacentCharacters;

// LeetCode 2246. Longest Path With Different Adjacent Characters: parent[] describes
// a tree rooted at node 0 (the same prevRoom[] shape LC 1916 uses, so
// DataStructures' ParentArrayTree/RootedTreeNode materialize it), labels[i] is node
// i's character, and the answer is the longest node-count path whose every adjacent
// pair has a different character.
//
// That is exactly TreeMetrics.Diameter's own multi-child "two tallest heights"
// composition, closed over LongestPathAlgebra's per-edge label check instead of
// DiameterAlgebra's unconditional one - so the two strategies differ only in
// whether each node's height is reused from the fold or walked again from scratch.
internal static class LongestPathWithDifferentAdjacentCharactersSolution
{
    // The textbook answer: for every node, walk its subtree from scratch twice -
    // once via Height (the longest validly-labeled chain starting there) and once
    // via the recursive best-path search itself - never reusing either result
    // across levels. O(n) work at each of n nodes, O(n^2) overall on a skewed
    // chain. Deliberately plain recursion over BCL lists: it is the arm the fold
    // below has to justify itself against.
    public static int LongestPathByRecomputedSubtreeWalk(int[] parent, string labels)
        => LongestPathByRecomputedSubtreeWalk(ParentArrayTree.Build(parent)[0], labels);

    public static int LongestPathByRecomputedSubtreeWalk(RootedTreeNode root, string labels)
        => LongestPathVia(root, labels).Best;

    // This repo's own bottom-up pass: TreeFold visits every node once and
    // LongestPathAlgebra folds height and best-path-through-node together, so no
    // subtree is ever walked twice. O(n).
    public static int LongestPathByTreeFold(int[] parent, string labels)
        => LongestPathByTreeFold(ParentArrayTree.Build(parent)[0], labels);

    public static int LongestPathByTreeFold(RootedTreeNode root, string labels)
    {
        LongestPathAlgebra.Prepare(labels);

        return TreeFold.Fold<
            RootedTreeNode, RootedTreeTopology, ListChildren<RootedTreeNode>,
            NaturalChildOrder<RootedTreeNode, ListChildren<RootedTreeNode>>, ListChildren<RootedTreeNode>,
            LongestPathAlgebra, PathState>(root).LongestPath;
    }

    // Accumulates the running best-height, second-best-height, and best-path values
    // as LongestPathVia folds over one node's children.
    private readonly record struct PathViaAccumulator(int BestHeight, int SecondBestHeight, int BestPath);

    private static (int Height, int Best) LongestPathVia(RootedTreeNode node, string labels)
    {
        var accumulator = new PathViaAccumulator(0, 0, 1);

        foreach (var child in node.Children)
        {
            accumulator = AccumulateChild(node, child, labels, accumulator);
        }

        var pathThroughNode = 1 + accumulator.BestHeight + accumulator.SecondBestHeight;

        return (1 + accumulator.BestHeight, Math.Max(pathThroughNode, accumulator.BestPath));
    }

    // Folds a single child into the running accumulator: recurses to get the
    // child's own best path (always contributes), then - only when the edge to
    // this child is valid (labels differ) - folds the child's height into the
    // two tallest heights seen so far.
    private static PathViaAccumulator AccumulateChild(
        RootedTreeNode node, RootedTreeNode child, string labels, PathViaAccumulator accumulator)
    {
        var (_, childBest) = LongestPathVia(child, labels);
        var bestPath = Math.Max(childBest, accumulator.BestPath);

        if (labels[child.Id] == labels[node.Id])
        {
            return accumulator with { BestPath = bestPath };
        }

        var childHeight = Height(child, labels);
        var (bestHeight, secondBestHeight) = FoldHeight(
            childHeight, accumulator.BestHeight, accumulator.SecondBestHeight);

        return new PathViaAccumulator(bestHeight, secondBestHeight, bestPath);
    }

    // Folds one more height into the running two tallest heights seen so far.
    private static (int BestHeight, int SecondBestHeight) FoldHeight(
        int childHeight, int bestHeight, int secondBestHeight)
    {
        if (childHeight > bestHeight)
        {
            return (childHeight, bestHeight);
        }

        return (bestHeight, Math.Max(childHeight, secondBestHeight));
    }

    // Recomputed from scratch every time a caller needs it, never memoized across
    // the many nodes that call it on overlapping subtrees - the source of the
    // O(n^2) blowup on a skewed chain.
    private static int Height(RootedTreeNode node, string labels)
    {
        var best = 0;

        foreach (var child in node.Children)
        {
            if (labels[child.Id] == labels[node.Id])
            {
                continue;
            }

            var height = Height(child, labels);

            if (height > best)
            {
                best = height;
            }
        }

        return 1 + best;
    }
}
