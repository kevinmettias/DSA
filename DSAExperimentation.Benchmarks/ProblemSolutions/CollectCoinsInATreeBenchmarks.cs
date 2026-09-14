using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CollectCoinsInATree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CollectCoinsInATreeSolution's, the same methods
// CollectCoinsInATreeTests proves correct. Each arm is handed the prepared adjacency
// list its hoisted overload takes, so tree construction is charged to [GlobalSetup]
// rather than to the trim being measured; the per-run degree and removal scratch is
// allocated inside the strategy, equally for both arms.
//
// The arms differ only in how the coin-aware first round finds its removable
// frontier: RescanUntilFixedPoint keeps rescanning every surviving node each round
// until a full pass removes nothing (O(n) per round, up to O(n) rounds on a
// path-shaped tree), while LeafQueue uses this repo's own Queue<int> so every
// zero-coin leaf is enqueued once and dequeued once, for one O(n) pass total.
[MemoryDiagnoser]
public class CollectCoinsInATreeBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2603;

    // Fraction of nodes that start with a coin - low enough that most leaves are
    // trimmed away in the first round, forcing real work through both arms.
    private const double CoinProbability = 0.1;

    [Params(200, 4_000)]
    public int NodeCount;

    private List<int>[] _adjacency = null!;
    private int[] _coins = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _adjacency = new List<int>[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            _adjacency[i] = [];
        }

        // A random recursive tree: each node (after the first) attaches to a
        // uniformly-chosen earlier node, giving a connected, cycle-free graph on
        // NodeCount vertices with NodeCount-1 edges - the same shape
        // MinimumHeightTreesBenchmarks.cs generates.
        for (var i = 1; i < NodeCount; i++)
        {
            var parent = random.Next(i);
            _adjacency[i].Add(parent);
            _adjacency[parent].Add(i);
        }

        _coins = new int[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            _coins[i] = random.NextDouble() < CoinProbability ? 1 : 0;
        }
    }

    [Benchmark(Baseline = true)]
    public int RescanUntilFixedPoint() =>
        CollectCoinsInATreeSolution.MinEdgesByRescan(_adjacency, _coins);

    [Benchmark]
    public int LeafQueue() =>
        CollectCoinsInATreeSolution.MinEdgesByLeafQueue(_adjacency, _coins);
}
