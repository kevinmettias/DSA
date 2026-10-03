using DSAExperimentation.LeetCode.PalindromePartitioningII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePartitioningIISolution's, the same methods
// PalindromePartitioningIISolutionTests proves correct. The original benchmark's two
// [Benchmark] arms (Baseline, PrimitiveComposed) were both compile-smoke
// placeholders (`=> 1`) - one real strategy, not two - so this measures the
// memoized recurrence against the bottom-up cut table instead, both over
// LeetCode's own example.
public class PalindromePartitioningIIBenchmarks
{
    private const string Workload = "aab";

    [Benchmark(Baseline = true)]
    public int MemoizedSuffixRecurrence() =>
        PalindromePartitioningIISolution.MinCutByMemoizedSuffixRecurrence(Workload);

    [Benchmark]
    public int IterativeDynamicProgramming() =>
        PalindromePartitioningIISolution.MinCutByIterativeDynamicProgramming(Workload);
}
