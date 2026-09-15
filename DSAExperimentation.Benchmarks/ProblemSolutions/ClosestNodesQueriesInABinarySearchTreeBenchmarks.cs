using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ClosestNodesQueriesInABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClosestNodesQueriesInABinarySearchTreeSolution's, the
// same methods ClosestNodesQueriesInABinarySearchTreeTests proves correct.
// LinearScanPerQuery rescans every node for every query and ignores the BST
// invariant entirely - O(n * queries) - while InOrderBinarySearch spends one
// in-order walk on an ascending buffer and then bisects it per query, O(n + queries
// * log n). The tree is built in [GlobalSetup] with this repo's own
// BinarySearchTree<int>.Insert over a shuffled distinct-value permutation, the same
// random-insertion-order precedent AllElementsInTwoBinarySearchTreesBenchmarks uses,
// so height stays close to O(log n) rather than degenerating on ascending input.
[MemoryDiagnoser]
public class ClosestNodesQueriesInABinarySearchTreeBenchmarks
{
    private const int RandomSeed = 2476; // LeetCode problem number

    private const int MaxQueryOffset = 5;

    private BinaryTreeNode<int>? _root;

    private int[] _queries = [];
    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, NodeCount).ToArray();
        var random = new Random(RandomSeed);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        _root = tree.Root;
        _queries = Enumerable.Range(0, NodeCount)
            .Select(_ => random.Next(-MaxQueryOffset, NodeCount + MaxQueryOffset))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[][] LinearScanPerQuery() =>
        ClosestNodesQueriesInABinarySearchTreeSolution.ClosestNodesByLinearScan(_root, _queries);

    [Benchmark]
    public int[][] InOrderBinarySearch() =>
        ClosestNodesQueriesInABinarySearchTreeSolution.ClosestNodesByInOrderBinarySearch(_root, _queries);
}
