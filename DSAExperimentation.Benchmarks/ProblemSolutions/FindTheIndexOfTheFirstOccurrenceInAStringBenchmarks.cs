using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheIndexOfTheFirstOccurrenceInAString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheIndexOfTheFirstOccurrenceInAStringSolution's,
// the same methods FindTheIndexOfTheFirstOccurrenceInAStringTests proves correct.
[MemoryDiagnoser]
public class FindTheIndexOfTheFirstOccurrenceInAStringBenchmarks
{
    private const string Needle = "needle";

    [Params(200, 5_000)]
    public int Length;

    private string _haystack = null!;

    [GlobalSetup]
    public void Setup() => _haystack = new string('a', Length) + Needle;

    [Benchmark(Baseline = true)]
    public int StringIndexOf() =>
        FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByStringIndexOf(_haystack, Needle);

    [Benchmark]
    public int RollingHashSearchFirst() =>
        FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByRollingHash(_haystack, Needle);
}
