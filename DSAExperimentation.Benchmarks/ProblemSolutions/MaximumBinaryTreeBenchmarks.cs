using DSAExperimentation.LeetCode.MaximumBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumBinaryTreeSolution's, the same methods
// MaximumBinaryTreeSolutionTests proves correct. Both now return the actual built tree
// (previously the baseline's height-only wrapper measured a weaker question than
// the real answer) - an ascending workload mirrors DiameterOfBinaryTreeBenchmarks'
// degenerate-chain framing, forcing the rescan baseline through its O(n^2)
// worst case against the O(n) monotonic-stack construction. Length stops at LC 654's
// 1,000-element cap.
public class MaximumBinaryTreeBenchmarks
{
    private int[] _values = [];

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? RescanForMax() => MaximumBinaryTreeSolution.ConstructByRescanForMax(_values);

    [Benchmark]
    public object? MonotonicStack() => MaximumBinaryTreeSolution.ConstructByMonotonicStack(_values);
}
