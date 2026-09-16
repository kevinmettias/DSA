using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
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

    private List<Func<ICache<int, int>, int?>> _script = new();

    [Params(200, 2_000)]
    public int Capacity { get; set; }

    [GlobalSetup]
    public void Setup() => _script = CacheReplayWorkloads.Build(Capacity, Seed);

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
}
