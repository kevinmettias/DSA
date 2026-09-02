using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindBeautifulIndicesInTheGivenArrayISolution's, the
// same methods FindBeautifulIndicesInTheGivenArrayITests proves correct. a and b
// each repeat s's own character but mismatch only in their very last position -
// the classic worst case for a naive substring scan, where every start position is
// compared almost all the way through the pattern before failing, instead of an
// early first-character mismatch making brute force look artificially competitive.
// Pattern length scales with s's length so the O(n*m) vs O(n) gap widens with it.
[MemoryDiagnoser]
public class FindBeautifulIndicesInTheGivenArrayIBenchmarks
{
    private const int SearchWindow = 100;

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private string _a = null!;
    private string _b = null!;

    [GlobalSetup]
    public void Setup()
    {
        var patternLength = Math.Max(1, Length / 10);
        _s = new string('a', Length);
        _a = new string('a', patternLength - 1) + 'b';
        _b = new string('a', patternLength - 1) + 'c';
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        FindBeautifulIndicesInTheGivenArrayISolution.FindBeautifulIndicesByBruteForce(_s, _a, _b, SearchWindow);

    [Benchmark]
    public int[] PrefixFunctionSearch() =>
        FindBeautifulIndicesInTheGivenArrayISolution.FindBeautifulIndicesByPrefixFunctionSearch(_s, _a, _b, SearchWindow);
}
