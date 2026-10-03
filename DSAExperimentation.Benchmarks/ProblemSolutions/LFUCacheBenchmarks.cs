using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Cache;
using DSAExperimentation.LeetCode.LFUCache;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LFUCacheSolution's, the same factories
// LFUCacheSolutionTests proves correct. [GlobalSetup] builds one fixed call script -
// Capacity initial puts filling the cache exactly, then Capacity rounds of
// get/put over a wider key range so some calls hit (recently touched keys)
// and some miss (evicted or never-inserted keys) - the same "script
// construction charged to setup, replay is what gets measured" shape
// LRUCacheBenchmarks already uses for its own peer cache problem.
public class LFUCacheBenchmarks
{
    private const int Seed = 460;

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
    public List<int> DictionaryLinearScan() => Replay(LFUCacheSolution.CreateByDictionaryLinearScan(Capacity));

    [Benchmark]
    public List<int> LfuCachePrimitive() => Replay(LFUCacheSolution.CreateByLfuCachePrimitive(Capacity));

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
