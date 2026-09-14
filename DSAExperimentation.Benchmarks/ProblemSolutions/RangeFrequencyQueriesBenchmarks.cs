using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RangeFrequencyQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RangeFrequencyQueriesSolution's, the same methods
// RangeFrequencyQueriesTests proves correct, run over a fixed batch of queries -
// RangeFreqQuery's index is built once per arm and queried QueryCount times against
// it, the pattern LeetCode's own class exposes, so the comparison is "build one
// index + QueryCount O(log n) lookups" against "no build + QueryCount O(n) rescans".
[MemoryDiagnoser]
public class RangeFrequencyQueriesBenchmarks
{
    private const int QueryCount = 500;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 2080;

    private const int ValueRange = 50;

    [Params(200, 5_000)]
    public int Length;

    private int[] _arr = null!;
    private (int Left, int Right, int Value)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(0, Length).Select(_ => random.Next(ValueRange)).ToArray();

        _queries = new (int Left, int Right, int Value)[QueryCount];
        for (var i = 0; i < QueryCount; i++)
        {
            var left = random.Next(0, Length);
            var right = random.Next(left, Length);
            var value = random.Next(ValueRange);
            _queries[i] = (left, right, value);
        }
    }

    [Benchmark(Baseline = true)]
    public long BruteForceRescan()
    {
        var total = 0L;

        foreach (var (left, right, value) in _queries)
        {
            total += RangeFrequencyQueriesSolution.QueryByBruteForceRescan(_arr, left, right, value);
        }

        return total;
    }

    [Benchmark]
    public long HashMapWithBinarySearch()
    {
        var indicesByValue = RangeFrequencyQueriesSolution.BuildValueIndex(_arr);
        var total = 0L;

        foreach (var (left, right, value) in _queries)
        {
            total += RangeFrequencyQueriesSolution.QueryByBinarySearchIndex(indicesByValue, left, right, value);
        }

        return total;
    }
}
