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

    private List<int>[] _adjacency = [];

    private int[] _coins = [];
    [Params(200, 4_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _adjacency = BuildRandomTree(random, NodeCount);
        _coins = TossCoins(random, NodeCount);
    }

    // A random recursive tree: each node (after the first) attaches to a
    // uniformly-chosen earlier node, giving a connected, cycle-free graph on
    // NodeCount vertices with NodeCount-1 edges - the same shape
    // MinimumHeightTreesBenchmarks.cs generates.
    private static List<int>[] BuildRandomTree(Random random, int nodeCount)
    {
        var adjacency = new List<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        for (var i = 1; i < nodeCount; i++)
        {
            var parent = random.Next(i);
            adjacency[i].Add(parent);
            adjacency[parent].Add(i);
        }

        return adjacency;
    }

    // One entry per node, a coin iff the draw falls under CoinProbability - low
    // enough that most leaves are trimmed away in the first round, forcing real work
    // through both arms.
    private static int[] TossCoins(Random random, int nodeCount)
    {
        var coins = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            var hasCoin = random.NextDouble() < CoinProbability;
            coins[i] = hasCoin ? 1 : 0;
        }

        return coins;
    }

    [Benchmark(Baseline = true)]
    public int RescanUntilFixedPoint() =>
        CollectCoinsInATreeSolution.MinEdgesByRescan(_adjacency, _coins);

    [Benchmark]
    public int LeafQueue() =>
        CollectCoinsInATreeSolution.MinEdgesByLeafQueue(_adjacency, _coins);
}
