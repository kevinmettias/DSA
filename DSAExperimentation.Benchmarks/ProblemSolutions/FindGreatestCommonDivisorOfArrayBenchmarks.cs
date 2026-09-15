using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindGreatestCommonDivisorOfArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindGreatestCommonDivisorOfArraySolution's, the same
// methods FindGreatestCommonDivisorOfArrayTests proves correct. The min/max scan is
// the same O(n) work either way, so what the workload has to expose is the single
// Gcd(min, max) call - _nums[0] is forced to 1 (subtraction's worst case:
// gcd(1, max) forces exactly max-1 single-unit decrements) and MaxValueExclusive is
// pushed well past LC 1979's own 1000-value bound, the same departure
// NumberOfDifferentSubsequencesGCDsBenchmarks/CheckIfItIsAGoodArrayBenchmarks make,
// because at the real bound subtraction's O(max) cost is too small to separate from
// the shared O(n) scan.
[MemoryDiagnoser]
public class FindGreatestCommonDivisorOfArrayBenchmarks
{
    private const int RandomSeed = 1979;
    private const int MaxValueExclusive = 2_000_000;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _nums[0] = 1;
    }

    [Benchmark(Baseline = true)]
    public int SubtractionGcdOfMinAndMax() => FindGreatestCommonDivisorOfArraySolution.FindGcdBySubtraction(_nums);

    [Benchmark]
    public int EuclideanGcdOfMinAndMax() => FindGreatestCommonDivisorOfArraySolution.FindGcdByEuclidean(_nums);
}
