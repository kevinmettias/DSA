using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SnapshotArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SnapshotArraySolution's, the same strategies
// SnapshotArrayTests proves correct. Setup seeds each array's single index with
// Length snapshots, one Set per Snap - the worst case for history size - and
// charges that construction to [GlobalSetup]; _querySnapId sits just past the final
// snapshot so both strategies are forced through their full worst-case floor
// lookup, the same "force the real worst case" convention
// TimeBasedKeyValueStoreBenchmarks uses.
[MemoryDiagnoser]
public class SnapshotArrayBenchmarks
{
    private const int ValueScaleFactor = 2;
    private const int IndexCount = 1;
    private const int QueryIndex = 0;

    private SnapshotArraySolution.ISnapshotArray _linearFloorScan = null!;

    private SnapshotArraySolution.ISnapshotArray _binarySearchFloor = null!;
    private int _querySnapId;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _linearFloorScan = Seed(SnapshotArraySolution.CreateByLinearFloorScan(IndexCount));
        _binarySearchFloor = Seed(SnapshotArraySolution.CreateByBinarySearchFloor(IndexCount));
        _querySnapId = Length + 1;
    }

    private SnapshotArraySolution.ISnapshotArray Seed(SnapshotArraySolution.ISnapshotArray snapshotArray)
    {
        for (var i = 0; i < Length; i++)
        {
            snapshotArray.Set(QueryIndex, i * ValueScaleFactor);
            snapshotArray.Snap();
        }

        return snapshotArray;
    }

    [Benchmark(Baseline = true)]
    public int LinearFloorScan() => _linearFloorScan.Get(QueryIndex, _querySnapId);

    [Benchmark]
    public int BinarySearchFloor() => _binarySearchFloor.Get(QueryIndex, _querySnapId);
}
