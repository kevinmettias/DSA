using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RemoveKDigits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveKDigitsSolution's, the same methods
// RemoveKDigitsTests proves correct - the LargestRectangleInHistogramBenchmarks
// precedent (O(n*k)-ish baseline vs. O(n) primitive-based sweep) applied to
// string-digit removal instead of histogram area.
[MemoryDiagnoser]
public class RemoveKDigitsBenchmarks
{
    private const int DigitCount = 10;
    private const int RemovalFraction = 3;

    private string _num = "";

    private int _removalCount;
    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var digits = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            digits[i] = (char)('0' + random.Next(0, DigitCount));
        }

        _num = new string(digits);
        _removalCount = Length / RemovalFraction;
    }

    [Benchmark(Baseline = true)]
    public string RepeatedFirstDescentRemoval() =>
        RemoveKDigitsSolution.RemoveByRepeatedFirstDescentRemoval(_num, _removalCount);

    [Benchmark]
    public string MonotonicStackSweep() =>
        RemoveKDigitsSolution.RemoveByMonotonicStackSweep(_num, _removalCount);
}
