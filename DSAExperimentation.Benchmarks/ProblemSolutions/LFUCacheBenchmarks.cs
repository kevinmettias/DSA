using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.LeetCode.LFUCache;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LFUCacheSolution's, the same factories
// LFUCacheTests proves correct. [GlobalSetup] builds one fixed call script -
// Capacity initial puts filling the cache exactly, then Capacity rounds of
// get/put over a wider key range so some calls hit (recently touched keys)
// and some miss (evicted or never-inserted keys) - the same "script
// construction charged to setup, replay is what gets measured" shape
// LRUCacheBenchmarks already uses for its own peer cache problem.
[MemoryDiagnoser]
public class LFUCacheBenchmarks
{
    private const int Seed = 460;

    private List<Func<ICache<int, int>, int?>> _script = new();

    [Params(200, 2_000)]
    public int Capacity { get; set; }

    [GlobalSetup]
    public void Setup() => _script = BuildScript(Capacity, new Random(Seed));

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
            AppendRound(script, random, capacity, roundKeyUpperBound);
        }

        return script;
    }

    // The get/put pair one measured round replays: a get of a key drawn from the wider range
    // the later rounds reach over, then a put of a fresh value under another such key.
    private static void AppendRound(
        List<Func<ICache<int, int>, int?>> script, Random random, int capacity, int roundKeyUpperBound)
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

    [Benchmark(Baseline = true)]
    public long DictionaryLinearScan() => Replay(LFUCacheSolution.CreateByDictionaryLinearScan(Capacity));

    [Benchmark]
    public long LfuCachePrimitive() => Replay(LFUCacheSolution.CreateByLfuCachePrimitive(Capacity));

    // Sums every returned get value rather than discarding it, so the JIT
    // can't eliminate the replay as dead code - the same "return the real
    // answer, not a weaker proxy" shape LRUCacheBenchmarks already follows.
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
