using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumLengthOfRepeatedSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Length of Repeated Subarray (LC 718): harness only, both arms are
// MaximumLengthOfRepeatedSubarraySolution's, the same methods
// MaximumLengthOfRepeatedSubarrayTests proves correct. Both arrays are identical,
// all-one-value arrays (the same shape as LeetCode's own official all-zeros example)
// so EVERY starting pair walks all the way to the end - brute force's genuine O(n^3)
// worst case, forced deliberately rather than left to chance, the same "force the
// real worst case" intent TwoSumBenchmarks' own setup comment names. A low-repeat
// random array would instead let brute force's early mismatch exit dominate and hide
// the asymptotic gap behind Memoizer's own per-state dictionary/delegate overhead.
[MemoryDiagnoser]
public class MaximumLengthOfRepeatedSubarrayBenchmarks
{
    [Params(60, 300, 1000)]
    public int Length;

    private int[] _first = null!;
    private int[] _second = null!;

    [GlobalSetup]
    public void Setup()
    {
        _first = new int[Length];
        _second = new int[Length];
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumLengthOfRepeatedSubarraySolution.FindLengthByBruteForce(_first, _second);

    [Benchmark]
    public int MemoizedSuffixPairDp() =>
        MaximumLengthOfRepeatedSubarraySolution.FindLengthByMemoizedSuffixPairDp(_first, _second);
}
