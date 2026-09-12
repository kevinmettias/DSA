using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumBinaryTreeSolution's, the same methods
// MaximumBinaryTreeTests proves correct. Both now return the actual built tree
// (previously the baseline's height-only wrapper measured a weaker question than
// the real answer) - an ascending workload mirrors DiameterOfBinaryTreeBenchmarks'
// degenerate-chain framing, forcing the rescan baseline through its O(n^2)
// worst case against the O(n) monotonic-stack construction.
[MemoryDiagnoser]
public class MaximumBinaryTreeBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? RescanForMax() => MaximumBinaryTreeSolution.ConstructByRescanForMax(_values);

    [Benchmark]
    public object? MonotonicStack() => MaximumBinaryTreeSolution.ConstructByMonotonicStack(_values);
}
