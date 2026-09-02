using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CreateMaximumNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CreateMaximumNumberSolution's, the same methods
// CreateMaximumNumberTests proves correct.
[MemoryDiagnoser]
public class CreateMaximumNumberBenchmarks
{
    private const int RandomSeed = 321; // LeetCode problem number

    private const int DigitUpperBoundExclusive = 10;

    [Params(20, 100)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(0, DigitUpperBoundExclusive)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(0, DigitUpperBoundExclusive)).ToArray();
        _k = Length;
    }

    [Benchmark(Baseline = true)]
    public int[] NaiveSubsequenceScan() => CreateMaximumNumberSolution.MaxNumberByNaiveScan(_nums1, _nums2, _k);

    [Benchmark]
    public int[] MonotonicStackSubsequence() =>
        CreateMaximumNumberSolution.MaxNumberByMonotonicStack(_nums1, _nums2, _k);
}
