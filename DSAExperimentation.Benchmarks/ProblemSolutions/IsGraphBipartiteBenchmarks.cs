using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.IsGraphBipartite;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IsGraphBipartiteSolution's, the same methods
// IsGraphBipartiteTests proves correct - a hand-rolled iterative DFS 2-coloring
// over the problem's own int[][] adjacency (a plain sbyte[] color array, an
// explicit Stack<int>) against this repo's BipartiteCheck.IsBipartite, a
// multi-root BFS 2-coloring composed from IGraphTopology/ListChildren/
// NaturalChildOrder with a Dictionary<TNode,bool> color map. Both walk every
// node/edge exactly once at O(V+E); the split under [MemoryDiagnoser] is the
// dictionary/heap-object overhead the composed primitive pays for its generality
// against the raw array baseline. The generated graph is genuinely bipartite
// (every edge crosses a fixed A/B split) so neither strategy short-circuits on
// an early color conflict - both are forced through their full worst-case walk.
//
// Materializing the BipartiteNode graph is input construction, so it is charged
// to [GlobalSetup] and handed to the strategy's prepared-input overload.
[MemoryDiagnoser]
public class IsGraphBipartiteBenchmarks
{
    // The graph is split into exactly two sides (A and B) to stay bipartite by
    // construction.
    private const int PartitionCount = 2;

    // Extra cross-only edges added per node for density.
    private const int DensityEdgesPerNode = 2;

    [Params(200, 5_000)]
    public int NodeCount;

    private int[][] _adjacency = null!;
    private BipartiteGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var half = NodeCount / PartitionCount;
        var edges = new List<(int From, int To)>();

        AddConnectivityEdges(edges, random, half);
        AddDensityEdges(edges, random, half);

        _adjacency = BuildAdjacency(NodeCount, edges);
        _graph = BipartiteGraph.Build(_adjacency);
    }

    // Guarantee connectivity: every B-side node gets one cross edge back to a
    // random A-side node.
    private void AddConnectivityEdges(List<(int From, int To)> edges, Random random, int half)
    {
        for (var i = half; i < NodeCount; i++)
        {
            edges.Add((random.Next(half), i));
        }
    }

    // Extra cross-only edges for density - still strictly A-to-B, so the graph
    // stays bipartite by construction.
    private void AddDensityEdges(List<(int From, int To)> edges, Random random, int half)
    {
        for (var i = 0; i < NodeCount; i++)
        {
            for (var e = 0; e < DensityEdgesPerNode; e++)
            {
                var inA = i < half;
                var target = inA ? half + random.Next(NodeCount - half) : random.Next(half);
                edges.Add((i, target));
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool ColorArrayDfs() => IsGraphBipartiteSolution.IsBipartiteByColorArrayDfs(_adjacency);

    [Benchmark]
    public bool BipartiteCheckBfs() => IsGraphBipartiteSolution.IsBipartiteByBipartiteCheck(_graph);

    private static int[][] BuildAdjacency(int nodeCount, List<(int From, int To)> edges)
    {
        var adjacency = Enumerable.Range(0, nodeCount).Select(_ => new List<int>()).ToArray();

        foreach (var (from, to) in edges)
        {
            adjacency[from].Add(to);
            adjacency[to].Add(from);
        }

        return adjacency.Select(neighbors => neighbors.ToArray()).ToArray();
    }
}
