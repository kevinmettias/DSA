using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheIndexOfTheFirstOccurrenceInAString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheIndexOfTheFirstOccurrenceInAStringSolution's,
// the same methods FindTheIndexOfTheFirstOccurrenceInAStringTests proves correct.
[MemoryDiagnoser]
public class FindTheIndexOfTheFirstOccurrenceInAStringBenchmarks
{
    private const string Needle = "needle";

    private string _haystack = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _haystack = new string('a', Length) + Needle;

    [Benchmark(Baseline = true)]
    public int StringIndexOf() =>
        FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByStringIndexOf(
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Haystack(_haystack),
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Needle(Needle));

    [Benchmark]
    public int RollingHashSearchFirst() =>
        FindTheIndexOfTheFirstOccurrenceInAStringSolution.IndexOfByRollingHash(
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Haystack(_haystack),
            new FindTheIndexOfTheFirstOccurrenceInAStringSolution.Needle(Needle));
}
