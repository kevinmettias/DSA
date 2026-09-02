using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubstringMatchingPattern;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubstringMatchingPatternSolution's, the same methods
// SubstringMatchingPatternTests proves correct. s is a run of 'a's and both halves
// of p are a long run of 'a's followed by a 'b' - a character s never contains - so
// every candidate window matches almost the whole prefix/suffix before failing on
// the last character, forcing both strategies through their full worst case instead
// of an early first-character mismatch making brute force look artificially
// competitive.
[MemoryDiagnoser]
public class SubstringMatchingPatternBenchmarks
{
    [Params(50, 300)]
    public int Length;

    private string _s = null!;
    private string _p = null!;

    [GlobalSetup]
    public void Setup()
    {
        var half = new string('a', Length / 4) + 'b';
        _s = new string('a', Length);
        _p = $"{half}*{half}";
    }

    [Benchmark(Baseline = true)]
    public bool BruteForce() => SubstringMatchingPatternSolution.HasMatchByBruteForce(_s, _p);

    [Benchmark]
    public bool PrefixFunctionSearch() => SubstringMatchingPatternSolution.HasMatchByPrefixFunctionSearch(_s, _p);
}
