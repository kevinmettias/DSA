using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reachable Nodes With Restrictions (LC 2368): a depth-first flood fill from node
// 0 over an adjacency list built from the tree's edges (baseline -
// CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarks' own DFS contrast)
// against this repo's own DisjointSet unioning every edge whose endpoints are BOTH
// unrestricted, then counting how many unrestricted nodes share node 0's root. A
// random tree (each node attached to a uniformly random earlier node) with a fixed
// fraction of non-root nodes marked restricted gives both strategies a real,
// irregularly-shaped graph to walk instead of a single long chain.
[MemoryDiagnoser]
public class ReachableNodesWithRestrictionsBenchmarks
{
    private const int RandomSeed = 2368; // LC problem number
    private const double RestrictedFraction = 0.2;

    [Params(200, 5_000)]
    public int NodeCount;

    private int[][] _edges = null!;
    private bool[] _isRestricted = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _edges = BuildTreeEdges(random, NodeCount);
        _isRestricted = BuildRestricted(random, NodeCount);
    }

    private static int[][] BuildTreeEdges(Random random, int n)
    {
        var edges = new int[n - 1][];

        for (var node = 1; node < n; node++)
        {
            var parent = random.Next(node);
            edges[node - 1] = [parent, node];
        }

        return edges;
    }

    private static bool[] BuildRestricted(Random random, int n)
    {
        var isRestricted = new bool[n];

        for (var node = 1; node < n; node++)
        {
            isRestricted[node] = random.NextDouble() < RestrictedFraction;
        }

        return isRestricted;
    }

    [Benchmark(Baseline = true)]
    public int DepthFirstFloodFill()
    {
        var adjacency = BuildAdjacency();
        var visited = new bool[NodeCount];
        var stack = new Stack<int>();
        stack.Push(0);
        visited[0] = true;
        var reachable = 0;

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            reachable++;

            foreach (var next in adjacency[node])
            {
                if (!visited[next] && !_isRestricted[next])
                {
                    visited[next] = true;
                    stack.Push(next);
                }
            }
        }

        return reachable;
    }

    private int[][] BuildAdjacency()
    {
        var adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in _edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        var result = new int[NodeCount][];
        for (var i = 0; i < NodeCount; i++)
        {
            result[i] = [.. adjacency[i]];
        }

        return result;
    }

    [Benchmark]
    public int DisjointSetUnionFind()
    {
        var components = new DisjointSet(NodeCount);

        foreach (var edge in _edges)
        {
            if (!_isRestricted[edge[0]] && !_isRestricted[edge[1]])
            {
                components.Union(edge[0], edge[1]);
            }
        }

        var reachable = 0;
        for (var i = 0; i < NodeCount; i++)
        {
            if (!_isRestricted[i] && components.IsConnected(i, 0))
            {
                reachable++;
            }
        }

        return reachable;
    }
}
