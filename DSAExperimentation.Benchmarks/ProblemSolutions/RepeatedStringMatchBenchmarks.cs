using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RepeatedStringMatch;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RepeatedStringMatchSolution's, the same methods
// RepeatedStringMatchTests proves correct. The repeated unit and the target pattern
// are built so they never match at any repeat count, forcing both strategies through
// every candidate length instead of an early-exit on the first.
[MemoryDiagnoser]
public class RepeatedStringMatchBenchmarks
{
    private const int APatternFillerLength = 9;

    private string _repeatedUnit = "";

    private string _targetPattern = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _repeatedUnit = new string('a', APatternFillerLength) + 'b';
        _targetPattern = new string('a', Length) + 'c';
    }

    [Benchmark(Baseline = true)]
    public int StringContains() =>
        RepeatedStringMatchSolution.MinRepeatsByStringContains(
            new RepeatedStringMatchSolution.RepeatedUnit(_repeatedUnit),
            new RepeatedStringMatchSolution.TargetPattern(_targetPattern));

    [Benchmark]
    public int PrefixFunctionSearchContains() =>
        RepeatedStringMatchSolution.MinRepeatsByPrefixFunctionSearch(
            new RepeatedStringMatchSolution.RepeatedUnit(_repeatedUnit),
            new RepeatedStringMatchSolution.TargetPattern(_targetPattern));
}
