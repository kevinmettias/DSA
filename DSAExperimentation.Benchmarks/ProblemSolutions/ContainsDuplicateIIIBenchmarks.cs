using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ContainsDuplicateIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContainsDuplicateIIISolution's, the same methods
// ContainsDuplicateIIITests proves correct. Values are spaced far enough apart
// that no pair ever actually satisfies valueDiff, forcing both strategies through
// their full worst-case window scan on every index instead of an early exit
// making brute force look artificially competitive.
[MemoryDiagnoser]
public class ContainsDuplicateIIIBenchmarks
{
    private const int IndexDiff = 50;
    private const int ValueDiff = 3;
    private const int RandomValueUpperBound = 1_000;
    private const int ValueSpacingMultiplier = 100;

    private int[] _values = [];

    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, RandomValueUpperBound) * ValueSpacingMultiplier)
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
