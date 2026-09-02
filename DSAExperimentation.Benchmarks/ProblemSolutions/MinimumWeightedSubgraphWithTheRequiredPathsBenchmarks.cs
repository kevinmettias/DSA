using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Weighted Subgraph With the Required Paths (LC 2203): querying every
// candidate meeting node with three fresh point-to-point ShortestPath.AStar calls
// (src1->node, src2->node, node->dest) against caching the whole graph's distances
// with exactly three ShortestPath.Dijkstra runs - forward from src1, forward from
// src2, and one on the edge-reversed graph from dest, since "distance to dest" is
// "distance from dest on the reverse graph" - then scanning every candidate's
// already-computed distances. The reverse-graph trick turns O(V) independent
// shortest-path searches into O(1) dictionary lookups after a single search each.
[MemoryDiagnoser]
public class MinimumWeightedSubgraphWithTheRequiredPathsBenchmarks
{
    private const int RandomSeed = 2203;
    private const int ExtraEdgesPerNode = 3;
    private const int MaxEdgeWeightExclusive = 50;

    [Params(30, 150)]
    public int NodeCount;

    private List<WeightedGraphNode> _forward = null!;
    private List<WeightedGraphNode> _reverse = null!;
    private int _src1;
    private int _src2;
    private int _dest;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _forward = BuildForwardGraph(NodeCount, random);
        _reverse = BuildReverseGraph(_forward);
        _src1 = 0;
        _src2 = 1;
        _dest = NodeCount - 1;
    }

    // Node 0 gets the usual "back edge per node" spanning structure so it reaches
    // everyone; node 1 additionally gets its own spread of forward edges so the
    // second source has real, non-trivial reach too, not just whatever the back-edge
    // chain happens to grant it.
    private static List<WeightedGraphNode> BuildForwardGraph(int nodeCount, Random random)
    {
        var nodes = Enumerable.Range(0, nodeCount).Select(id => new WeightedGraphNode(id)).ToList();

        AddSpanningBackEdges(nodes, random);
        AddSecondSourceSpread(nodes, random);
        AddRandomExtraEdges(nodes, random);

        return nodes;
    }

    private static void AddSpanningBackEdges(List<WeightedGraphNode> nodes, Random random)
    {
        for (var i = 1; i < nodes.Count; i++)
        {
            AddEdge(nodes[random.Next(i)], nodes[i], random);
        }
    }

    private static void AddSecondSourceSpread(List<WeightedGraphNode> nodes, Random random)
    {
        for (var e = 0; e < ExtraEdgesPerNode; e++)
        {
            AddEdge(nodes[1], nodes[random.Next(nodes.Count)], random);
        }
    }

    private static void AddRandomExtraEdges(List<WeightedGraphNode> nodes, Random random)
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            for (var e = 0; e < ExtraEdgesPerNode; e++)
            {
                var target = random.Next(nodes.Count);

                if (target != i)
                {
                    AddEdge(nodes[i], nodes[target], random);
                }
            }
        }
    }

    private static void AddEdge(WeightedGraphNode from, WeightedGraphNode to, Random random)
        => from.Edges.Add((random.Next(1, MaxEdgeWeightExclusive), to));

    private static List<WeightedGraphNode> BuildReverseGraph(List<WeightedGraphNode> forward)
    {
        var reverse = forward.Select(node => new WeightedGraphNode(node.Id)).ToList();

        foreach (var node in forward)
        {
            foreach (var (weight, target) in node.Edges)
            {
                reverse[target.Id].Edges.Add((weight, reverse[node.Id]));
            }
        }

        return reverse;
    }

    [Benchmark(Baseline = true)]
    public long PerNodePointToPointAStar()
    {
        var best = long.MaxValue;

        foreach (var node in _forward)
        {
            var d1 = AStar(_forward[_src1], node);
            var d2 = AStar(_forward[_src2], node);
            var d3 = AStar(node, _forward[_dest]);

            if (d1 is not null && d2 is not null && d3 is not null)
            {
                best = Math.Min(best, (long)d1 + d2.Value + d3.Value);
            }
        }

        return best == long.MaxValue ? -1 : best;
    }

    private static int? AStar(WeightedGraphNode source, WeightedGraphNode target)
        => ShortestPath.AStar<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int,
            ZeroHeuristic<WeightedGraphNode, int>>(source, target);

    [Benchmark]
    public long SingleSourceDijkstraReuse()
    {
        var fromSrc1 = Dijkstra(_forward[_src1]);
        var fromSrc2 = Dijkstra(_forward[_src2]);
        var toDest = Dijkstra(_reverse[_dest]);

        var best = long.MaxValue;

        for (var id = 0; id < _forward.Count; id++)
        {
            if (fromSrc1.TryGetValue(_forward[id], out var d1) &&
                fromSrc2.TryGetValue(_forward[id], out var d2) &&
                toDest.TryGetValue(_reverse[id], out var d3))
            {
                best = Math.Min(best, (long)d1 + d2 + d3);
            }
        }

        return best == long.MaxValue ? -1 : best;
    }

    private static Dictionary<WeightedGraphNode, int> Dijkstra(WeightedGraphNode source)
        => ShortestPath.Dijkstra<WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(source);
}
