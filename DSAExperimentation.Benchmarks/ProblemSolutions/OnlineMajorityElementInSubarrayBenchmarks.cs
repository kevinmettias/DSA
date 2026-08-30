using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Online Majority Element In Subarray (LC 1157): a naive per-query O(range) scan
// (tally every value in [left, right] with a Dictionary, then check the threshold)
// vs. this repo's own HashMap<int, DynamicArray<int>> position index, counting a
// candidate's occurrences in O(log n) via BinarySearch.LowerBound/UpperBound over a
// DynamicArraySequence<int> view. Every generated query's range sits entirely inside
// one same-valued run, so its left endpoint's value is always the answer with an
// occurrence count equal to the whole range - a trivially verifiable majority
// (2*threshold > range) for every query, matching LeetCode's own query guarantee,
// without needing randomized sampling to demonstrate the timing difference.
[MemoryDiagnoser]
public class OnlineMajorityElementInSubarrayBenchmarks
{
    private const int RunLength = 25;
    private const int QueryCount = 200;

    [Params(1_000, 8_000)]
    public int Length;

    private int[] _values = null!;
    private (int Left, int Right, int Threshold)[] _queries = null!;
    private HashMap<int, DynamicArray<int>> _positionsByValue = null!;

    [GlobalSetup]
    public void Setup()
    {
        _values = BuildRunLengthEncodedArray(Length);
        _queries = BuildQueriesWithinRuns(Length);
        _positionsByValue = BuildPositionIndex(_values);
    }

    private static int[] BuildRunLengthEncodedArray(int length)
    {
        var values = new int[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = i / RunLength;
        }

        return values;
    }

    private static (int Left, int Right, int Threshold)[] BuildQueriesWithinRuns(int length)
    {
        var random = new Random(1);
        var queries = new (int Left, int Right, int Threshold)[QueryCount];
        var runCount = (length + RunLength - 1) / RunLength;

        for (var i = 0; i < QueryCount; i++)
        {
            var run = random.Next(runCount);
            var runStart = run * RunLength;
            var runEnd = Math.Min(runStart + RunLength, length) - 1;
            var left = random.Next(runStart, runEnd + 1);
            var right = random.Next(left, runEnd + 1);
            queries[i] = (left, right, right - left + 1);
        }

        return queries;
    }

    private static HashMap<int, DynamicArray<int>> BuildPositionIndex(int[] values)
    {
        var positionsByValue = new HashMap<int, DynamicArray<int>>();

        for (var i = 0; i < values.Length; i++)
        {
            if (!positionsByValue.TryGetValue(values[i], out var positions))
            {
                positions = new DynamicArray<int>();
                positionsByValue.Set(values[i], positions);
            }

            positions.Add(i);
        }

        return positionsByValue;
    }

    [Benchmark(Baseline = true)]
    public long LinearScanPerQuery()
    {
        var total = 0L;

        foreach (var (left, right, threshold) in _queries)
        {
            total += CountByLinearScan(left, right, threshold);
        }

        return total;
    }

    private int CountByLinearScan(int left, int right, int threshold)
    {
        var counts = new Dictionary<int, int>();

        for (var i = left; i <= right; i++)
        {
            counts[_values[i]] = counts.GetValueOrDefault(_values[i]) + 1;
        }

        foreach (var (value, count) in counts)
        {
            if (count >= threshold)
            {
                return value;
            }
        }

        return -1;
    }

    [Benchmark]
    public long HashMapWithBinarySearchPerQuery()
    {
        var total = 0L;

        foreach (var (left, right, threshold) in _queries)
        {
            total += QueryByCandidateAtLeft(left, right, threshold);
        }

        return total;
    }

    private int QueryByCandidateAtLeft(int left, int right, int threshold)
    {
        var value = _values[left];

        if (!_positionsByValue.TryGetValue(value, out var positions))
        {
            return -1;
        }

        var sequence = new DynamicArraySequence<int>(positions);
        var lower = BinarySearch.LowerBound<int, DynamicArraySequence<int>>(sequence, left);
        var upper = BinarySearch.UpperBound<int, DynamicArraySequence<int>>(sequence, right);

        return upper - lower >= threshold ? value : -1;
    }
}
