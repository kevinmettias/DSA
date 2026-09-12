using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DecodeWaysII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DecodeWaysIISolution's, the same methods
// DecodeWaysIITests proves correct.
[MemoryDiagnoser]
public class DecodeWaysIIBenchmarks
{
    private const string WildcardPair = "2*";
    private const int PairLength = 2;

    [Params(20, 200)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup()
    {
        var pairs = Enumerable.Repeat(WildcardPair, Length / PairLength);
        _value = string.Concat(pairs);
    }

    [Benchmark(Baseline = true)]
    public long Tabulation() => DecodeWaysIISolution.NumDecodingsByTabulation(_value);

    [Benchmark]
    public long Memoized() => DecodeWaysIISolution.NumDecodingsByMemoization(_value);
}
