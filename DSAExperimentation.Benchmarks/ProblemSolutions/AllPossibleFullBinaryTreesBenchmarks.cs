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
    [Params(13, 19)]
    public int Nodes;

    [Benchmark(Baseline = true)]
    public int Naive() => Naive(Nodes).Count;

    [Benchmark]
    public int Memoized() => Memoizer.Memoize<int, List<BinaryTreeNode<int>?>>(Nodes, Recurrence).Count;

    private static List<BinaryTreeNode<int>?> Naive(int n)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (n % 2 == 0)
        {
            return trees;
        }

        if (n == 1)
        {
            trees.Add(new BinaryTreeNode<int>(0));
            return trees;
        }

        for (var leftCount = 1; leftCount < n; leftCount += 2)
        {
            var lefts = Naive(leftCount);
            var rights = Naive(n - 1 - leftCount);

            foreach (var left in lefts)
            {
                foreach (var right in rights)
                {
                    trees.Add(new BinaryTreeNode<int>(0) { Left = left, Right = right });
                }
            }
        }

        return trees;
    }

    private static List<BinaryTreeNode<int>?> Recurrence(int n, Func<int, List<BinaryTreeNode<int>?>> generate)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (n % 2 == 0)
        {
            return trees;
        }

        if (n == 1)
        {
            trees.Add(new BinaryTreeNode<int>(0));
            return trees;
        }

        for (var leftCount = 1; leftCount < n; leftCount += 2)
        {
            var lefts = generate(leftCount);
            var rights = generate(n - 1 - leftCount);

            foreach (var left in lefts)
            {
                foreach (var right in rights)
                {
                    trees.Add(new BinaryTreeNode<int>(0) { Left = left, Right = right });
                }
            }
        }

        return trees;
    }
}
