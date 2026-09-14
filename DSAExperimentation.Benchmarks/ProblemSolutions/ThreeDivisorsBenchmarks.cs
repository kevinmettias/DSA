using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ThreeDivisors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ThreeDivisorsSolution's, the same methods
// ThreeDivisorsTests proves correct - the textbook O(num) full-range trial division
// per number against BinarySearch.LowerBound anchoring each number's scan at
// floor(sqrt(num)), then a short downward walk that bails out the moment a fourth
// divisor appears. The random numbers are built once in [GlobalSetup], so generation
// is not charged to either arm; each arm tallies how many of them have exactly three
// divisors so the whole workload is consumed.
[MemoryDiagnoser]
public class ThreeDivisorsBenchmarks
{
    // LC problem number, reused as the deterministic workload seed.
    private const int RandomSeed = 1952;
    private const int MaxGeneratedNumber = 20_000;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxGeneratedNumber)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRangeScan()
    {
        var total = 0;

        foreach (var num in _nums)
        {
            total += ThreeDivisorsSolution.IsThreeByFullRangeScan(num) ? 1 : 0;
        }

        return total;
    }

    [Benchmark]
    public int BinarySearchAnchored()
    {
        var total = 0;

        foreach (var num in _nums)
        {
            total += ThreeDivisorsSolution.IsThreeByBinarySearchAnchor(num) ? 1 : 0;
        }

        return total;
    }
}
