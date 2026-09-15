using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumRepeatingSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumRepeatingSubstringSolution's, the same methods
// MaximumRepeatingSubstringTests proves correct. sequence is built from Word
// repeated end to end so both strategies are forced through every increasing
// candidate length instead of failing on the very first repeat.
[MemoryDiagnoser]
public class MaximumRepeatingSubstringBenchmarks
{
    private const string Word = "ab";

    private string _sequence = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var repeatedWords = Enumerable.Repeat(Word, Length / Word.Length);
        _sequence = string.Concat(repeatedWords);
    }

    [Benchmark(Baseline = true)]
    public int StringContains() => MaximumRepeatingSubstringSolution.MaxRepeatingByStringContains(
        new MaximumRepeatingSubstringSolution.Haystack(_sequence),
        new MaximumRepeatingSubstringSolution.RepeatedWord(Word));

    [Benchmark]
    public int PrefixFunctionSearchContains() =>
        MaximumRepeatingSubstringSolution.MaxRepeatingByPrefixFunctionSearch(
            new MaximumRepeatingSubstringSolution.Haystack(_sequence),
            new MaximumRepeatingSubstringSolution.RepeatedWord(Word));
}
