using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RepeatedStringMatch;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RepeatedStringMatchSolution's, the same methods
// RepeatedStringMatchTests proves correct. `a` and `b` are built so they never
// match at any repeat count, forcing both strategies through every candidate
// length instead of an early-exit on the first.
[MemoryDiagnoser]
public class RepeatedStringMatchBenchmarks
{
    private const int APatternFillerLength = 9;

    [Params(200, 5_000)]
    public int Length;

    private string _a = null!;
    private string _b = null!;

    [GlobalSetup]
    public void Setup()
    {
        _a = new string('a', APatternFillerLength) + 'b';
        _b = new string('a', Length) + 'c';
    }

    [Benchmark(Baseline = true)]
    public int StringContains() => RepeatedStringMatchSolution.MinRepeatsByStringContains(_a, _b);

    [Benchmark]
    public int PrefixFunctionSearchContains() => RepeatedStringMatchSolution.MinRepeatsByPrefixFunctionSearch(_a, _b);
}
