using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design HashMap (LC 706): a naive List<(int Key,int Value)> linear-scan map (the
// "no hashing at all" baseline a first-pass implementation reaches for) vs. this repo's
// HashMap<TKey,TValue>. Both are populated with the same Length distinct keys, then
// probed with Length Get calls split evenly between present and absent keys, so the
// linear scan's O(n) cost per lookup is fully exercised on every probe rather than
// short-circuited by an early hit.
[MemoryDiagnoser]
public class DesignHashMapBenchmarks
{
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
            .Select(i => i % 2 == 0 ? _keys[i] : Length + i)
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanList()
    {
        var entries = new List<(int Key, int Value)>(Length);
        foreach (var key in _keys)
        {
            entries.Add((key, key));
        }

        var found = 0;
        foreach (var probe in _probeKeys)
        {
            foreach (var entry in entries)
            {
                if (entry.Key == probe)
                {
                    found++;
                    break;
                }
            }
        }

        return found;
    }

    [Benchmark]
    public int RepoHashMap()
    {
        var map = new HashMap<int, int>();
        foreach (var key in _keys)
        {
            map.Set(key, key);
        }

        var found = 0;
        foreach (var probe in _probeKeys)
        {
            if (map.HasKey(probe))
            {
                found++;
            }
        }

        return found;
    }
}
