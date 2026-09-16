using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.WildcardMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WildcardMatchingSolution's, the same methods
// WildcardMatchingTests proves correct.
[MemoryDiagnoser]
public class WildcardMatchingBenchmarks
{
    private const string TextSuffix = "b";
    private const string PatternValue = "*a*b";

    private string _text = "";
    private string _pattern = "";

    [Params(20, 80)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _text = new string('a', Length) + TextSuffix;
        _pattern = PatternValue;
    }

    [Benchmark(Baseline = true)]
    public bool IsMatchByGreedyTwoPointer() => WildcardMatchingSolution.IsMatchByGreedyTwoPointer(
        new WildcardMatchingSolution.MatchedText(_text),
        new WildcardMatchingSolution.WildcardPattern(_pattern));

    [Benchmark]
    public bool IsMatchByMemoizedDp() => WildcardMatchingSolution.IsMatchByMemoizedDp(
        new WildcardMatchingSolution.MatchedText(_text),
        new WildcardMatchingSolution.WildcardPattern(_pattern));
}
