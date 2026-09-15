using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ConstructBinarySearchTreeFromPreorderTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// ConstructBinarySearchTreeFromPreorderTraversalSolution's, the same methods the
// coverage test proves correct - repeated BinarySearchTree<int>.Insert (a fresh
// compare-and-descend walk per value, O(n^2) on this workload) against the
// upper-bound recursion that reads preorder once, O(n). LeetCode's answer is the
// built tree; each arm returns the root's value purely so the built tree cannot be
// optimized away, which costs both arms the same O(1).
[MemoryDiagnoser]
public class ConstructBinarySearchTreeFromPreorderTraversalBenchmarks
{
    private int[] _ascendingPreorder = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Strictly ascending is itself a valid BST preorder (a fully right-skewed
        // tree) - the adversarial input that makes every BinarySearchTree.Insert
        // walk the full height built so far instead of O(log n) on average.
        _ascendingPreorder = Enumerable.Range(0, Length).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedTreeInsert() =>
        ConstructBinarySearchTreeFromPreorderTraversalSolution
            .BstFromPreorderByRepeatedInsert(_ascendingPreorder)?.Value ?? 0;

    [Benchmark]
    public int BoundedRecursion() =>
        ConstructBinarySearchTreeFromPreorderTraversalSolution
            .BstFromPreorderByUpperBoundRecursion(_ascendingPreorder)?.Value ?? 0;
}
