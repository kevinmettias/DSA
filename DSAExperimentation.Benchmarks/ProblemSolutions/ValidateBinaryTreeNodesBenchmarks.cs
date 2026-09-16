using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidateBinaryTreeNodes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidateBinaryTreeNodesSolution's, the same methods
// ValidateBinaryTreeNodesTests proves correct - the textbook scan that tries every
// node as a candidate root and re-traverses from scratch (O(n) per candidate, O(n^2)
// overall) against this repo's DisjointSet-based single O(n * alpha(n)) pass, the
// same "naive re-validate from scratch vs. one DisjointSet pass" shape
// RedundantConnectionIIBenchmarks uses for LC 685.
[MemoryDiagnoser]
public class ValidateBinaryTreeNodesBenchmarks
{
    private int[] _leftChild = [];

    private int[] _rightChild = [];
    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // A left-leaning chain n-1 -> n-2 -> ... -> 0, rooted at the LAST index -
        // every candidate root the naive scan tries before reaching it still walks a
        // real (shrinking) prefix of the chain instead of failing in O(1), which is
        // what forces the naive strategy through its full O(n^2), not just O(n).
        var leftChild = new int[NodeCount];
        var rightChild = new int[NodeCount];
        Array.Fill(leftChild, -1);
        Array.Fill(rightChild, -1);

        for (var i = 1; i < NodeCount; i++)
        {
            leftChild[i] = i - 1;
        }

        _leftChild = leftChild;
        _rightChild = rightChild;
    }

    [Benchmark(Baseline = true)]
    public bool IsValidByRootScan() =>
        ValidateBinaryTreeNodesSolution.IsValidByRootScan(NodeCount, _leftChild, _rightChild);

    [Benchmark]
    public bool IsValidByDisjointSet() =>
        ValidateBinaryTreeNodesSolution.IsValidByDisjointSet(NodeCount, _leftChild, _rightChild);
}
