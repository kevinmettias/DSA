using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ContainsDuplicateIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContainsDuplicateIIISolution's, the same methods
// ContainsDuplicateIIITests proves correct. Values are spaced far enough apart
// that no pair ever actually satisfies valueDiff, forcing both strategies through
// their full worst-case window scan on every index instead of an early exit
// making brute force look artificially competitive.
//
// That spacing has to hold for every pair, so each index draws its own multiple of
// ValueSpacingMultiplier rather than an arbitrary one: drawing from a fixed pool of
// RandomValueUpperBound multiples of that step only spaces DISTINCT values apart, and
// equal draws - certain long before Length reaches the pool's size - are exactly the
// pair valueDiff = 3 admits, which let both arms exit early and timed the window scan
// they exist to avoid. The multiples are then shuffled with the fixed seed, the same
// "deterministic permutation for a benchmark workload" idiom
// CountIncreasingQuadrupletsBenchmarks' own Setup uses.
[MemoryDiagnoser]
public class ContainsDuplicateIIIBenchmarks
{
    private const int IndexDiff = 50;
    private const int ValueDiff = 3;
    private const int ValueSpacingMultiplier = 100;

    private int[] _values = [];

    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(1, Length)
            .Select(multiple => multiple * ValueSpacingMultiplier)
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool HasNearbyAlmostDuplicateBySlidingWindowBruteForce() =>
        ContainsDuplicateIIISolution.HasNearbyAlmostDuplicateBySlidingWindowBruteForce(
            _values, IndexDiff, ValueDiff);

    [Benchmark]
    public bool HasNearbyAlmostDuplicateByBucketedHashMap() =>
        ContainsDuplicateIIISolution.HasNearbyAlmostDuplicateByBucketedHashMap(
            _values, IndexDiff, ValueDiff);
}
