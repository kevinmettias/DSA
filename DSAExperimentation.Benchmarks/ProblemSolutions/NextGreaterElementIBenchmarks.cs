using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NextGreaterElementI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NextGreaterElementISolution's, the same methods
// NextGreaterElementITests proves correct. nums2 is a random permutation of
// distinct values so no query short-circuits on an early match, forcing
// PerQueryRescan through its full worst-case inner scan.
[MemoryDiagnoser]
public class NextGreaterElementIBenchmarks
{
    private const int RandomSeed = 496; private int[] _nums1 = [];

    private int[] _nums2 = [];
    // LC problem number

    [Params(200, 2_500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums2 = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _nums1 = _nums2.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] PerQueryRescan() =>
        NextGreaterElementISolution.NextGreaterElementByPerQueryRescan(_nums1, _nums2);

    [Benchmark]
    public int[] MonotonicStackSweep() =>
        NextGreaterElementISolution.NextGreaterElementByMonotonicStackSweep(_nums1, _nums2);
}
