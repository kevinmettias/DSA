using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfSubarrayMinimums;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfSubarrayMinimumsSolution's, the same methods
// SumOfSubarrayMinimumsTests proves correct. _arr is a random permutation so the
// brute-force arm's inner loop always runs its full remaining length - with every
// value distinct there is no run of equal minimums to let it settle early.
[MemoryDiagnoser]
public class SumOfSubarrayMinimumsBenchmarks
{
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 907;

    [Params(200, 5_000)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => SumOfSubarrayMinimumsSolution.SumSubarrayMinsByBruteForce(_arr);

    [Benchmark]
    public int MonotonicStackContribution() =>
        SumOfSubarrayMinimumsSolution.SumSubarrayMinsByMonotonicStack(_arr);
}
