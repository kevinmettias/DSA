using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.WordBreak;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the sole arm is WordBreakSolution's, the same method
// WordBreakTests proves correct. _s tiles a single short dictionary word, so the
// scan has to walk the whole string confirming segmentability rather than
// bailing out early on a dead prefix.
[MemoryDiagnoser]
public class WordBreakBenchmarks
{
    private const string RepeatedWord = "cat";

    private static readonly string[] Dictionary = [RepeatedWord];

    private string _s = "";

    [Params(600, 3000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var repeatedWords = Enumerable.Repeat(RepeatedWord, Length / RepeatedWord.Length);
        _s = string.Concat(repeatedWords);
    }

    [Benchmark]
    public bool TrieMemoizedSuffixSearch() => WordBreakSolution.CanBreakByTrieMemoized(_s, Dictionary);
}
