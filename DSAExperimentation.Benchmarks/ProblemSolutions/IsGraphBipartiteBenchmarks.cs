using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using BipartiteCheckOperations = DSAExperimentation.Algorithms.Bipartiteness.BipartiteCheck;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Is Graph Bipartite? (LC 785): a hand-rolled iterative DFS 2-coloring directly
// over the problem's own int[][] adjacency (a plain sbyte[] color array, an
// explicit Stack<int>) against this repo's BipartiteCheck.IsBipartite - a
// multi-root BFS 2-coloring composed from IGraphTopology/ListChildren/
// NaturalChildOrder with a Dictionary<TNode,bool> color map. Both walk every
// node/edge exactly once at O(V+E); the split under [MemoryDiagnoser] is the
// dictionary/heap-object overhead the composed primitive pays for its generality
// against the raw array baseline. The generated graph is genuinely bipartite
// (every edge crosses a fixed A/B split) so neither strategy short-circuits on
// an early color conflict - both are forced through their full worst-case walk.
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
    private List<GraphNode> _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var half = NodeCount / PartitionCount;
        var edges = new List<(int From, int To)>();

        AddConnectivityEdges(edges, random, half);
        AddDensityEdges(edges, random, half);

        _adjacency = BuildAdjacency(NodeCount, edges);
        _nodes = BuildGraphNodes(NodeCount, edges);
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
    public bool ArrayAdjacencyIterativeDfs()
    {
        var color = new sbyte[_adjacency.Length];
        var stack = new Stack<int>();

        for (var start = 0; start < _adjacency.Length; start++)
        {
            if (color[start] != 0)
            {
                continue;
            }

            color[start] = 1;
            stack.Push(start);

            if (!Walk(stack, color))
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool BipartiteCheckBfs()
        => BipartiteCheckOperations.IsBipartite<
            GraphNode, GraphNodeTopology, ListChildren<GraphNode>,
            NaturalChildOrder<GraphNode, ListChildren<GraphNode>>, ListChildren<GraphNode>>(
            _nodes);

    private bool Walk(Stack<int> stack, sbyte[] color)
    {
        while (stack.Count > 0)
        {
            var node = stack.Pop();

            foreach (var neighbor in _adjacency[node])
            {
                if (color[neighbor] == color[node])
                {
                    return false;
                }

                if (color[neighbor] == 0)
                {
                    color[neighbor] = (sbyte)-color[node];
                    stack.Push(neighbor);
                }
            }
        }

        return true;
    }

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

    private static List<GraphNode> BuildGraphNodes(int nodeCount, List<(int From, int To)> edges)
    {
        var nodes = Enumerable.Range(0, nodeCount).Select(id => new GraphNode(id)).ToList();

        foreach (var (from, to) in edges)
        {
            nodes[from].Neighbors.Add(nodes[to]);
            nodes[to].Neighbors.Add(nodes[from]);
        }

        return nodes;
    }

    // See IsGraphBipartiteTests.Fixtures for the full explanation - repeated here
    // rather than shared because TwoSumBenchmarks/MedianOfTwoSortedArraysBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private sealed class GraphNode(int id)
    {
        public int Id { get; } = id;

        public List<GraphNode> Neighbors { get; } = [];
    }

    private readonly struct GraphNodeTopology : IGraphTopology<GraphNode, ListChildren<GraphNode>>
    {
        public static ListChildren<GraphNode> GetChildren(GraphNode node) => new(node.Neighbors);
    }
}
