using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumWindowSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumWindowSubstringSolution's, the same methods
// MinimumWindowSubstringTests proves correct. Target is deliberately built from
// characters absent from _s (same "force the unreachable worst case" trick
// TwoSumBenchmarks/LongestSubstringWithoutRepeatingCharactersBenchmarks already
// use) so neither strategy ever satisfies "missing == 0" and early-exits -
// BruteForce is forced through every O(n^2) start/end pair instead of breaking out
// after a handful of characters, and SlidingWindowHashMap is forced through its
// full single O(n) pass with the left pointer never advancing.
[MemoryDiagnoser]
public class MinimumWindowSubstringBenchmarks
{
    private const string Target = "XYZ";
    private const int AlphabetSize = 26;

    private string _s = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(0, AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForce() => MinimumWindowSubstringSolution.MinWindowByBruteForce(
        new MinimumWindowSubstringSolution.SearchedText(_s),
        new MinimumWindowSubstringSolution.RequiredCharacters(Target));

    [Benchmark]
    public string SlidingWindowHashMap() => MinimumWindowSubstringSolution.MinWindowBySlidingWindowHashMap(
        new MinimumWindowSubstringSolution.SearchedText(_s),
        new MinimumWindowSubstringSolution.RequiredCharacters(Target));
}
