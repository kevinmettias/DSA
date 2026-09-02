using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reachable Nodes In Subdivided Graph (LC 882): materializing every subdivision
// node as a real graph node and BFS-ing the whole thing (brute force - cost grows
// with the total subdivision count, which can dwarf the original graph) vs. this
// repo's own ShortestPath.Dijkstra over just the ORIGINAL graph (edge weight =
// subdivision count + 1) plus a per-edge arithmetic pass that counts each edge's
// reachable subdivision nodes analytically (ReachableNodesInSubdividedGraphTests'
// algorithm) - cost independent of how large any single edge's subdivision count is.
// Reuses RandomWeightedGraphs (ShortestPathAlgorithmBenchmarks' fixture), symmetrized
// so every edge is walkable from both endpoints, matching LC882's undirected graph.
[MemoryDiagnoser]
public class ReachableNodesInSubdividedGraphBenchmarks
{
    // LC problem number, used as the deterministic seed for graph generation.
    private const int RandomSeed = 882;

    private const int ExtraEdgesPerNode = 2;

    // Move budget scales with node count so larger graphs stay proportionally explorable.
    private const int MovesPerNodeBudget = 25;

    [Params(30, 150)]
    public int NodeCount;

    private List<WeightedGraphNode> _vertices = null!;
    private List<(int From, int To, int Cnt)> _edges = null!;
    private int _maxMoves;

    [GlobalSetup]
    public void Setup()
    {
        var (vertices, _) = RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, RandomSeed);

        // Capture the directed edge list once before symmetrizing, so every edge is
        // represented exactly once regardless of how many directions it's walkable in.
        var directedEdges = vertices
            .SelectMany(v => v.Edges.Select(e => (From: v.Id, To: e.Target.Id, Weight: e.Weight)))
            .ToList();

        foreach (var (from, to, weight) in directedEdges)
        {
            vertices[to].Edges.Add((weight, vertices[from]));
        }

        _vertices = vertices;
        _edges = directedEdges.Select(e => (e.From, e.To, Cnt: e.Weight - 1)).ToList();
        _maxMoves = NodeCount * MovesPerNodeBudget;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSubdividedBfs()
    {
        var adjacency = BuildSubdividedAdjacency();
        return CountReachableViaBfs(adjacency);
    }

    private List<List<int>> BuildSubdividedAdjacency()
    {
        var adjacency = new List<List<int>>(NodeCount);

        for (var i = 0; i < NodeCount; i++)
        {
            adjacency.Add([]);
        }

        foreach (var (from, to, cnt) in _edges)
        {
            var previous = from;

            for (var k = 0; k < cnt; k++)
            {
                previous = AppendSubdivisionNode(adjacency, previous);
            }

            Connect(adjacency, previous, to);
        }

        return adjacency;
    }

    // Appends one new subdivision node between `previous` and the rest of the chain,
    // returning the new node's id so the caller can keep threading the chain forward.
    private static int AppendSubdivisionNode(List<List<int>> adjacency, int previous)
    {
        var mid = adjacency.Count;
        adjacency.Add([]);
        Connect(adjacency, previous, mid);
        return mid;
    }

    private int CountReachableViaBfs(List<List<int>> adjacency)
    {
        var distance = new int[adjacency.Count];
        Array.Fill(distance, -1);
        distance[0] = 0;

        var queue = new Queue<int>();
        queue.Enqueue(0);
        var reachable = 0;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            reachable++;
            VisitNeighbors(adjacency, distance, queue, node);
        }

        return reachable;
    }

    private void VisitNeighbors(List<List<int>> adjacency, int[] distance, Queue<int> queue, int node)
    {
        foreach (var next in adjacency[node])
        {
            if (distance[next] != -1)
            {
                continue;
            }

            distance[next] = distance[node] + 1;

            if (distance[next] <= _maxMoves)
            {
                queue.Enqueue(next);
            }
        }
    }

    [Benchmark]
    public int DijkstraPlusAnalytic()
    {
        var distances = ShortestPath.Dijkstra<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(_vertices[0]);

        return CountReachableOriginalVertices(distances) + CountReachableSubdivisionNodes(distances);
    }

    private int CountReachableOriginalVertices(Dictionary<WeightedGraphNode, int> distances)
    {
        var reachable = 0;

        foreach (var vertex in _vertices)
        {
            if (distances.TryGetValue(vertex, out var distance) && distance <= _maxMoves)
            {
                reachable++;
            }
        }

        return reachable;
    }

    private int CountReachableSubdivisionNodes(Dictionary<WeightedGraphNode, int> distances)
    {
        var reachable = 0;

        foreach (var (from, to, cnt) in _edges)
        {
            var remainingFromStart = RemainingSubdivisionCapacity(distances, _vertices[from], cnt);
            var remainingFromEnd = RemainingSubdivisionCapacity(distances, _vertices[to], cnt);
            reachable += Math.Min(cnt, remainingFromStart + remainingFromEnd);
        }

        return reachable;
    }

    // How many of an edge's `edgeSubdivisionCount` subdivision nodes are still within
    // the move budget when walked in from `vertex`.
    private int RemainingSubdivisionCapacity(Dictionary<WeightedGraphNode, int> distances, WeightedGraphNode vertex, int edgeSubdivisionCount)
    {
        if (!distances.TryGetValue(vertex, out var distance))
        {
            return 0;
        }

        var movesLeftOnEdge = Math.Min(edgeSubdivisionCount, _maxMoves - distance);
        return Math.Max(0, movesLeftOnEdge);
    }

    private static void Connect(List<List<int>> adjacency, int a, int b)
    {
        adjacency[a].Add(b);
        adjacency[b].Add(a);
    }
}
