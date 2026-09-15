using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DecodeWays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DecodeWaysSolution's, the same methods
// DecodeWaysTests proves correct.
[MemoryDiagnoser]
public class DecodeWaysBenchmarks
{
    private string _value = "";

    [Params(20, 80)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _value = new string('1', Length);

    [Benchmark(Baseline = true)]
    public int Tabulation() => DecodeWaysSolution.NumDecodingsByTabulation(_value);

    [Benchmark]
    public int Memoized() => DecodeWaysSolution.NumDecodingsByMemoization(_value);
}
