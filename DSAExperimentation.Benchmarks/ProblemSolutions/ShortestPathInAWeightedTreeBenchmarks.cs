using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShortestPathInAWeightedTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestPathInAWeightedTreeSolution's, the same
// methods ShortestPathInAWeightedTreeTests proves correct. Each node i > 0
// attaches to a uniformly random earlier node, the same randomized-parent shape
// MaximumPointsAfterCollectingCoinsFromAllNodesBenchmarks' Setup comment
// contrasts with a worst-case chain - here it is deliberate rather than a
// simplification, since a bushy tree keeps most Euler-tour subtree ranges short
// while the brute-force BFS arm still pays for every node on every [2, x] query
// regardless of shape.
[MemoryDiagnoser]
public class ShortestPathInAWeightedTreeBenchmarks
{
    private const int RandomSeed = 3515; // LeetCode problem number
    private const int MaxWeight = 10_000;

    private int _nodeCount;

    private int[][] _edges = [];
    private int[][] _queries = [];
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nodeCount = NodeCount;
        _edges = BuildEdges(_nodeCount, random);
        _queries = BuildQueries(_nodeCount, _edges, random);
    }

    // Each node i > 0 attaches to a uniformly random earlier node, so node i + 1 is
    // always parented to something in [1, i] that already exists.
    private static int[][] BuildEdges(int nodeCount, Random random)
    {
        var edges = new int[nodeCount - 1][];

        for (var i = 1; i < nodeCount; i++)
        {
            var parent = random.Next(0, i);
            edges[i - 1] = [parent + 1, i + 1, random.Next(1, MaxWeight + 1)];
        }

        return edges;
    }

    // A query is either a weight update on an edge the tree actually has, or the
    // [2, x] distance query the BFS arm pays for in full.
    private static int[][] BuildQueries(int nodeCount, int[][] edges, Random random)
    {
        var queries = new int[nodeCount][];

        for (var i = 0; i < nodeCount; i++)
        {
            if (edges.Length > 0 && random.Next(2) == 0)
            {
                var edge = edges[random.Next(edges.Length)];
                queries[i] = [1, edge[0], edge[1], random.Next(1, MaxWeight + 1)];
            }
            else
            {
                queries[i] = [2, random.Next(1, nodeCount + 1)];
            }
        }

        return queries;
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceBfs() =>
        ShortestPathInAWeightedTreeSolution.ShortestPathQueriesByBruteForceBfs(_nodeCount, _edges, _queries);

    [Benchmark]
    public int[] EulerFenwick() =>
        ShortestPathInAWeightedTreeSolution.ShortestPathQueriesByEulerFenwick(_nodeCount, _edges, _queries);
}
