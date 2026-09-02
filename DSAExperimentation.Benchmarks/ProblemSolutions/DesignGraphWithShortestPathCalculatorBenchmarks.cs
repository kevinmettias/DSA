using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Graph With Shortest Path Calculator (LC 2642): the design problem's real
// cost is repeated shortestPath(node1, node2) queries against a graph that can
// grow via addEdge between calls, so nothing may be cached - each query has to
// run a fresh single-source shortest-path search from node1. NaiveArrayDijkstra is
// the textbook O(V^2) per-query Dijkstra (plain distance array, linear-scan min
// extraction, no priority queue); RepoHeapDijkstra is this repo's own
// ShortestPath.Dijkstra (Collections.Heap-backed frontier, O((V+E) log V) per
// query) on the same WeightedGraphNode/WeightedGraphTopology fixture
// ShortestPathAlgorithmBenchmarks/CourseScheduleIVBenchmarks already use.
[MemoryDiagnoser]
public class DesignGraphWithShortestPathCalculatorBenchmarks
{
    private const int ExtraEdgesPerNode = 3;
    private const int RandomSeed = 2642; // LC problem number
    private const int QueryCount = 200;

    [Params(50, 200)]
    public int NodeCount;

    private List<WeightedGraphNode> _vertices = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (vertices, _) = RandomWeightedGraphs.Build(NodeCount, ExtraEdgesPerNode, RandomSeed);
        _vertices = vertices;

        var random = new Random(RandomSeed);
        _queries = Enumerable.Range(0, QueryCount).Select(_ => random.Next(NodeCount)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long NaiveArrayDijkstra()
    {
        var reached = 0L;

        foreach (var source in _queries)
        {
            reached += ArrayDijkstra(source).Count(distance => distance != int.MaxValue);
        }

        return reached;
    }

    [Benchmark]
    public long RepoHeapDijkstra()
    {
        var reached = 0L;

        foreach (var source in _queries)
        {
            var distances = ShortestPath.Dijkstra<
                WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(_vertices[source]);
            reached += distances.Count;
        }

        return reached;
    }

    // The textbook O(V^2) Dijkstra: a plain distance array plus a linear scan for
    // the next unsettled minimum, instead of the repo's Heap<T,TOrder> frontier.
    private int[] ArrayDijkstra(int source)
    {
        var distances = new int[_vertices.Count];
        Array.Fill(distances, int.MaxValue);
        distances[source] = 0;
        var settled = new bool[_vertices.Count];

        for (var iteration = 0; iteration < _vertices.Count; iteration++)
        {
            var current = ExtractMinUnsettled(distances, settled);

            if (current == -1)
            {
                break;
            }

            settled[current] = true;
            RelaxNeighbors(distances, current);
        }

        return distances;
    }

    private void RelaxNeighbors(int[] distances, int current)
    {
        foreach (var (weight, target) in _vertices[current].Edges)
        {
            var candidate = distances[current] + weight;

            if (candidate < distances[target.Id])
            {
                distances[target.Id] = candidate;
            }
        }
    }

    private static int ExtractMinUnsettled(int[] distances, bool[] settled)
    {
        var best = -1;

        for (var i = 0; i < distances.Length; i++)
        {
            if (!settled[i] && distances[i] != int.MaxValue && (best == -1 || distances[i] < distances[best]))
            {
                best = i;
            }
        }

        return best;
    }
}
