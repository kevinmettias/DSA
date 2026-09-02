using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.LeetCode.LRUCache;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LRUCacheSolution's, the same factories
// LRUCacheTests proves correct. [GlobalSetup] builds one fixed call script -
// Capacity initial puts filling the cache exactly, then Capacity rounds of
// get/put over a wider key range so some calls hit (recently touched keys)
// and some miss (evicted or never-inserted keys) - the same "script
// construction charged to setup, replay is what gets measured" shape
// DesignTaskManagerBenchmarks already uses for its own instance-API problem.
[MemoryDiagnoser]
public class LRUCacheBenchmarks
{
    private const int Seed = 146;

    [Params(200, 2_000)]
    public int Capacity;

    private List<Func<ICache<int, int>, int?>> _script = null!;

    [GlobalSetup]
    public void Setup() => _script = BuildScript(Capacity, new Random(Seed));

    [Benchmark(Baseline = true)]
    public long DictionaryLinkedList() => Replay(LRUCacheSolution.CreateByDictionaryLinkedList(Capacity));

    [Benchmark]
    public long LruCachePrimitive() => Replay(LRUCacheSolution.CreateByLruCachePrimitive(Capacity));

    // Sums every returned get value rather than discarding it, so the JIT
    // can't eliminate the replay as dead code - the same "return the real
    // answer, not a weaker proxy" shape OpenTheLockBenchmarks/
    // DesignTaskManagerBenchmarks already follow.
    private long Replay(ICache<int, int> cache)
    {
        var executedValueSum = 0L;

        foreach (var op in _script)
        {
            executedValueSum += op(cache) ?? 0;
        }

        return executedValueSum;
    }

    private static List<Func<ICache<int, int>, int?>> BuildScript(int capacity, Random random)
    {
        var script = new List<Func<ICache<int, int>, int?>>();

        for (var key = 0; key < capacity; key++)
        {
            var value = random.Next(0, capacity);
            script.Add(cache =>
            {
                cache.Set(key, value);
                return null;
            });
        }

        var roundKeyUpperBound = capacity * 2;

        for (var round = 0; round < capacity; round++)
        {
            var getKey = random.Next(0, roundKeyUpperBound);
            script.Add(cache => cache.TryGetValue(getKey, out var value) ? value : -1);

            var putKey = random.Next(0, roundKeyUpperBound);
            var putValue = random.Next(0, capacity);
            script.Add(cache =>
            {
                cache.Set(putKey, putValue);
                return null;
            });
        }

        return script;
    }
}
