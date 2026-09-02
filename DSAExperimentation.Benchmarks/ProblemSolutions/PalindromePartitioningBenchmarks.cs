using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PalindromePartitioning;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is PalindromePartitioningSolution's, the same
// method PalindromePartitioningTests proves correct. The original benchmark's two
// [Benchmark] arms (Baseline, PrimitiveComposed) were both compile-smoke
// placeholders (`=> 1`) - one real strategy, not two - so there is only one arm
// here too, mirroring BalancedBinaryTreeBenchmarks' precedent for a
// single-strategy problem.
[MemoryDiagnoser]
public class PalindromePartitioningBenchmarks
{
    private const string Workload = "aab";

    [Benchmark(Baseline = true)]
    public List<List<string>> Backtracking() => PalindromePartitioningSolution.PartitionByBacktracking(Workload);
}
