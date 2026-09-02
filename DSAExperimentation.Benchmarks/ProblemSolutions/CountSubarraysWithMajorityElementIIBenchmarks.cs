using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubarraysWithMajorityElementIISolution's, the
// same methods CountSubarraysWithMajorityElementIITests proves correct.
//
// Length stays well below LC's own 1e5 upper bound - the O(n^2) brute force
// enumerates every subarray directly and would not finish otherwise; the Fenwick
// prefix-sum sweep is O(n log n) regardless. A small 5-value alphabet keeps
// target frequent enough that a meaningful share of subarrays qualify.
[MemoryDiagnoser]
public class CountSubarraysWithMajorityElementIIBenchmarks
{
    private const int RandomSeed = 3739; // LC problem number
    private const int AlphabetSize = 5;
    private const int Target = 1;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, AlphabetSize + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountSubarraysWithMajorityElementIISolution.CountByBruteForce(_nums, Target);

    [Benchmark]
    public long FenwickPrefixSum() => CountSubarraysWithMajorityElementIISolution.CountByFenwickPrefixSum(_nums, Target);
}
