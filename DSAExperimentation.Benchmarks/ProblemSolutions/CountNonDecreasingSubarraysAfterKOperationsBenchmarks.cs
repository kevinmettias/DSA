using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountNonDecreasingSubarraysAfterKOperations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNonDecreasingSubarraysAfterKOperationsSolution's,
// the same methods CountNonDecreasingSubarraysAfterKOperationsTests proves correct.
// Neither strategy needs anything prepared beyond the array itself, so
// [GlobalSetup] only charges workload construction.
[MemoryDiagnoser]
public class CountNonDecreasingSubarraysAfterKOperationsBenchmarks
{
    private const int Seed = 3420;
    private const int Budget = 300;

    [Params(200, 1000)]
    public int Size;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = NonDecreasingSubarrayWorkloads.BuildNums(Size, Seed);

    [Benchmark(Baseline = true)]
    public long PrefixMaxBruteForce() =>
        CountNonDecreasingSubarraysAfterKOperationsSolution.CountByPrefixMaxBruteForce(_nums, Budget);

    [Benchmark]
    public long MonotonicDequeWindow() =>
        CountNonDecreasingSubarraysAfterKOperationsSolution.CountByMonotonicDequeWindow(_nums, Budget);
}
