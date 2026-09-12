using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignHashMap.DesignHashMapSolution;

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

    [Params(200, 5_000)]
    public int Length;

    private int[] _keys = null!;
    private int[] _probeKeys = null!;

    [GlobalSetup]
    public void Setup()
    {
        _keys = Enumerable.Range(0, Length).ToArray();

        // Half hits (existing keys), half misses (keys one past the populated range) -
        // a miss forces both strategies through their full failed-lookup path (a whole
        // list scan, or a whole bucket-chain walk) instead of an early return.
        _probeKeys = Enumerable.Range(0, Length)
            .Select(i => i % AlternatingModulus == 0 ? _keys[i] : Length + i)
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanList() => Replay(new MyHashMapByLinearScanList());

    [Benchmark]
    public int HashMapBacked() => Replay(new MyHashMapByHashMapBacked());

    private int Replay(IMyHashMap map)
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
