using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindGreatestCommonDivisorOfArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindGreatestCommonDivisorOfArraySolution's, the same
// methods FindGreatestCommonDivisorOfArraySolutionTests proves correct. The min/max scan is
// the same O(n) work either way, so what the workload has to expose is the single
// Gcd(min, max) call - _nums[0] is forced to 1 (subtraction's worst case:
// gcd(1, max) forces exactly max-1 single-unit decrements). LC 1979 caps the array
// at 1000 values of at most 1000, so the larger Length is that cap and those
// decrements stop near 999, too few to separate subtraction's O(max) cost from the
// shared O(n) scan; the gap is measured where LeetCode poses it anyway.
public class FindGreatestCommonDivisorOfArrayBenchmarks
{
    private const int RandomSeed = 1979;
    // One past LC 1979's largest value, 1000.
    private const int NumberUpperBoundExclusive = 1_001;

    private int[] _nums = [];

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, NumberUpperBoundExclusive, random);
        _nums[0] = 1;
    }

    [Benchmark(Baseline = true)]
    public int SubtractionGcdOfMinAndMax() => FindGreatestCommonDivisorOfArraySolution.FindGcdBySubtraction(_nums);

    [Benchmark]
    public int EuclideanGcdOfMinAndMax() => FindGreatestCommonDivisorOfArraySolution.FindGcdByEuclidean(_nums);
}
