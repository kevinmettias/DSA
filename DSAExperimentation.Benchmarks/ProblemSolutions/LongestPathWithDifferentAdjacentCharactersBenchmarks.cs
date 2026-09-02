using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Path With Different Adjacent Characters (LC 2246): a naive baseline that,
// for every node, walks its subtree from scratch twice - once via Height (the
// longest valid-labeled chain starting there) and once via the recursive best-path
// search itself - never reusing either result across levels, O(n) work times n
// nodes, O(n^2) overall on a skewed chain - vs. this repo's own TreeFold closed over
// LongestPathAlgebra, which folds height and best-path-through-node together in one
// bottom-up pass, O(n). Mirrors DiameterOfBinaryTreeBenchmarks' RecomputedHeightPerNode
// vs. TreeMetricsDiameter shape, generalized to LongestPathAlgebra's per-edge label
// check.
[MemoryDiagnoser]
public class LongestPathWithDifferentAdjacentCharactersBenchmarks
{
    // Setup alternates each new node's label between exactly two characters
    // ('a'/'b') so that every parent-child edge in the chain has different
    // adjacent labels.
    private const int LabelAlternationPeriod = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private PathNode _root = null!;

    // A skewed chain with alternating labels, not a bushy random tree - every edge
    // is valid, so this is the naive baseline's worst case (a fresh O(depth) walk
    // repeated at every one of n nodes) and the case where the fold's single O(n)
    // pass wins most decisively, the same reasoning DiameterOfBinaryTreeBenchmarks'
    // Setup comment gives for its own left-skewed chain.
    [GlobalSetup]
    public void Setup()
    {
        _root = new PathNode(0, 'a');
        var current = _root;

        for (var i = 1; i < NodeCount; i++)
        {
            var child = new PathNode(i, i % LabelAlternationPeriod == 0 ? 'a' : 'b');
            current.Children.Add(child);
            current = child;
        }
    }

    [Benchmark(Baseline = true)]
    public int RecomputedChainPerNode() => LongestPathVia(_root).Best;

    // Accumulates the running best-height, second-best-height, and best-path values
    // as LongestPathVia folds over one node's children.
    private readonly record struct PathViaAccumulator(int BestHeight, int SecondBestHeight, int BestPath);

    private static (int Height, int Best) LongestPathVia(PathNode node)
    {
        var accumulator = new PathViaAccumulator(0, 0, 1);

        foreach (var child in node.Children)
        {
            accumulator = AccumulateChild(node, child, accumulator);
        }

        var pathThroughNode = 1 + accumulator.BestHeight + accumulator.SecondBestHeight;

        return (1 + accumulator.BestHeight, Math.Max(pathThroughNode, accumulator.BestPath));
    }

    // Folds a single child into the running accumulator: recurses to get the
    // child's own best path (always contributes), then - only when the edge to
    // this child is valid (labels differ) - folds the child's height into the
    // two tallest heights seen so far.
    private static PathViaAccumulator AccumulateChild(PathNode node, PathNode child, PathViaAccumulator accumulator)
    {
        var (_, childBest) = LongestPathVia(child);
        var bestPath = Math.Max(childBest, accumulator.BestPath);

        if (child.Label == node.Label)
        {
            return accumulator with { BestPath = bestPath };
        }

        var (bestHeight, secondBestHeight) = FoldHeight(
            Height(child), accumulator.BestHeight, accumulator.SecondBestHeight);

        return new PathViaAccumulator(bestHeight, secondBestHeight, bestPath);
    }

    // Folds one more height into the running two tallest heights seen so far.
    private static (int BestHeight, int SecondBestHeight) FoldHeight(int childHeight, int bestHeight, int secondBestHeight)
    {
        if (childHeight > bestHeight)
        {
            return (childHeight, bestHeight);
        }

        return (bestHeight, Math.Max(childHeight, secondBestHeight));
    }

    // Recomputed from scratch every time a caller needs it, never memoized across
    // the many nodes that call it on overlapping subtrees - the source of the O(n^2)
    // blowup on a skewed chain, mirroring DiameterOfBinaryTreeBenchmarks' own
    // separate Height() helper.
    private static int Height(PathNode node)
    {
        var best = 0;

        foreach (var child in node.Children)
        {
            if (child.Label == node.Label)
            {
                continue;
            }

            var height = Height(child);

            if (height > best)
            {
                best = height;
            }
        }

        return 1 + best;
    }

    [Benchmark]
    public int TreeFoldLongestPath()
        => TreeFold.Fold<
            PathNode, PathTopology, ListChildren<PathNode>,
            NaturalChildOrder<PathNode, ListChildren<PathNode>>, ListChildren<PathNode>,
            LongestPathAlgebra, PathState>(_root).LongestPath;
}
