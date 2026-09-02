using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindSubarrayWithBitwiseORClosestToK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindSubarrayWithBitwiseORClosestToKSolution's, the
// same methods FindSubarrayWithBitwiseORClosestToKTests proves correct - the
// quadratic every-subarray scan vs. the O(n log(max value)) distinct-OR sweep.
[MemoryDiagnoser]
public class FindSubarrayWithBitwiseORClosestToKBenchmarks
{
    private const int K = 1 << 15;

    // LC problem number, used as the deterministic seed for value generation.
    private const int RandomSeed = 3171;

    private int[] _nums = null!;

    [Params(80, 500)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1 << 20)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FindSubarrayWithBitwiseORClosestToKSolution.MinimumDifferenceByBruteForce(_nums, K);

    [Benchmark]
    public int OrCompression() => FindSubarrayWithBitwiseORClosestToKSolution.MinimumDifferenceByOrCompression(_nums, K);
}
