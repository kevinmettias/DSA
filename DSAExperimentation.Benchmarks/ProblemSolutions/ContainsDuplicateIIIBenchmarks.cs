using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Contains Duplicate III (LC 220): the canonical O(n*indexDiff) sliding-window brute force
// vs. the O(n) bucketed sliding window using this repo's own HashMap<long,long>. Values are
// spaced far enough apart that no pair ever actually satisfies valueDiff, forcing both
// strategies through their full worst-case window scan on every index instead of an early
// exit making brute force look artificially competitive.
[MemoryDiagnoser]
public class ContainsDuplicateIIIBenchmarks
{
    private const int IndexDiff = 50;
    private const int ValueDiff = 3;
    private const int RandomValueUpperBound = 1_000;
    private const int ValueSpacingMultiplier = 100;

    [Params(500, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, RandomValueUpperBound) * ValueSpacingMultiplier)
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool SlidingWindowBruteForce()
    {
        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = Math.Max(0, i - IndexDiff); j < i; j++)
            {
                if (Math.Abs((long)_values[i] - _values[j]) <= ValueDiff)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool BucketedHashMap()
    {
        const long width = ValueDiff + 1;
        var buckets = new HashMap<long, long>();

        for (var i = 0; i < _values.Length; i++)
        {
            if (IsDuplicateAtIndex(buckets, i, width))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsDuplicateAtIndex(HashMap<long, long> buckets, int i, long width)
    {
        var bucketId = BucketId(_values[i], width);

        if (buckets.HasKey(bucketId)
            || (buckets.TryGetValue(bucketId - 1, out var lower) && _values[i] - lower <= ValueDiff)
            || (buckets.TryGetValue(bucketId + 1, out var upper) && upper - _values[i] <= ValueDiff))
        {
            return true;
        }

        buckets.Set(bucketId, _values[i]);

        if (i >= IndexDiff)
        {
            var staleBucketId = BucketId(_values[i - IndexDiff], width);
            buckets.TryRemove(staleBucketId);
        }

        return false;
    }

    private static long BucketId(long value, long width) => value >= 0 ? value / width : ((value + 1) / width) - 1;
}
