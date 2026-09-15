using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfSubarraysThatMatchAPatternIISolution's,
// the same methods NumberOfSubarraysThatMatchAPatternIITests proves correct.
// NumsLength is scaled up from Part I's benchmark (20/100) to actually
// exercise the gap the O(n + m) ZFunction strategy exists to close, while
// staying well short of this problem's own 10^6 bound so BruteForce's O(n*m)
// arm still finishes in a reasonable benchmark run. nums is a strictly
// increasing run and pattern is all 1s, so every window matches - the worst
// case for BruteForce, since no candidate start can bail out on an early
// mismatch and every one has to be walked to completion.
[MemoryDiagnoser]
public class NumberOfSubarraysThatMatchAPatternIIBenchmarks
{
    private int[] _nums = [];

    private int[] _pattern = [];
    [Params(1000, 5000)]
    public int NumsLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums = Enumerable.Range(1, NumsLength).ToArray();
        _pattern = Enumerable.Repeat(1, NumsLength / 2).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => NumberOfSubarraysThatMatchAPatternIISolution.CountMatchesByBruteForce(_nums, _pattern);

    [Benchmark]
    public int ZFunction() => NumberOfSubarraysThatMatchAPatternIISolution.CountMatchesByZFunction(_nums, _pattern);
}
