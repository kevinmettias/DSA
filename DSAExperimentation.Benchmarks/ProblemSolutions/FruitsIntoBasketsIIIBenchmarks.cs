using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FruitsIntoBasketsIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FruitsIntoBasketsIIISolution's, the same
// methods FruitsIntoBasketsIIITests proves correct. Random fruits/baskets
// across the problem's full 1e9 capacity range avoid the O(n^2) rescan
// getting an unrealistic early-exit shape, so the segment tree's O(log^2 n)
// search is measured against a genuinely scanning baseline.
[MemoryDiagnoser]
public class FruitsIntoBasketsIIIBenchmarks
{
    private const int Seed = 3479;
    private const int MaxCapacityExclusive = 1_000_000_000;

    private int[] _fruits = [];

    private int[] _baskets = [];
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _fruits = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxCapacityExclusive)).ToArray();
        _baskets = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxCapacityExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FruitsIntoBasketsIIISolution.CountUnplacedByBruteForce(_fruits, _baskets);

    [Benchmark]
    public int SegmentTreeSearch() => FruitsIntoBasketsIIISolution.CountUnplacedBySegmentTreeSearch(_fruits, _baskets);
}
