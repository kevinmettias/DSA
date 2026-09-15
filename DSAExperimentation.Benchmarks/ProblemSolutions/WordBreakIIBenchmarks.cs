using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.WordBreakII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WordBreakIISolution's, the same methods
// WordBreakIITests proves correct. _s tiles a single short dictionary word so
// both strategies reach the identical unique sentence, isolating the
// segmentation-scan cost itself rather than sentence-construction cost.
[MemoryDiagnoser]
public class WordBreakIIBenchmarks
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

    [Benchmark(Baseline = true)]
    public int HashSetUnboundedScan() => WordBreakIISolution.SentencesByHashSetScan(_s, Dictionary).Count;

    [Benchmark]
    public int TriePrunedMemoized() => WordBreakIISolution.SentencesByTrieMemoized(_s, Dictionary).Count;
}
