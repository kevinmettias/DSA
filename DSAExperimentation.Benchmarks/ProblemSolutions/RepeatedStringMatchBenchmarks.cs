using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Repeated String Match (LC 686): built-in string.Contains (baseline, the same
// "treat the BCL method as brute force" role StringIndexOf plays in
// FindTheIndexOfTheFirstOccurrenceInAStringBenchmarks) vs this repo's KMP-based
// PrefixFunctionSearch, checked against up to ceil(b.Length/a.Length)+1 repeats of
// `a`. `a` and `b` are built so they never match at any repeat count, forcing both
// strategies through every candidate length instead of an early-exit on the first.
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
    public int StringContains() => MinRepeats(_a, _b, static (candidate, pattern) => candidate.Contains(pattern, StringComparison.Ordinal));

    [Benchmark]
    public int PrefixFunctionSearchContains() => MinRepeats(_a, _b, static (candidate, pattern) => PrefixFunctionSearch.FindAll(candidate, pattern).Count > 0);

    private static int MinRepeats(string a, string b, Func<string, string, bool> contains)
    {
        var minRepeats = (int)Math.Ceiling((double)b.Length / a.Length);

        for (var repeats = minRepeats; repeats <= minRepeats + 1; repeats++)
        {
            var repeatedSegments = Enumerable.Repeat(a, repeats);
            var candidate = string.Concat(repeatedSegments);
            if (contains(candidate, b))
            {
                return repeats;
            }
        }

        return -1;
    }
}
