using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TimeBasedKeyValueStore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TimeBasedKeyValueStoreSolution's, the same strategies
// TimeBasedKeyValueStoreTests proves correct. Setup seeds each store with one key's
// Length strictly increasing timestamps (the problem's own guarantee), charging that
// construction to [GlobalSetup]; _queryTimestamp sits just past the final entry so
// both strategies are forced through their full worst-case floor lookup, the same
// "force the real worst case" convention OnlineElectionBenchmarks uses.
[MemoryDiagnoser]
public class TimeBasedKeyValueStoreBenchmarks
{
    private const int TimestampStep = 2;
    private const string Key = "foo";

    [Params(200, 5_000)]
    public int Length;

    private TimeBasedKeyValueStoreSolution.ITimeMap _linearFloorScan = null!;
    private TimeBasedKeyValueStoreSolution.ITimeMap _binarySearchFloor = null!;
    private int _queryTimestamp;

    [GlobalSetup]
    public void Setup()
    {
        _linearFloorScan = Seed(TimeBasedKeyValueStoreSolution.CreateByLinearFloorScan());
        _binarySearchFloor = Seed(TimeBasedKeyValueStoreSolution.CreateByBinarySearchFloor());
        _queryTimestamp = (Length * TimestampStep) + 1;
    }

    [Benchmark(Baseline = true)]
    public string LinearFloorScan() => _linearFloorScan.Get(Key, _queryTimestamp);

    [Benchmark]
    public string BinarySearchFloor() => _binarySearchFloor.Get(Key, _queryTimestamp);

    private TimeBasedKeyValueStoreSolution.ITimeMap Seed(TimeBasedKeyValueStoreSolution.ITimeMap store)
    {
        for (var i = 0; i < Length; i++)
        {
            store.Set(Key, $"v{i}", i * TimestampStep);
        }

        return store;
    }
}
