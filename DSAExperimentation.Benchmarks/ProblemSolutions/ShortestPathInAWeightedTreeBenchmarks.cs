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

    [Params(200, 2_000)]
    public int NodeCount;

    private int _n;
    private int[][] _edges = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _n = NodeCount;
        _edges = new int[_n - 1][];

        for (var i = 1; i < _n; i++)
        {
            var parent = random.Next(0, i);
            _edges[i - 1] = [parent + 1, i + 1, random.Next(1, MaxWeight + 1)];
        }

        _queries = new int[_n][];

        for (var i = 0; i < _n; i++)
        {
            if (_edges.Length > 0 && random.Next(2) == 0)
            {
                var edge = _edges[random.Next(_edges.Length)];
                _queries[i] = [1, edge[0], edge[1], random.Next(1, MaxWeight + 1)];
            }
            else
            {
                _queries[i] = [2, random.Next(1, _n + 1)];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceBfs() =>
        ShortestPathInAWeightedTreeSolution.ShortestPathQueriesByBruteForceBfs(_n, _edges, _queries);

    [Benchmark]
    public int[] EulerFenwick() =>
        ShortestPathInAWeightedTreeSolution.ShortestPathQueriesByEulerFenwick(_n, _edges, _queries);
}
