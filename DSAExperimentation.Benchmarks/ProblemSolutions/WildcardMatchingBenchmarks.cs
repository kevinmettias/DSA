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

    private string _text = null!;
    private string _pattern = null!;

    [Params(20, 80)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        _text = new string('a', Length) + TextSuffix;
        _pattern = PatternValue;
    }

    [Benchmark(Baseline = true)]
    public bool GreedyTwoPointer() => WildcardMatchingSolution.IsMatchByGreedyTwoPointer(_text, _pattern);

    [Benchmark]
    public bool MemoizedDp() => WildcardMatchingSolution.IsMatchByMemoizedDp(_text, _pattern);
}
