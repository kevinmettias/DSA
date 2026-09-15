using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestSubstringWithAtLeastKRepeatingCharacters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// LongestSubstringWithAtLeastKRepeatingCharactersSolution's, the same methods
// LongestSubstringWithAtLeastKRepeatingCharactersTests proves correct.
[MemoryDiagnoser]
public class LongestSubstringWithAtLeastKRepeatingCharactersBenchmarks
{
    // LC problem number, reused as the deterministic seed.
    private const int Seed = 395;
    private const int K = 3;

    private string _s = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _s = LongestSubstringWithAtLeastKRepeatingCharactersWorkloads.BuildString(Length, Seed);

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        LongestSubstringWithAtLeastKRepeatingCharactersSolution.LongestByBruteForce(_s, K);

    [Benchmark]
    public int DivideAndConquerHashMap() =>
        LongestSubstringWithAtLeastKRepeatingCharactersSolution.LongestByDivideAndConquer(_s, K);
}
