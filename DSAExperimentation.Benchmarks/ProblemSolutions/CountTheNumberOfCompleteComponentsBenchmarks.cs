using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count the Number of Complete Components (LC 2685): a BFS-collected component plus
// an O(k^2) pairwise adjacency-set scan per component (the direct reading of
// "complete") against this repo's DisjointSet, which unions every edge once and
// then checks each root's tally of nodes/edges against n*(n-1)/2 in one linear pass
// - no per-pair adjacency lookups at all. Every clique genuinely is complete (no
// early mismatch to cut either scan short), so the O(k^2) pairwise checks inside
// each fixed-size clique - not just the O(V+E) edge sweep both arms share - are
// what the gap below comes from.
[MemoryDiagnoser]
public class CountTheNumberOfCompleteComponentsBenchmarks
{
    private const int CliqueSize = 25;

    // NodeCount / CliqueSize disjoint cliques, each already complete.
    [Params(250, 2_500)]
    public int NodeCount;

    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var edges = new List<int[]>();

        for (var first = 0; first < NodeCount; first += CliqueSize)
        {
            for (var i = first; i < first + CliqueSize; i++)
            {
                for (var j = i + 1; j < first + CliqueSize; j++)
                {
                    edges.Add([i, j]);
                }
            }
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public int AdjacencySetPairwiseScan() => CountByAdjacencySetScan(NodeCount, _edges);

    [Benchmark]
    public int DisjointSetTally() => CountByDisjointSet(NodeCount, _edges);

    private static int CountByAdjacencySetScan(int n, int[][] edges)
    {
        var adjacency = new List<HashSet<int>>(n);
        for (var i = 0; i < n; i++)
        {
            adjacency.Add([]);
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        var visited = new bool[n];
        var complete = 0;

        for (var start = 0; start < n; start++)
        {
            if (visited[start])
            {
                continue;
            }

            var component = CollectComponentByBfs(start, adjacency, visited);
            if (IsCompleteByPairwiseScan(component, adjacency))
            {
                complete++;
            }
        }

        return complete;
    }

    private static List<int> CollectComponentByBfs(int start, List<HashSet<int>> adjacency, bool[] visited)
    {
        var component = new List<int>();
        var queue = new Queue<int>();
        queue.Enqueue(start);
        visited[start] = true;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            component.Add(node);

            foreach (var neighbor in adjacency[node])
            {
                if (visited[neighbor])
                {
                    continue;
                }

                visited[neighbor] = true;
                queue.Enqueue(neighbor);
            }
        }

        return component;
    }

    private static bool IsCompleteByPairwiseScan(List<int> component, List<HashSet<int>> adjacency)
    {
        for (var i = 0; i < component.Count; i++)
        {
            for (var j = i + 1; j < component.Count; j++)
            {
                if (!adjacency[component[i]].Contains(component[j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static int CountByDisjointSet(int n, int[][] edges)
    {
        var components = new DisjointSet(n);
        foreach (var edge in edges)
        {
            components.Union(edge[0], edge[1]);
        }

        var nodeCount = new int[n];
        var edgeCount = new int[n];

        for (var node = 0; node < n; node++)
        {
            nodeCount[components.Find(node)]++;
        }

        foreach (var edge in edges)
        {
            edgeCount[components.Find(edge[0])]++;
        }

        var complete = 0;
        for (var node = 0; node < n; node++)
        {
            if (components.Find(node) != node)
            {
                continue;
            }

            if (edgeCount[node] == nodeCount[node] * (nodeCount[node] - 1) / 2)
            {
                complete++;
            }
        }

        return complete;
    }
}
