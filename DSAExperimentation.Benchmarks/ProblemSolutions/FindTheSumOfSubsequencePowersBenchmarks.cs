using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheSumOfSubsequencePowers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheSumOfSubsequencePowersSolution's, the same
// methods FindTheSumOfSubsequencePowersTests proves correct. k is fixed well below
// Length so both the 2^n brute force and the O(n^4 k) threshold-counting DP stay
// inside a reasonable wall-clock budget at these sizes.
[MemoryDiagnoser]
public class FindTheSumOfSubsequencePowersBenchmarks
{
    private const int MaxValueExclusive = 1_000_000;
    private const int Seed = 3098;
    private const int K = 4;

    private int[] _nums = [];

    [Params(10, 16)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FindTheSumOfSubsequencePowersSolution.SumOfPowersByBruteForce(_nums, K);

    [Benchmark]
    public int ThresholdCounting() => FindTheSumOfSubsequencePowersSolution.SumOfPowersByThresholdCounting(_nums, K);
}
