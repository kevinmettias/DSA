using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfPairsSatisfyingInequality;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfPairsSatisfyingInequalitySolution's, the same
// methods NumberOfPairsSatisfyingInequalityTests proves correct - the textbook
// O(n^2) pairwise scan against the O(n log n) FenwickTree sweep over the
// coordinate-compressed differences.
[MemoryDiagnoser]
public class NumberOfPairsSatisfyingInequalityBenchmarks
{
    private const int RandomSeed = 2426; // LeetCode problem number
    private const int ValueBound = 10_000;

    private int[] _nums1 = [];
    private int[] _nums2 = [];
    private int _diff;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
        _diff = random.Next(0, ValueBound);
    }

    [Benchmark(Baseline = true)]
    public long PairwiseScan()
        => NumberOfPairsSatisfyingInequalitySolution.CountPairsByPairwiseScan(_nums1, _nums2, _diff);

    [Benchmark]
    public long FenwickTreeSweep()
        => NumberOfPairsSatisfyingInequalitySolution.CountPairsByFenwickTreeSweep(_nums1, _nums2, _diff);
}
