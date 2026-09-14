using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfDifferentSubsequencesGCDs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfDifferentSubsequencesGCDsSolution's, the same
// methods NumberOfDifferentSubsequencesGCDsTests proves correct. Rescanning the
// whole array for every candidate gcd (O(max * n)) vs. walking only each candidate's
// multiples through this repo's own Set<int> for O(1) presence checks
// (O(max log max), harmonic). MaxValueExclusive is held fixed across both [Params]
// sizes so the array length varies independently of the value domain - brute force
// scales with both, the Set-based walk only with the value domain.
[MemoryDiagnoser]
public class NumberOfDifferentSubsequencesGCDsBenchmarks
{
    private const int RandomSeed = 1;
    private const int MinValue = 1;
    private const int MaxValueExclusive = 5_000;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(MinValue, MaxValueExclusive)).Distinct().ToArray();
    }

    [Benchmark(Baseline = true)]
    public int CountDifferentSubsequenceGcdsByWholeArrayScan() =>
        NumberOfDifferentSubsequencesGCDsSolution.CountDifferentSubsequenceGcdsByWholeArrayScan(_nums);

    [Benchmark]
    public int CountDifferentSubsequenceGcdsBySetMultiples() =>
        NumberOfDifferentSubsequencesGCDsSolution.CountDifferentSubsequenceGcdsBySetMultiples(_nums);
}
