using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Critical and Pseudo-Critical Edges in MST (LC 1489): both benchmarks run the
// identical per-edge Kruskal loop (baseline MST weight, then MST-excluding-edge, then
// MST-forcing-edge, for every edge) - the only thing that differs is how "are these
// two components already connected?" gets answered. NaiveBfsConnectivity is the
// textbook approach with no union-find at all: it keeps the edges accepted so far as
// an adjacency list and re-runs a fresh BFS from scratch for every connectivity
// check, O(V+E) each time. DisjointSetKruskal swaps that for this repo's own
// DataStructures.DisjointSet.DisjointSet - the same primitive
// Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal composes internally -
// whose path compression + union-by-rank answer the same question in O(a(n))
// amortized.
[MemoryDiagnoser]
public class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks
{
    [Params(40, 150)]
    public int NodeCount;

    private int[][] _edges = null!;
    private int[] _edgesByWeight = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1489);
        var edges = new List<int[]>();

        // A spanning chain guarantees connectivity; the extra random edges are what
        // give Kruskal real ties and cycles to resolve.
        for (var node = 1; node < NodeCount; node++)
        {
            edges.Add([node - 1, node, random.Next(1, 1_000)]);
        }

        for (var extra = 0; extra < NodeCount * 3; extra++)
        {
            var a = random.Next(NodeCount);
            var b = random.Next(NodeCount);

            if (a != b)
            {
                edges.Add([Math.Min(a, b), Math.Max(a, b), random.Next(1, 1_000)]);
            }
        }

        _edges = edges.ToArray();
        _edgesByWeight = Enumerable.Range(0, _edges.Length).OrderBy(i => _edges[i][2]).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveBfsConnectivity()
        => CountCriticalAndPseudoCritical(useDisjointSet: false);

    [Benchmark]
    public int DisjointSetKruskal()
        => CountCriticalAndPseudoCritical(useDisjointSet: true);

    private int CountCriticalAndPseudoCritical(bool useDisjointSet)
    {
        var baseline = MstWeight(useDisjointSet, skipIndex: -1, forceIndex: -1)!.Value;
        var classified = 0;

        for (var i = 0; i < _edges.Length; i++)
        {
            var withoutEdge = MstWeight(useDisjointSet, skipIndex: i, forceIndex: -1);

            if (withoutEdge is null || withoutEdge > baseline)
            {
                classified++;
                continue;
            }

            if (MstWeight(useDisjointSet, skipIndex: -1, forceIndex: i) == baseline)
            {
                classified++;
            }
        }

        return classified;
    }

    private int? MstWeight(bool useDisjointSet, int skipIndex, int forceIndex)
        => useDisjointSet
            ? DisjointSetMstWeight(skipIndex, forceIndex)
            : NaiveBfsMstWeight(skipIndex, forceIndex);

    private int? DisjointSetMstWeight(int skipIndex, int forceIndex)
    {
        var components = new DisjointSet(NodeCount);
        var totalWeight = 0;
        var edgesUsed = 0;

        if (forceIndex >= 0)
        {
            var forced = _edges[forceIndex];
            components.Union(forced[0], forced[1]);
            totalWeight += forced[2];
            edgesUsed++;
        }

        foreach (var index in _edgesByWeight)
        {
            if (index == skipIndex || index == forceIndex)
            {
                continue;
            }

            var edge = _edges[index];

            if (components.IsConnected(edge[0], edge[1]))
            {
                continue;
            }

            components.Union(edge[0], edge[1]);
            totalWeight += edge[2];
            edgesUsed++;
        }

        return edgesUsed == NodeCount - 1 ? totalWeight : null;
    }

    private int? NaiveBfsMstWeight(int skipIndex, int forceIndex)
    {
        var adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = [];
        }

        var totalWeight = 0;
        var edgesUsed = 0;

        void UseEdge(int[] edge)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
            totalWeight += edge[2];
            edgesUsed++;
        }

        bool IsConnected(int start, int target)
        {
            if (start == target)
            {
                return true;
            }

            var visited = new bool[NodeCount];
            var queue = new Queue<int>();
            visited[start] = true;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var neighbor in adjacency[current])
                {
                    if (neighbor == target)
                    {
                        return true;
                    }

                    if (!visited[neighbor])
                    {
                        visited[neighbor] = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return false;
        }

        if (forceIndex >= 0)
        {
            UseEdge(_edges[forceIndex]);
        }

        foreach (var index in _edgesByWeight)
        {
            if (index == skipIndex || index == forceIndex)
            {
                continue;
            }

            var edge = _edges[index];

            if (IsConnected(edge[0], edge[1]))
            {
                continue;
            }

            UseEdge(edge);
        }

        return edgesUsed == NodeCount - 1 ? totalWeight : null;
    }
}
