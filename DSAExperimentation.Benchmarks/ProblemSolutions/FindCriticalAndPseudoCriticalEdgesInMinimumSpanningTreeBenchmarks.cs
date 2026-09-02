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
    // 1489 is the LeetCode problem number, reused here as a fixed benchmark seed.
    private const int RandomSeed = 1489;
    private const int MaxEdgeWeight = 1_000;
    private const int ExtraEdgeMultiplier = 3;
    private const int WeightIndex = 2;

    [Params(40, 150)]
    public int NodeCount;

    private int[][] _edges = null!;
    private int[] _edgesByWeight = null!;

    // Bundles the edge index to skip (excluded-edge probe) and the edge index to force
    // (forced-edge probe) that CountCriticalAndPseudoCritical threads through both MST
    // weight strategies for a single edge classification.
    private readonly record struct EdgeSelection(int SkipIndex, int ForceIndex);

    // Mutable running total shared by both MST weight strategies' edge-accumulation
    // helpers - a class (not a struct) so helper methods can mutate the caller's totals
    // without ref parameters.
    private sealed class WeightAccumulator
    {
        public int TotalWeight;
        public int EdgesUsed;
    }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edges = new List<int[]>();

        AddSpanningChainEdges(edges, random);
        AddExtraRandomEdges(edges, random);

        _edges = edges.ToArray();
        _edgesByWeight = Enumerable.Range(0, _edges.Length).OrderBy(i => _edges[i][WeightIndex]).ToArray();
    }

    // A spanning chain guarantees connectivity; the extra random edges (added by
    // AddExtraRandomEdges) are what give Kruskal real ties and cycles to resolve.
    private void AddSpanningChainEdges(List<int[]> edges, Random random)
    {
        for (var node = 1; node < NodeCount; node++)
        {
            edges.Add([node - 1, node, random.Next(1, MaxEdgeWeight)]);
        }
    }

    private void AddExtraRandomEdges(List<int[]> edges, Random random)
    {
        for (var extra = 0; extra < NodeCount * ExtraEdgeMultiplier; extra++)
        {
            var a = random.Next(NodeCount);
            var b = random.Next(NodeCount);

            if (a != b)
            {
                edges.Add([Math.Min(a, b), Math.Max(a, b), random.Next(1, MaxEdgeWeight)]);
            }
        }
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
        var accumulator = AccumulateDisjointSetWeight(components, new EdgeSelection(skipIndex, forceIndex));

        return accumulator.EdgesUsed == NodeCount - 1 ? accumulator.TotalWeight : null;
    }

    private WeightAccumulator AccumulateDisjointSetWeight(DisjointSet components, EdgeSelection selection)
    {
        var accumulator = new WeightAccumulator();

        if (selection.ForceIndex >= 0)
        {
            UnionWeightedEdge(components, _edges[selection.ForceIndex], accumulator);
        }

        foreach (var index in _edgesByWeight)
        {
            TryUnionEdge(components, index, selection, accumulator);
        }

        return accumulator;
    }

    private void TryUnionEdge(DisjointSet components, int index, EdgeSelection selection, WeightAccumulator accumulator)
    {
        if (index == selection.SkipIndex || index == selection.ForceIndex)
        {
            return;
        }

        var edge = _edges[index];

        if (components.IsConnected(edge[0], edge[1]))
        {
            return;
        }

        UnionWeightedEdge(components, edge, accumulator);
    }

    private void UnionWeightedEdge(DisjointSet components, int[] edge, WeightAccumulator accumulator)
    {
        components.Union(edge[0], edge[1]);
        accumulator.TotalWeight += edge[WeightIndex];
        accumulator.EdgesUsed++;
    }

    private int? NaiveBfsMstWeight(int skipIndex, int forceIndex)
    {
        var adjacency = BuildBfsAdjacency();
        var accumulator = AccumulateBfsSpanningWeight(adjacency, new EdgeSelection(skipIndex, forceIndex));

        return accumulator.EdgesUsed == NodeCount - 1 ? accumulator.TotalWeight : null;
    }

    private List<int>[] BuildBfsAdjacency()
    {
        var adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            adjacency[i] = [];
        }

        return adjacency;
    }

    private WeightAccumulator AccumulateBfsSpanningWeight(List<int>[] adjacency, EdgeSelection selection)
    {
        var accumulator = new WeightAccumulator();

        if (selection.ForceIndex >= 0)
        {
            UseBfsEdge(adjacency, _edges[selection.ForceIndex], accumulator);
        }

        foreach (var index in _edgesByWeight)
        {
            TryUseBfsEdge(adjacency, index, selection, accumulator);
        }

        return accumulator;
    }

    private void TryUseBfsEdge(List<int>[] adjacency, int index, EdgeSelection selection, WeightAccumulator accumulator)
    {
        if (index == selection.SkipIndex || index == selection.ForceIndex)
        {
            return;
        }

        var edge = _edges[index];

        if (ReachableViaBfs(adjacency, edge[0], edge[1]))
        {
            return;
        }

        UseBfsEdge(adjacency, edge, accumulator);
    }

    private void UseBfsEdge(List<int>[] adjacency, int[] edge, WeightAccumulator accumulator)
    {
        adjacency[edge[0]].Add(edge[1]);
        adjacency[edge[1]].Add(edge[0]);
        accumulator.TotalWeight += edge[WeightIndex];
        accumulator.EdgesUsed++;
    }

    private static bool ReachableViaBfs(List<int>[] adjacency, int start, int target)
    {
        if (start == target)
        {
            return true;
        }

        var visited = new bool[adjacency.Length];
        var queue = new Queue<int>();
        visited[start] = true;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            if (VisitNeighbors(adjacency, queue, visited, target))
            {
                return true;
            }
        }

        return false;
    }

    private static bool VisitNeighbors(List<int>[] adjacency, Queue<int> queue, bool[] visited, int target)
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

        return false;
    }
}
