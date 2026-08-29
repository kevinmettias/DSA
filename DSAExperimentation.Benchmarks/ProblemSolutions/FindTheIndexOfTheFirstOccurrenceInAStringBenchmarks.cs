using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class FindTheIndexOfTheFirstOccurrenceInAStringBenchmarks
{
    private string _haystack = null!;
    private const string Needle = "needle";
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() => _haystack = new string('a', Length) + Needle;
    [Benchmark(Baseline = true)] public int StringIndexOf() => _haystack.IndexOf(Needle, StringComparison.Ordinal);
    [Benchmark] public int RollingHashSearchFirst() { var matches = RollingHashSearch.FindAll(_haystack, Needle); return matches.Count == 0 ? -1 : matches[0]; }
}
