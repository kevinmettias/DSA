using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ConvertSortedArrayToBinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is
// ConvertSortedArrayToBinarySearchTreeSolution's, the same method
// ConvertSortedArrayToBinarySearchTreeTests proves correct. The original
// benchmark's two [Benchmark] arms called the identical private helper - one
// real strategy, not two - so there is only one arm here too.
//
// Returns object, not BinaryTreeNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class ConvertSortedArrayToBinarySearchTreeBenchmarks
{
    private int[] _values = null!;

    [Params(200, 5_000)]
    public int Length;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? MidpointRecursion() =>
        ConvertSortedArrayToBinarySearchTreeSolution.BuildByMidpointRecursion(_values);
}
