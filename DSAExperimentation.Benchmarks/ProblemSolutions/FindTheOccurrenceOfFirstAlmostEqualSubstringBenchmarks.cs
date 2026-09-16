using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheOccurrenceOfFirstAlmostEqualSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindTheOccurrenceOfFirstAlmostEqualSubstringSolution's, the same methods
// FindTheOccurrenceOfFirstAlmostEqualSubstringTests proves correct. text is
// all 'a', pattern is all 'a' except its final two characters - every window
// mismatches at exactly two positions, both late in the pattern, so the
// brute force has to scan almost the whole pattern before finding its second
// mismatch on every one of the O(n) windows (the same "mismatch at the very
// last character" forcing FindBeautifulIndicesInTheGivenArrayII's benchmark
// uses) and the whole search still ends in -1, the case that gives the
// Z-function arm's O(n + m) no head start from an early return either.
[MemoryDiagnoser]
public class FindTheOccurrenceOfFirstAlmostEqualSubstringBenchmarks
{
    private string _text = "";

    private string _pattern = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var patternLength = Math.Max(2, Length / 10);
        _text = new string('a', Length);
        _pattern = $"{new string('a', patternLength - 2)}bb";
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.IndexOfFirstAlmostEqualSubstringByBruteForce(
            new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.SearchedText(_text),
            new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.MatchPattern(_pattern));

    [Benchmark]
    public int ZFunction() =>
        FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.IndexOfFirstAlmostEqualSubstringByZFunction(
            new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.SearchedText(_text),
            new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.MatchPattern(_pattern));
}
