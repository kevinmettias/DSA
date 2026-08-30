using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Unique Binary Search Trees II (LC 95): plain recursive left-root-right
// generation, which re-derives the same (start,end) sub-range from multiple
// parent calls (e.g. for n=4, GenerateTrees(3,4) is reached both directly, as
// root=2's right subtree, and again nested inside root=1's right subtree
// GenerateTrees(2,4)) - call count grows as 3^n - vs. the identical recurrence
// wrapped in this repo's own Memoizer, caching each distinct (start,end) range so
// it is only ever generated once, sharing the resulting subtree objects across
// every parent that needs that same range. Mirrors UniqueBinarySearchTreesBenchmarks'
// tabulation-vs-Memoizer pairing for LC 96, the counting-only sibling of this
// problem.
[MemoryDiagnoser]
public class UniqueBinarySearchTreesIIBenchmarks
{
    [Params(8, 12)]
    public int Nodes;

    [Benchmark(Baseline = true)]
    public int Naive() => Naive(1, Nodes).Count;

    [Benchmark]
    public int Memoized() => Memoizer.Memoize<(int Start, int End), List<BinaryTreeNode<int>?>>(
        (1, Nodes), Recurrence).Count;

    private static List<BinaryTreeNode<int>?> Naive(int start, int end)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (start > end)
        {
            trees.Add(null);
            return trees;
        }

        for (var root = start; root <= end; root++)
        {
            var lefts = Naive(start, root - 1);
            var rights = Naive(root + 1, end);

            foreach (var left in lefts)
            {
                foreach (var right in rights)
                {
                    trees.Add(new BinaryTreeNode<int>(root) { Left = left, Right = right });
                }
            }
        }

        return trees;
    }

    private static List<BinaryTreeNode<int>?> Recurrence(
        (int Start, int End) range,
        Func<(int Start, int End), List<BinaryTreeNode<int>?>> generate)
    {
        var trees = new List<BinaryTreeNode<int>?>();

        if (range.Start > range.End)
        {
            trees.Add(null);
            return trees;
        }

        for (var root = range.Start; root <= range.End; root++)
        {
            var lefts = generate((range.Start, root - 1));
            var rights = generate((root + 1, range.End));

            foreach (var left in lefts)
            {
                foreach (var right in rights)
                {
                    trees.Add(new BinaryTreeNode<int>(root) { Left = left, Right = right });
                }
            }
        }

        return trees;
    }
}
