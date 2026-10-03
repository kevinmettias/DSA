using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindSubarrayWithBitwiseORClosestToK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindSubarrayWithBitwiseORClosestToKSolution's, the
// same methods FindSubarrayWithBitwiseORClosestToKSolutionTests proves correct - the
// quadratic every-subarray scan vs. the O(n log(max value)) distinct-OR sweep.
public class FindSubarrayWithBitwiseORClosestToKBenchmarks
{
    private const int K = 1 << 15;

    // LC problem number, used as the deterministic seed for value generation.
    private const int RandomSeed = 3171;

    private int[] _nums = [];

    [Params(80, 500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, 1 << 20, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FindSubarrayWithBitwiseORClosestToKSolution.MinimumDifferenceByBruteForce(_nums, K);

    [Benchmark]
    public int OrCompression() => FindSubarrayWithBitwiseORClosestToKSolution.MinimumDifferenceByOrCompression(_nums, K);
}
