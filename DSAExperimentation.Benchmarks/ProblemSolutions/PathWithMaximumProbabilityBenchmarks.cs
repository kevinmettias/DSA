using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Path with Maximum Probability (LC 1514): the textbook exhaustive DFS over
// every source-to-target path (exponential in NodeCount - every edge
// branches the path count) against this repo's own ShortestPath.Dijkstra
// run over the same edges reweighted to -log(probability), the non-negative
// transform PathWithMaximumProbabilityTests documents, which turns
// "maximize a product" into Dijkstra's own "minimize a non-negative sum"
// with zero changes to Dijkstra itself. Edges only ever point from a lower
// id to a higher one (mirrors RandomWeightedGraphs' own "back edge" trick,
// applied here in the forward direction), so the graph is acyclic and
// NaiveDfs needs no visited set - it still explores every one of the
// exponentially many paths, it just never has to guard against revisits.
[MemoryDiagnoser]
public class PathWithMaximumProbabilityBenchmarks
{
    [Params(10, 14)]
    public int NodeCount;

    private List<(double Probability, int To)>[] _adjacency = null!;
    private ProbabilityNode[] _nodes = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        _target = NodeCount - 1;
        var random = new Random(1514);

        _nodes = Enumerable.Range(0, NodeCount).Select(id => new ProbabilityNode(id)).ToArray();
        _adjacency = Enumerable.Range(0, NodeCount).Select(_ => new List<(double, int)>()).ToArray();

        // Guarantees node 0 reaches every node: each node i > 0 gets one
        // forward edge from an earlier, already-reachable node j < i.
        for (var i = 1; i < NodeCount; i++)
        {
            AddEdge(random.Next(i), i, random);
        }

        // Extra forward edges per node for branching density, so NaiveDfs
        // actually explores an exponential number of distinct paths.
        for (var i = 0; i < NodeCount - 1; i++)
        {
            for (var e = 0; e < 2; e++)
            {
                AddEdge(i, random.Next(i + 1, NodeCount), random);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public double NaiveDfsOverEveryPath()
    {
        var best = 0.0;
        Dfs(0, 1.0, ref best);
        return best;
    }

    [Benchmark]
    public double DijkstraOverNegativeLogWeights()
    {
        var distances = ShortestPath.Dijkstra<
            ProbabilityNode, ProbabilityTopology, ListEdges<ProbabilityNode, double>, double>(_nodes[0]);

        return distances.TryGetValue(_nodes[_target], out var cost) ? Math.Exp(-cost) : 0.0;
    }

    private void Dfs(int node, double productSoFar, ref double best)
    {
        if (node == _target)
        {
            best = Math.Max(best, productSoFar);
        }

        foreach (var (probability, to) in _adjacency[node])
        {
            Dfs(to, productSoFar * probability, ref best);
        }
    }

    private void AddEdge(int from, int to, Random random)
    {
        var probability = 0.5 + (random.NextDouble() * 0.49); // (0.5, 0.99]
        _adjacency[from].Add((probability, to));
        _nodes[from].Edges.Add((-Math.Log(probability), _nodes[to]));
    }

    // Own copy of the solution rather than depending on the Tests project -
    // see CheapestFlightsWithinKStopsBenchmarks' own FlightState for the
    // precedent this mirrors.
    private sealed class ProbabilityNode(int id)
    {
        public int Id { get; } = id;

        public List<(double Cost, ProbabilityNode Target)> Edges { get; } = [];
    }

    private readonly struct ProbabilityTopology
        : IEdgeTopology<ProbabilityNode, ListEdges<ProbabilityNode, double>, double>
    {
        public static ListEdges<ProbabilityNode, double> GetEdges(ProbabilityNode node) => new(node.Edges);
    }
}
