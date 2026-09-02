using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PalindromePartitioningII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is PalindromePartitioningIISolution's, the same
// method PalindromePartitioningIITests proves correct. The original benchmark's
// two [Benchmark] arms (Baseline, PrimitiveComposed) were both compile-smoke
// placeholders (`=> 1`) - one real strategy, not two - so this measures the
// actual memoized recurrence against LeetCode's own example instead.
[MemoryDiagnoser]
public class PalindromePartitioningIIBenchmarks
{
    private const string Workload = "aab";

    [Benchmark(Baseline = true)]
    public int MemoizedSuffixRecurrence() =>
        PalindromePartitioningIISolution.MinCutByMemoizedSuffixRecurrence(Workload);
}
