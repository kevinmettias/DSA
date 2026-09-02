using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Good Paths (LC 2421): the textbook approach checks every node pair
// directly - for each of the O(n^2) pairs with equal value, walk the unique tree
// path between them (BFS/DFS, O(n)) to confirm no node on it exceeds that value -
// O(n^3) worst case overall, against this repo's own DisjointSet (Union-Find):
// process edges in increasing order of their higher-valued endpoint, merging
// components in O(n * alpha(n)) total while tracking each component's max value and
// how many nodes attain it.
[MemoryDiagnoser]
public class NumberOfGoodPathsBenchmarks
{
    [Params(50, 500)]
    public int NodeCount;

    private int[] _vals = null!;
    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        // A path graph 0-1-2-...-(n-1) with values drawn from a bounded range:
        // pigeonhole guarantees repeated values along the chain, so the naive
        // approach's per-pair path walk has real same-value pairs to chase rather
        // than exiting immediately on "no duplicate value" checks.
        var random = new Random(2421); // LeetCode problem number
        _vals = Enumerable.Range(0, NodeCount).Select(_ => random.Next(1, NodeCount)).ToArray();

        var edges = new int[NodeCount - 1][];
        for (var i = 0; i < NodeCount - 1; i++)
        {
            edges[i] = [i, i + 1];
        }

        _edges = edges;
    }

    [Benchmark(Baseline = true)]
    public long PairwisePathWalk()
    {
        var n = _vals.Length;
        var adjacency = BuildAdjacency(n, _edges);
        var goodPaths = 0L;

        for (var a = 0; a < n; a++)
        {
            for (var b = a; b < n; b++)
            {
                if (a == b)
                {
                    goodPaths++;
                    continue;
                }

                if (_vals[a] == _vals[b] && IsGoodPath(adjacency, _vals, a, b))
                {
                    goodPaths++;
                }
            }
        }

        return goodPaths;
    }

    [Benchmark]
    public long DisjointSetSweep() => CountGoodPaths(_vals, _edges);

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];
        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }

    private static bool IsGoodPath(List<int>[] adjacency, int[] vals, int start, int end)
    {
        var maxAllowed = vals[start];
        var visited = new bool[adjacency.Length];
        var stack = new Stack<int>();
        stack.Push(start);
        visited[start] = true;

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (node == end)
            {
                return true;
            }

            foreach (var neighbor in adjacency[node])
            {
                if (visited[neighbor] || vals[neighbor] > maxAllowed)
                {
                    continue;
                }

                visited[neighbor] = true;
                stack.Push(neighbor);
            }
        }

        return false;
    }

    private static long CountGoodPaths(int[] vals, int[][] edges)
    {
        var n = vals.Length;
        var components = new DisjointSet(n);
        var componentMaxValue = (int[])vals.Clone();
        var componentMaxCount = new int[n];
        Array.Fill(componentMaxCount, 1);

        var goodPaths = (long)n;

        foreach (var edge in edges.OrderBy(e => Math.Max(vals[e[0]], vals[e[1]])))
        {
            var rootA = components.Find(edge[0]);
            var rootB = components.Find(edge[1]);

            if (rootA == rootB)
            {
                continue;
            }

            var maxA = componentMaxValue[rootA];
            var maxB = componentMaxValue[rootB];

            if (maxA == maxB)
            {
                goodPaths += (long)componentMaxCount[rootA] * componentMaxCount[rootB];
            }

            var mergedMax = Math.Max(maxA, maxB);
            var mergedCount = maxA == maxB
                ? componentMaxCount[rootA] + componentMaxCount[rootB]
                : maxA > maxB ? componentMaxCount[rootA] : componentMaxCount[rootB];

            components.Union(edge[0], edge[1]);
            var newRoot = components.Find(edge[0]);
            componentMaxValue[newRoot] = mergedMax;
            componentMaxCount[newRoot] = mergedCount;
        }

        return goodPaths;
    }
}
