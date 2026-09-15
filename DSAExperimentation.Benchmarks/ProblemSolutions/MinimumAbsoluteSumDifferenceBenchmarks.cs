using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumAbsoluteSumDifference;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumAbsoluteSumDifferenceSolution's, the same
// strategies MinimumAbsoluteSumDifferenceTests proves correct. The sort is charged
// to the measured method on purpose - paying for it once is the whole reason the
// sorted arm beats the O(n^2) rescan.
[MemoryDiagnoser]
public class MinimumAbsoluteSumDifferenceBenchmarks
{
    private const int MaxValueExclusive = 100_000;
    private const int ValueSeed = 1;

    private int[] _nums1 = [];

    private int[] _nums2 = [];
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(ValueSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRescan() =>
        MinimumAbsoluteSumDifferenceSolution.MinAbsoluteSumDiffByFullRescan(_nums1, _nums2);

    [Benchmark]
    public int SortedBinarySearch() =>
        MinimumAbsoluteSumDifferenceSolution.MinAbsoluteSumDiffBySortedBinarySearch(_nums1, _nums2);
}
