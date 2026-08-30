using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Critical and Pseudo-Critical Edges in MST (LC 1489): both benchmarks run the
// identical per-edge Kruskal loop (baseline MST weight, then MST-excluding-edge, then
// MST-forcing-edge, for every edge) - the only thing that differs is the union-find
// underneath it. NaiveUnionFind is a hand-rolled Find/Union with no path compression
// and no union-by-rank (Find can degrade toward O(n) as components chain together);
// DisjointSetKruskal swaps in this repo's own DataStructures.DisjointSet.DisjointSet
// - the same primitive Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal
// composes internally - whose path compression + union-by-rank keep Find/Union at
// O(a(n)) amortized.
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
    public int NaiveUnionFind()
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
            : NaiveUnionFindMstWeight(skipIndex, forceIndex);

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

    private int? NaiveUnionFindMstWeight(int skipIndex, int forceIndex)
    {
        var parent = new int[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            parent[i] = i;
        }

        int Find(int x)
        {
            while (parent[x] != x)
            {
                x = parent[x];
            }

            return x;
        }

        var totalWeight = 0;
        var edgesUsed = 0;

        void UseEdge(int[] edge)
        {
            var rootA = Find(edge[0]);
            var rootB = Find(edge[1]);

            if (rootA != rootB)
            {
                parent[rootA] = rootB;
            }

            totalWeight += edge[2];
            edgesUsed++;
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

            if (Find(edge[0]) == Find(edge[1]))
            {
                continue;
            }

            UseEdge(edge);
        }

        return edgesUsed == NodeCount - 1 ? totalWeight : null;
    }
}
