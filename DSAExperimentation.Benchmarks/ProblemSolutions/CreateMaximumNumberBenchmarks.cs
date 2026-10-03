using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CreateMaximumNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CreateMaximumNumberSolution's, the same methods
// CreateMaximumNumberSolutionTests proves correct.
public class CreateMaximumNumberBenchmarks
{
    private const int RandomSeed = 321; // LeetCode problem number

    private const int DigitUpperBoundExclusive = 10;

    private int[] _nums1 = [];

    private int[] _nums2 = [];
    private int _digitCount;
    [Params(20, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = SeededDraws.Values(Length, 0, DigitUpperBoundExclusive, random);
        _nums2 = SeededDraws.Values(Length, 0, DigitUpperBoundExclusive, random);
        _digitCount = Length;
    }

    [Benchmark(Baseline = true)]
    public int[] NaiveSubsequenceScan() => CreateMaximumNumberSolution.MaxNumberByNaiveScan(_nums1, _nums2, _digitCount);

    [Benchmark]
    public int[] MonotonicStackSubsequence() =>
        CreateMaximumNumberSolution.MaxNumberByMonotonicStack(_nums1, _nums2, _digitCount);
}
