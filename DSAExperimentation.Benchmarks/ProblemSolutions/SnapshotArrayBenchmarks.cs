using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Snapshot Array (LC 1146): a linear floor-scan over one index's recorded snap-id
// history vs. this repo's own BinarySearch.UpperBound over a
// DynamicArraySequence<int> witness - the same "floor lookup" composition
// SnapshotArrayTests/TimeBasedKeyValueStoreBenchmarks already use. Setup seeds one
// index's history with Length strictly increasing snap ids (one Set per snap, the
// worst case for history size); _querySnapId sits just past the final entry so
// both strategies are forced through their full worst-case scan, the same "force
// the real worst case" convention TwoSumBenchmarks uses.
[MemoryDiagnoser]
public class SnapshotArrayBenchmarks
{
    private const int ValueScaleFactor = 2;

    [Params(200, 5_000)]
    public int Length;

    private DynamicArray<int> _snapIds = null!;
    private DynamicArray<int> _values = null!;
    private int _querySnapId;

    [GlobalSetup]
    public void Setup()
    {
        _snapIds = new DynamicArray<int>();
        _values = new DynamicArray<int>();

        for (var i = 0; i < Length; i++)
        {
            _snapIds.Add(i);
            _values.Add(i * ValueScaleFactor);
        }

        _querySnapId = Length + 1;
    }

    [Benchmark(Baseline = true)]
    public int LinearFloorScan()
    {
        var result = 0;

        for (var i = 0; i < _snapIds.Count; i++)
        {
            if (_snapIds.Get(i) > _querySnapId)
            {
                break;
            }

            result = _values.Get(i);
        }

        return result;
    }

    [Benchmark]
    public int BinarySearchFloor()
    {
        var sequence = new DynamicArraySequence<int>(_snapIds);
        var floorIndex = BinarySearch.UpperBound(sequence, _querySnapId) - 1;

        return floorIndex < 0 ? 0 : _values.Get(floorIndex);
    }
}
