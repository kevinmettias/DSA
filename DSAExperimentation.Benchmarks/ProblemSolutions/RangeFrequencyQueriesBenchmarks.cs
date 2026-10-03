using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RangeFrequencyQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RangeFrequencyQueriesSolution's, the same methods
// RangeFrequencyQueriesSolutionTests proves correct, run over a fixed batch of queries -
// RangeFreqQuery's index is built once per arm and queried QueryCount times against
// it, the pattern LeetCode's own class exposes, so the comparison is "build one
// index + QueryCount O(log n) lookups" against "no build + QueryCount O(n) rescans".
public class RangeFrequencyQueriesBenchmarks
{
    private const int QueryCount = 500;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 2080;

    // The array and the queried values both hold ValueRange values from 1, LC 2080's lowest.
    private const int ValueRange = 50;
    private const int MinValue = 1;
    private const int ValueBoundExclusive = MinValue + ValueRange;

    private int[] _arr = [];

    private (int Left, int Right, int Value)[] _queries = [];

    // Every query's frequency, in query order - what each arm returns.
    private int[] _frequencies = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = SeededDraws.Values(Length, MinValue, ValueBoundExclusive, random);

        _queries = new (int Left, int Right, int Value)[QueryCount];
        for (var i = 0; i < QueryCount; i++)
        {
            var left = random.Next(0, Length);
            var right = random.Next(left, Length);
            var value = random.Next(MinValue, ValueBoundExclusive);
            _queries[i] = (left, right, value);
        }

        _frequencies = new int[QueryCount];
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceRescan()
    {
        for (var i = 0; i < _queries.Length; i++)
        {
            var (left, right, value) = _queries[i];
            _frequencies[i] = RangeFrequencyQueriesSolution.QueryByBruteForceRescan(_arr, left, right, value);
        }

        return _frequencies;
    }

    [Benchmark]
    public int[] HashMapWithBinarySearch()
    {
        var indicesByValue = RangeFrequencyQueriesSolution.BuildValueIndex(_arr);

        for (var i = 0; i < _queries.Length; i++)
        {
            var (left, right, value) = _queries[i];
            _frequencies[i] = RangeFrequencyQueriesSolution.QueryByBinarySearchIndex(indicesByValue, left, right, value);
        }

        return _frequencies;
    }
}
