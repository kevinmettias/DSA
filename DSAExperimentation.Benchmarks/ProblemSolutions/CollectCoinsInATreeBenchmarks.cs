using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Collect Coins in a Tree (LC 2603): the coin-aware leaf trim's two arms differ only
// in how the first round finds its removable frontier - RescanUntilFixedPoint keeps
// rescanning every surviving node each round until a full pass removes nothing
// (O(n) per round, up to O(n) rounds on a path-shaped tree), while LeafQueue uses
// this repo's own Queue<int> so every zero-coin leaf is enqueued once and dequeued
// once, for one O(n) pass total. Both arms then peel two more unconditional leaf
// layers (the radius-2 free-collection rule) and report twice the surviving edge
// count.
[MemoryDiagnoser]
public class CollectCoinsInATreeBenchmarks
{
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
        var random = new Random(2603);
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
    public int RescanUntilFixedPoint()
    {
        var (adjacency, degree) = CloneGraph();
        var removed = new bool[NodeCount];

        TrimZeroCoinLeavesByRescanning(adjacency, degree, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);

        return 2 * RemainingEdgeCount(adjacency, removed);
    }

    [Benchmark]
    public int LeafQueue()
    {
        var (adjacency, degree) = CloneGraph();
        var removed = new bool[NodeCount];

        TrimZeroCoinLeavesByQueue(adjacency, degree, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);

        return 2 * RemainingEdgeCount(adjacency, removed);
    }

    // Trimming mutates degree/adjacency in place, so each invocation needs its own
    // copy - charged equally to both arms rather than to whichever ran first.
    private (List<int>[] Adjacency, int[] Degree) CloneGraph()
    {
        var adjacency = new List<int>[NodeCount];
        var degree = new int[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = _adjacency[i];
            degree[i] = _adjacency[i].Count;
        }

        return (adjacency, degree);
    }

    private void TrimZeroCoinLeavesByRescanning(List<int>[] adjacency, int[] degree, bool[] removed)
    {
        bool removedAny;

        do
        {
            removedAny = false;

            for (var node = 0; node < NodeCount; node++)
            {
                if (!removed[node] && degree[node] == 1 && _coins[node] == 0)
                {
                    RemoveNode(adjacency, degree, removed, node);
                    removedAny = true;
                }
            }
        } while (removedAny);
    }

    private void TrimZeroCoinLeavesByQueue(List<int>[] adjacency, int[] degree, bool[] removed)
    {
        var queue = new RepoQueue();

        for (var node = 0; node < NodeCount; node++)
        {
            if (degree[node] == 1 && _coins[node] == 0)
            {
                removed[node] = true;
                queue.Enqueue(node);
            }
        }

        while (queue.TryDequeue(out var node))
        {
            foreach (var neighbor in adjacency[node])
            {
                if (removed[neighbor])
                {
                    continue;
                }

                degree[neighbor]--;

                if (degree[neighbor] == 1 && _coins[neighbor] == 0)
                {
                    removed[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private static void TrimLeafLayerUnconditionally(List<int>[] adjacency, int[] degree, bool[] removed)
    {
        var leaves = new List<int>();

        for (var node = 0; node < adjacency.Length; node++)
        {
            if (!removed[node] && degree[node] <= 1)
            {
                leaves.Add(node);
            }
        }

        foreach (var leaf in leaves)
        {
            RemoveNode(adjacency, degree, removed, leaf);
        }
    }

    private static void RemoveNode(List<int>[] adjacency, int[] degree, bool[] removed, int node)
    {
        removed[node] = true;

        foreach (var neighbor in adjacency[node])
        {
            if (!removed[neighbor])
            {
                degree[neighbor]--;
            }
        }
    }

    private static int RemainingEdgeCount(List<int>[] adjacency, bool[] removed)
    {
        var edgeCount = 0;

        for (var node = 0; node < adjacency.Length; node++)
        {
            if (removed[node])
            {
                continue;
            }

            foreach (var neighbor in adjacency[node])
            {
                if (!removed[neighbor])
                {
                    edgeCount++;
                }
            }
        }

        return edgeCount / 2;
    }
}
