using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Unreachable Pairs of Nodes in an Undirected Graph (LC 2316): a DFS flood
// fill over an adjacency list built from the edges (baseline -
// NumberOfOperationsToMakeNetworkConnectedBenchmarks' own DFS-vs-Union-Find
// contrast, applied here to summed component sizes instead of a component count)
// vs. this repo's own DisjointSet unioning every edge and tallying component sizes
// through a HashMap<root,size>. The graph is built as several disjoint chains
// (not one connected component) so both strategies have multiple real components
// to discover and size.
[MemoryDiagnoser]
public class CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarks
{
    private const int RandomSeed = 2316;
    private const int NodesPerComponent = 25;

    [Params(200, 2_000)]
    public int NodeCount;

    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var segmentSize = Math.Max(2, Math.Min(NodesPerComponent, NodeCount));
        var edges = new List<int[]>();

        for (var node = 1; node < NodeCount; node++)
        {
            if (node % segmentSize != 0)
            {
                edges.Add([node - 1, node]);
            }
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public long DepthFirstFloodFill()
    {
        var adjacency = BuildAdjacency();
        var visited = new bool[NodeCount];
        var totalPairs = (long)NodeCount * (NodeCount - 1) / 2;
        var reachablePairs = 0L;

        for (var i = 0; i < NodeCount; i++)
        {
            if (visited[i])
            {
                continue;
            }

            var size = Visit(i, adjacency, visited);
            reachablePairs += (long)size * (size - 1) / 2;
        }

        return totalPairs - reachablePairs;
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

    private static int Visit(int start, int[][] adjacency, bool[] visited)
    {
        var stack = new Stack<int>();
        stack.Push(start);
        visited[start] = true;
        var size = 0;

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            size++;

            foreach (var next in adjacency[node])
            {
                if (!visited[next])
                {
                    visited[next] = true;
                    stack.Push(next);
                }
            }
        }

        return size;
    }

    [Benchmark]
    public long DisjointSetUnionFind()
    {
        var components = new DisjointSet(NodeCount);
        foreach (var edge in _edges)
        {
            components.Union(edge[0], edge[1]);
        }

        var sizeByRoot = new HashMap<int, long>();
        for (var i = 0; i < NodeCount; i++)
        {
            var root = components.Find(i);
            sizeByRoot.TryGetValue(root, out var size);
            sizeByRoot.Set(root, size + 1);
        }

        var totalPairs = (long)NodeCount * (NodeCount - 1) / 2;
        var reachablePairs = 0L;

        foreach (var size in sizeByRoot.Values)
        {
            reachablePairs += size * (size - 1) / 2;
        }

        return totalPairs - reachablePairs;
    }
}
