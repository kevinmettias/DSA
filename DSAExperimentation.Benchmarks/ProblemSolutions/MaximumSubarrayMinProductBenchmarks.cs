using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSubarrayMinProduct;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSubarrayMinProductSolution's, the same methods
// MaximumSubarrayMinProductTests proves correct, and both take LeetCode's own nums
// array, so the workload is generated once in [GlobalSetup] rather than inside either
// measured call.
//
// _arr is a random permutation of 1..Length, so the baseline's inner loop always runs
// its full remaining length while the sweep still pays one push and at most one pop
// per element on each of its two passes.
[MemoryDiagnoser]
public class MaximumSubarrayMinProductBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1856;

    private int[] _arr = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumSubarrayMinProductSolution.MaxSumMinProductByBruteForce(_arr);

    [Benchmark]
    public int MonotonicStackSweep() =>
        MaximumSubarrayMinProductSolution.MaxSumMinProductByMonotonicStack(_arr);
}
