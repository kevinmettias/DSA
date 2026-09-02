using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// All Possible Full Binary Trees (LC 894): plain recursive split-based generation,
// which re-derives the same node-count subproblem from multiple parent calls (e.g.
// AllPossibleFbt(5) is reached both directly and again nested inside larger splits)
// vs. the identical recurrence wrapped in this repo's own Memoizer, caching each
// distinct node count so its full binary trees are generated once and shared across
// every parent split that needs that same count - the same naive-vs-Memoizer
// pairing UniqueBinarySearchTreesIIBenchmarks already uses for LC 95, keyed here by
// a single node count instead of a (start,end) range since a full binary tree's
// shape never depends on the values it carries (every node is 0).
[MemoryDiagnoser]
public class AllPossibleFullBinaryTreesBenchmarks
{
    // A full binary tree only exists for odd node counts, so evenness is checked
    // with this divisor and left-subtree sizes are stepped by it to stay odd.
    private const int NodeCountParityDivisor = 2;

    [Params(13, 19)]
    public int Nodes;

    [Benchmark(Baseline = true)]
    public int Naive() => Naive(Nodes).Count;

    [Benchmark]
    public int Memoized() => Memoizer.Memoize<int, List<BinaryTreeNode<int>?>>(Nodes, Recurrence).Count;

    private static List<BinaryTreeNode<int>?> Naive(int n) => BuildFullBinaryTrees(n, Naive);

    private static List<BinaryTreeNode<int>?> Recurrence(int n, Func<int, List<BinaryTreeNode<int>?>> generate) =>
        BuildFullBinaryTrees(n, generate);

    // Shared recurrence body for both Naive and Recurrence: they differ only in how
    // sub-counts are generated (plain self-recursion vs. a memoized delegate), so
    // that single point of variation is passed in as `generate`.
    private static List<BinaryTreeNode<int>?> BuildFullBinaryTrees(
        int n,
        Func<int, List<BinaryTreeNode<int>?>> generate)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (n % NodeCountParityDivisor == 0)
        {
            return trees;
        }

        if (n == 1)
        {
            trees.Add(new BinaryTreeNode<int>(0));
            return trees;
        }

        for (var leftCount = 1; leftCount < n; leftCount += NodeCountParityDivisor)
        {
            AppendSplits(trees, generate(leftCount), generate(n - 1 - leftCount));
        }

        return trees;
    }

    private static void AppendSplits(
        List<BinaryTreeNode<int>?> trees,
        List<BinaryTreeNode<int>?> lefts,
        List<BinaryTreeNode<int>?> rights)
    {
        foreach (var left in lefts)
        {
            foreach (var right in rights)
            {
                trees.Add(new BinaryTreeNode<int>(0) { Left = left, Right = right });
            }
        }
    }
}
