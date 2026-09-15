using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestNiceSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestNiceSubstringSolution's, the same methods
// LongestNiceSubstringTests proves correct. BruteForceAllSubstrings builds a fresh
// 128-entry presence table for each of the O(n^2) substrings and rescans it, O(n^3)
// overall, against this repo's own Set<char> finding the first character missing its
// opposite-case partner and recursing on the two halves - O(n^2) worst case, since no
// nice substring can ever cross that character. A tiny 4-letter mixed-case alphabet
// keeps both variants paying real work instead of exiting on an early mismatch.
[MemoryDiagnoser]
public class LongestNiceSubstringBenchmarks
{
    private const string Alphabet = "aAbB";

    // LC problem number, used as the deterministic seed for text generation.
    private const int RandomSeed = 1763;

    private string _text = "";

    [Params(30, 150)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForceAllSubstrings()
        => LongestNiceSubstringSolution.FindLongestNiceSubstringByBruteForceSubstrings(_text);

    [Benchmark]
    public string DivideAndConquer()
        => LongestNiceSubstringSolution.FindLongestNiceSubstringByDivideAndConquer(_text);
}
