using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.Finding3DigitEvenNumbers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are Finding3DigitEvenNumbersSolution's, the same methods
// Finding3DigitEvenNumbersTests proves correct. The digit array is LeetCode's own
// input shape, so [GlobalSetup] only has to choose its size and seed.
//
// Digits are drawn from the full 0-9 alphabet specifically so most of the ~450
// possible distinct 3-digit even numbers actually get found, giving the baseline's
// growing List.Contains scan real duplicate-checking work to pay for on every
// candidate instead of exiting a mostly-empty list immediately.
[MemoryDiagnoser]
public class Finding3DigitEvenNumbersBenchmarks
{
    private const int RandomSeed = 2094; // LC problem number
    private const int DigitRangeExclusive = 10;

    [Params(30, 100)]
    public int Length;

    private int[] _digits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _digits = Enumerable.Range(0, Length).Select(_ => random.Next(0, DigitRangeExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] ListContainsScanDedupe() =>
        Finding3DigitEvenNumbersSolution.FindEvenNumbersByListScanDedupe(_digits);

    [Benchmark]
    public int[] SetDedupeThenMergeSort() =>
        Finding3DigitEvenNumbersSolution.FindEvenNumbersBySetDedupe(_digits);
}
