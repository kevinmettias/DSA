using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SearchInABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only. Both arms are SearchInABinarySearchTreeSolution's, the same
// methods SearchInABinarySearchTreeTests proves correct. Both build their own
// input from the same shuffled insertion order so tree height stays close to
// O(log n) instead of the degenerate O(n) ascending-insertion case, the same
// convention DeleteNodeInABSTBenchmarks already uses.
[MemoryDiagnoser]
public class SearchInABinarySearchTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    private int _target;
    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = SeededSequences.ShuffledZeroTo(NodeCount, seed: 1);

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        // presumption: allow -- values always has NodeCount >= 1 entries above, so
        // at least one Insert ran and Root is never null here.
        _root = tree.Root!;
        _target = NodeCount - 1;
    }

    // Projects to the found node's value (BinaryTreeNode<int> itself is
    // internal, and a [Benchmark] method must be public) purely so
    // BenchmarkDotNet has a public return type to consume - the call being
    // measured is still the one-line solution call.
    [Benchmark(Baseline = true)]
    public int? LinearScan() =>
        SearchInABinarySearchTreeSolution.SearchBstByLinearScan(_root, _target)?.Value;

    [Benchmark]
    public int? BinarySearchTreeDescent() =>
        SearchInABinarySearchTreeSolution.SearchBstByBstDescent(_root, _target)?.Value;
}
