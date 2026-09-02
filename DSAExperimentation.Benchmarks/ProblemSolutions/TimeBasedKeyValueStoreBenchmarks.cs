using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Time Based Key-Value Store (LC 981): a linear floor-scan over one key's recorded
// timestamp history vs. this repo's own BinarySearch.UpperBound over a
// DynamicArraySequence<int> witness (TimeBasedKeyValueStoreTests' exact "floor
// lookup" composition, OnlineElectionBenchmarks' precedent for benchmarking that
// idiom). Setup seeds one key's history with Length strictly increasing
// timestamps (the problem's own guarantee); _queryTimestamp sits just past the
// final entry so both strategies are forced through their full worst-case scan,
// the same "force the real worst case" convention TwoSumBenchmarks uses.
[MemoryDiagnoser]
public class TimeBasedKeyValueStoreBenchmarks
{
    private const int TimestampStep = 2;

    [Params(200, 5_000)]
    public int Length;

    private DynamicArray<int> _timestamps = null!;
    private DynamicArray<string> _values = null!;
    private int _queryTimestamp;

    [GlobalSetup]
    public void Setup()
    {
        _timestamps = new DynamicArray<int>();
        _values = new DynamicArray<string>();

        for (var i = 0; i < Length; i++)
        {
            _timestamps.Add(i * TimestampStep);
            _values.Add($"v{i}");
        }

        _queryTimestamp = (Length * TimestampStep) + 1;
    }

    [Benchmark(Baseline = true)]
    public string LinearFloorScan()
    {
        var result = string.Empty;

        for (var i = 0; i < _timestamps.Count; i++)
        {
            if (_timestamps.Get(i) > _queryTimestamp)
            {
                break;
            }

            result = _values.Get(i);
        }

        return result;
    }

    [Benchmark]
    public string BinarySearchFloor()
    {
        var sequence = new DynamicArraySequence<int>(_timestamps);
        var floorIndex = BinarySearch.UpperBound(sequence, _queryTimestamp) - 1;

        return floorIndex < 0 ? string.Empty : _values.Get(floorIndex);
    }
}
