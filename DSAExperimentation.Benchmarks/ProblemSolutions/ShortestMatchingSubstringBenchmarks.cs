using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestMatchingSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestMatchingSubstringSolution's, the same methods
// ShortestMatchingSubstringTests proves correct. The brute-force arm still gets the
// split MatchPattern (parsing p is trivial but still input construction, not part
// of the search); the KMP arm gets the fully-built PatternOccurrences so the three
// occurrence scans are charged to [GlobalSetup], leaving only the greedy
// binary-search combination to measure.
[MemoryDiagnoser]
public class ShortestMatchingSubstringBenchmarks
{
    // LC problem number, reused as the deterministic text seed.
    private const int TextSeed = 3455;
    private const string Pattern = "ab*cd*ef";

    [Params(500, 5000)]
    public int TextLength;

    private string _text = null!;
    private MatchPattern _pattern;
    private PatternOccurrences _occurrences;

    [GlobalSetup]
    public void Setup()
    {
        _text = ShortestMatchingSubstringWorkloads.BuildText(TextLength, seed: TextSeed);
        _pattern = MatchPattern.Parse(Pattern);
        _occurrences = PatternOccurrences.Build(_text, _pattern);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceIndexOf() =>
        ShortestMatchingSubstringSolution.ShortestLengthByBruteForceIndexOf(_text, _pattern);

    [Benchmark]
    public int KmpBinarySearch() =>
        ShortestMatchingSubstringSolution.ShortestLengthByKmpBinarySearch(_occurrences);
}
