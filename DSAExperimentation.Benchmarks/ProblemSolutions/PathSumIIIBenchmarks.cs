using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Path Sum III (LC 437): the textbook O(n^2) double-DFS (for every node, walk
// its entire downward subtree summing paths from there) vs. a single DFS
// carrying a running root-to-node sum plus this repo's own HashMap<long,int>
// counting how many ancestor prefix sums equal (runningSum - target) - the same
// running-sum-HashMap counting trick TwoSumBenchmarks proves for a flat array,
// here threaded down a tree with an explicit backtrack (undo the current node's
// own prefix-sum entry once both children are done) so a sibling subtree never
// sees it. _root is BinaryTrees.Balanced, whose node values are all
// non-negative, so Target is deliberately unreachable - both strategies are
// forced through their full traversal instead of an early match.
[MemoryDiagnoser]
public class PathSumIIIBenchmarks
{
    private const int Target = -1;

    [Params(200, 5_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int DoubleDfs() => CountFromEveryNode(_root);

    private static int CountFromEveryNode(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return 0;
        }

        return CountDownwardFrom(node, 0) + CountFromEveryNode(node.Left) + CountFromEveryNode(node.Right);
    }

    private static int CountDownwardFrom(BinaryTreeNode<int>? node, long runningSum)
    {
        if (node is null)
        {
            return 0;
        }

        runningSum += node.Value;
        var matches = runningSum == Target ? 1 : 0;

        return matches + CountDownwardFrom(node.Left, runningSum) + CountDownwardFrom(node.Right, runningSum);
    }

    [Benchmark]
    public int PrefixSumHashMap()
    {
        var prefixSumCounts = new HashMap<long, int>();
        prefixSumCounts.Set(0, 1);

        return CountPaths(_root, 0, prefixSumCounts);
    }

    private static int CountPaths(BinaryTreeNode<int>? node, long runningSum, HashMap<long, int> prefixSumCounts)
    {
        if (node is null)
        {
            return 0;
        }

        runningSum += node.Value;
        var matches = CountMatchesAtNode(runningSum, prefixSumCounts);
        RecordPrefixSum(runningSum, prefixSumCounts);

        matches += CountPaths(node.Left, runningSum, prefixSumCounts);
        matches += CountPaths(node.Right, runningSum, prefixSumCounts);

        UnwindPrefixSum(runningSum, prefixSumCounts);

        return matches;
    }

    private static int CountMatchesAtNode(long runningSum, HashMap<long, int> prefixSumCounts)
        => prefixSumCounts.TryGetValue(runningSum - Target, out var count) ? count : 0;

    private static void RecordPrefixSum(long runningSum, HashMap<long, int> prefixSumCounts)
        => prefixSumCounts.Set(runningSum, (prefixSumCounts.TryGetValue(runningSum, out var existing) ? existing : 0) + 1);

    private static void UnwindPrefixSum(long runningSum, HashMap<long, int> prefixSumCounts)
    {
        var afterChildren = prefixSumCounts.TryGetValue(runningSum, out var updated) ? updated : 0;

        if (afterChildren <= 1)
        {
            prefixSumCounts.TryRemove(runningSum);
        }
        else
        {
            prefixSumCounts.Set(runningSum, afterChildren - 1);
        }
    }
}
