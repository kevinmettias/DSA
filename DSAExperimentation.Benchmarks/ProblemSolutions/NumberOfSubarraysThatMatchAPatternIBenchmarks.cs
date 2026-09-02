using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfSubarraysThatMatchAPatternISolution's,
// the same methods NumberOfSubarraysThatMatchAPatternITests proves correct.
// NumsLength matches LC 3034's own bound (n <= 100). nums is a strictly
// increasing run and pattern is all 1s, so every window matches - the worst
// case for BruteForce, since no candidate start can bail out on an early
// mismatch and every one has to be walked to completion.
[MemoryDiagnoser]
public class NumberOfSubarraysThatMatchAPatternIBenchmarks
{
    [Params(20, 100)]
    public int NumsLength;

    private int[] _nums = null!;
    private int[] _pattern = null!;

    [GlobalSetup]
    public void Setup()
    {
        _nums = Enumerable.Range(1, NumsLength).ToArray();
        _pattern = Enumerable.Repeat(1, NumsLength / 2).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => NumberOfSubarraysThatMatchAPatternISolution.CountMatchesByBruteForce(_nums, _pattern);

    [Benchmark]
    public int PrefixFunctionSearch() =>
        NumberOfSubarraysThatMatchAPatternISolution.CountMatchesByPrefixFunctionSearch(_nums, _pattern);
}
