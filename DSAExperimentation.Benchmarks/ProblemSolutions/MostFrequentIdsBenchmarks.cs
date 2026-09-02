using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MostFrequentIds;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MostFrequentIdsSolution's, the same methods
// MostFrequentIdsTests proves correct. A modest id space (relative to Length)
// keeps counts churning and the collection non-trivially sized throughout, so
// the brute-force rescan actually has work to do at every step.
[MemoryDiagnoser]
public class MostFrequentIdsBenchmarks
{
    private const int MaxIdExclusive = 1_000;
    private const int MaxFreqMagnitude = 5;
    private const int Seed = 3092;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[] _freq = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = new int[Length];
        _freq = new int[Length];

        // Tracks running per-id counts so a generated freq[i] can never take an
        // id negative, matching LeetCode's own guarantee about the input.
        var counts = new Dictionary<int, int>();

        for (var i = 0; i < Length; i++)
        {
            var id = random.Next(1, MaxIdExclusive);
            counts.TryGetValue(id, out var current);

            var delta = current > 0 && random.Next(2) == 0
                ? -random.Next(1, Math.Min(current, MaxFreqMagnitude) + 1)
                : random.Next(1, MaxFreqMagnitude + 1);

            counts[id] = current + delta;
            _nums[i] = id;
            _freq[i] = delta;
        }
    }

    [Benchmark(Baseline = true)]
    public long[] BruteForce() => MostFrequentIdsSolution.MostFrequentCountsByBruteForce(_nums, _freq);

    [Benchmark]
    public long[] LazyHeap() => MostFrequentIdsSolution.MostFrequentCountsByLazyHeap(_nums, _freq);
}
