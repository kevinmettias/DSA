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
    [Params(30, 150)]
    public int NodeCount;

    private List<WeightedGraphNode> _vertices = null!;
    private List<(int From, int To, int Cnt)> _edges = null!;
    private int _maxMoves;

    [GlobalSetup]
    public void Setup()
    {
        var (vertices, _) = RandomWeightedGraphs.Build(NodeCount, extraEdgesPerNode: 2, seed: 882);

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
        _maxMoves = NodeCount * 25;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSubdividedBfs()
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
                var mid = adjacency.Count;
                adjacency.Add([]);
                Connect(adjacency, previous, mid);
                previous = mid;
            }

            Connect(adjacency, previous, to);
        }

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

        return reachable;
    }

    [Benchmark]
    public int DijkstraPlusAnalytic()
    {
        var distances = ShortestPath.Dijkstra<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(_vertices[0]);

        var reachable = 0;

        foreach (var vertex in _vertices)
        {
            if (distances.TryGetValue(vertex, out var distance) && distance <= _maxMoves)
            {
                reachable++;
            }
        }

        foreach (var (from, to, cnt) in _edges)
        {
            var fromU = distances.TryGetValue(_vertices[from], out var du)
                ? Math.Max(0, Math.Min(cnt, _maxMoves - du))
                : 0;
            var fromV = distances.TryGetValue(_vertices[to], out var dv)
                ? Math.Max(0, Math.Min(cnt, _maxMoves - dv))
                : 0;
            reachable += Math.Min(cnt, fromU + fromV);
        }

        return reachable;
    }

    private static void Connect(List<List<int>> adjacency, int a, int b)
    {
        adjacency[a].Add(b);
        adjacency[b].Add(a);
    }
}
