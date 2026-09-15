using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountKSubsequencesOfAStringWithMaximumBeauty;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CountKSubsequencesOfAStringWithMaximumBeautySolution's, the same methods
// CountKSubsequencesOfAStringWithMaximumBeautyTests proves correct. Only 5 of the
// 26 letters are used so there are real ties at the group boundary (the case
// CountByGroupedFrequencyProduct's C(groupSize, remaining) branch exists for) and
// so CountByBruteForceCombinations has a non-trivial C(5, k) search space to walk
// instead of returning almost immediately.
[MemoryDiagnoser]
public class CountKSubsequencesOfAStringWithMaximumBeautyBenchmarks
{
    private const int AlphabetPoolSize = 5;
    private const int Seed = 2842;

    private string _s = string.Empty;

    private int _k;
    [Params(5_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetPoolSize));
        }

        _s = new string(chars);
        _k = AlphabetPoolSize - 1;
    }

    [Benchmark(Baseline = true)]
    public long BruteForceCombinations() => CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByBruteForceCombinations(_s, _k);

    [Benchmark]
    public long GroupedFrequencyProduct() => CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByGroupedFrequencyProduct(_s, _k);
}
