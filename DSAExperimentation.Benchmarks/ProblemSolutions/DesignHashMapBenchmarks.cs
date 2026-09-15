using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignHashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignHashMapSolution's, the same classes
// DesignHashMapTests proves correct. Both are populated with the same Length
// distinct keys, then probed with Length Get calls split evenly between present
// and absent keys, so the linear scan's O(n) cost per lookup is fully exercised
// on every probe rather than short-circuited by an early hit. Every populated
// value equals its own key (always >= 0), so "Get(key) != -1" is an unambiguous
// hit test for both strategies.
[MemoryDiagnoser]
public class DesignHashMapBenchmarks
{
    private const int AlternatingModulus = 2;

    private int[] _keys = [];

    private int[] _probeKeys = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _keys = Enumerable.Range(0, Length).ToArray();

        // Half hits (existing keys), half misses (keys one past the populated range) -
        // a miss forces both strategies through their full failed-lookup path (a whole
        // list scan, or a whole bucket-chain walk) instead of an early return.
        _probeKeys = Enumerable.Range(0, Length)
            .Select(i => IsHit(i) ? ExistingKeyAt(i) : AbsentKeyAt(i))
            .ToArray();
    }

    // The hit/miss split the probe keys above are built from.
    private bool IsHit(int i) => i % AlternatingModulus == 0;

    private int ExistingKeyAt(int i) => _keys[i];

    private int AbsentKeyAt(int i) => Length + i;

    [Benchmark(Baseline = true)]
    public int LinearScanList() => Replay(new DesignHashMapSolution.MyHashMapByLinearScanList());

    [Benchmark]
    public int HashMapBacked() => Replay(new DesignHashMapSolution.MyHashMapByHashMapBacked());

    private int Replay(DesignHashMapSolution.IMyHashMap map)
    {
        foreach (var key in _keys)
        {
            map.Put(key, key);
        }

        var found = 0;

        foreach (var probe in _probeKeys)
        {
            if (map.Get(probe) != -1)
            {
                found++;
            }
        }

        return found;
    }
}
