using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.LeetCode.LRUCache;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LRUCacheSolution's, the same factories
// LRUCacheSolutionTests proves correct. [GlobalSetup] builds one fixed call script -
// Capacity initial puts filling the cache exactly, then Capacity rounds of
// get/put over a wider key range so some calls hit (recently touched keys)
// and some miss (evicted or never-inserted keys) - the same "script
// construction charged to setup, replay is what gets measured" shape
// DesignTaskManagerBenchmarks already uses for its own instance-API problem.
public class LRUCacheBenchmarks
{
    private const int Seed = 146;

    private List<Func<ICache<int, int>, int?>> _script = new();

    // What every get returned, in script order; a put returns nothing and adds nothing.
    private List<int> _gets = [];

    [Params(200, 2_000)]
    public int Capacity { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _script = CacheReplayWorkloads.Build(Capacity, Seed);
        _gets = new List<int>(_script.Count);
    }

    [Benchmark(Baseline = true)]
    public List<int> DictionaryLinkedList() => Replay(LRUCacheSolution.CreateByDictionaryLinkedList(Capacity));

    [Benchmark]
    public List<int> LruCachePrimitive() => Replay(LRUCacheSolution.CreateByLruCachePrimitive(Capacity));

    // Returns every get's answer in order - the replay's whole observable output.
    private List<int> Replay(ICache<int, int> cache)
    {
        _gets.Clear();

        foreach (var op in _script)
        {
            if (op(cache) is { } value)
            {
                _gets.Add(value);
            }
        }

        return _gets;
    }
}
