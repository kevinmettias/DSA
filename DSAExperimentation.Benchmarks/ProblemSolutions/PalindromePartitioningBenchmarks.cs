using DSAExperimentation.LeetCode.PalindromePartitioning;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePartitioningSolution's, the same methods
// PalindromePartitioningTests proves correct. The original benchmark's two [Benchmark]
// arms (Baseline, PrimitiveComposed) were both compile-smoke placeholders (`=> 1`) -
// one real strategy, not two - so the pair here is the backtracking walk against the
// precomputed-table walk. The workload is the private constant "aab", LeetCode 131's
// own example, so both arms enumerate its two partitions.
public class PalindromePartitioningBenchmarks
{
    private const string Workload = "aab";

    [Benchmark(Baseline = true)]
    public List<List<string>> Backtracking() => PalindromePartitioningSolution.PartitionByBacktracking(Workload);

    [Benchmark]
    public List<List<string>> PrecomputedPalindromeTable() =>
        PalindromePartitioningSolution.PartitionByPrecomputedPalindromeTable(Workload);
}
