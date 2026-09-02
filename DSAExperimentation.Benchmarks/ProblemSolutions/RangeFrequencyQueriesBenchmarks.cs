using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Range Frequency Queries (LC 2080): a brute-force rescan baseline (counts
// occurrences of value in arr[left..right] directly, O(n) per query) vs. this repo's
// own HashMap<TKey,TValue> mapping each value to a DynamicArray<int> of its indices
// (built once, O(n)), so every Query afterward is an O(log n)
// BinarySearch.UpperBound - LowerBound instead of a rescan. Both methods answer the
// same fixed batch of queries.
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
            for (var i = left; i <= right; i++)
            {
                if (_arr[i] == value)
                {
                    total++;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public long HashMapWithBinarySearch()
    {
        var indicesByValue = BuildValueIndex();
        return SumRangeMatches(indicesByValue);
    }

    private HashMap<int, DynamicArray<int>> BuildValueIndex()
    {
        var indicesByValue = new HashMap<int, DynamicArray<int>>();

        for (var i = 0; i < _arr.Length; i++)
        {
            if (!indicesByValue.TryGetValue(_arr[i], out var indices))
            {
                indices = new DynamicArray<int>();
                indicesByValue.Set(_arr[i], indices);
            }

            indices.Add(i);
        }

        return indicesByValue;
    }

    private long SumRangeMatches(HashMap<int, DynamicArray<int>> indicesByValue)
    {
        var total = 0L;

        foreach (var (left, right, value) in _queries)
        {
            if (!indicesByValue.TryGetValue(value, out var indices))
            {
                continue;
            }

            var sequence = new DynamicArraySequence<int>(indices);
            total += BinarySearch.UpperBound(sequence, right) - BinarySearch.LowerBound(sequence, left);
        }

        return total;
    }
}
