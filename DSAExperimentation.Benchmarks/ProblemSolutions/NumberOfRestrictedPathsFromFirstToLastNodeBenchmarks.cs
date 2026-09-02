using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Restricted Paths From First to Last Node (LC 1786): both variants share
// the same Dijkstra-computed distances-to-last-node (precomputed once in Setup, not
// timed), so the benchmarked difference is purely how restricted (Dist-decreasing)
// paths from node N are counted afterward. The graph gives each node i a "step 1"
// edge to i-1 and a "step 2" edge to i-2, both equally shortest (matching
// FibonacciBenchmarks' recurrence exactly) - so the same downstream node is reached
// two ways at every layer, making NaiveDfs's unmemoized recount genuinely
// exponential. DagFold composes this repo's own memoized fold over the identical
// (node, edges, Dist) data, turning the same walk into one Combine per distinct
// node.
[MemoryDiagnoser]
public class NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks
{
    // The graph's "step 2" edges skip one node (i to i-2) with a matching weight of 2.
    private const int TwoStepOffset = 2;

    // Kept modest (<=30), same reasoning as FibonacciBenchmarks: NaiveDfs's blowup
    // here really is O(golden-ratio^N).
    [Params(20, 30)]
    public int N;

    private RestrictedPathNode _start = null!;

    [GlobalSetup]
    public void Setup()
    {
        var nodes = Enumerable.Range(0, N + 1).Select(id => new RestrictedPathNode(id)).ToArray();

        for (var i = 1; i <= N; i++)
        {
            AddEdge(nodes[i], nodes[i - 1], weight: 1);

            if (i >= TwoStepOffset)
            {
                AddEdge(nodes[i], nodes[i - TwoStepOffset], weight: TwoStepOffset);
            }
        }

        var distances = ShortestPath.Dijkstra<
            RestrictedPathNode, RestrictedPathEdgeTopology, ListEdges<RestrictedPathNode, int>, int>(nodes[0]);

        foreach (var node in nodes)
        {
            node.Dist = distances[node];
        }

        _start = nodes[N];
    }

    private static void AddEdge(RestrictedPathNode a, RestrictedPathNode b, int weight)
    {
        a.Edges.Add((weight, b));
        b.Edges.Add((weight, a));
    }

    [Benchmark(Baseline = true)]
    public long NaiveDfs() => CountPaths(_start);

    private static long CountPaths(RestrictedPathNode node)
    {
        if (node.Dist == 0)
        {
            return 1;
        }

        var total = 0L;

        foreach (var (_, target) in node.Edges)
        {
            if (target.Dist < node.Dist)
            {
                total += CountPaths(target);
            }
        }

        return total;
    }

    [Benchmark]
    public long DagFoldMemoized()
        => DagFold.Fold<
            RestrictedPathNode, RestrictedPathChildTopology, ListChildren<RestrictedPathNode>,
            NaturalChildOrder<RestrictedPathNode, ListChildren<RestrictedPathNode>>, ListChildren<RestrictedPathNode>,
            RestrictedPathCountAlgebra, long>(_start);

    // See NumberOfRestrictedPathsFromFirstToLastNodeTests.Fixtures for the full
    // explanation - repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own copy
    // of the solution rather than depending on the Tests project.
    private sealed class RestrictedPathNode(int id)
    {
        public int Id { get; } = id;

        public List<(int Weight, RestrictedPathNode Target)> Edges { get; } = [];

        public int Dist { get; set; }
    }

    private readonly struct RestrictedPathEdgeTopology
        : IEdgeTopology<RestrictedPathNode, ListEdges<RestrictedPathNode, int>, int>
    {
        public static ListEdges<RestrictedPathNode, int> GetEdges(RestrictedPathNode node) => new(node.Edges);
    }

    private readonly struct RestrictedPathChildTopology
        : IDagTopology<RestrictedPathNode, ListChildren<RestrictedPathNode>>
    {
        public static ListChildren<RestrictedPathNode> GetChildren(RestrictedPathNode node)
        {
            var children = new List<RestrictedPathNode>();

            foreach (var (_, target) in node.Edges)
            {
                if (target.Dist < node.Dist)
                {
                    children.Add(target);
                }
            }

            return new ListChildren<RestrictedPathNode>(children);
        }
    }

    private readonly struct RestrictedPathCountAlgebra : IFoldAlgebra<RestrictedPathNode, long>
    {
        private const long Modulo = 1_000_000_007;

        public static long Empty => 0;

        public static long Combine(RestrictedPathNode node, IReadOnlyList<long> children)
        {
            if (node.Dist == 0)
            {
                return 1;
            }

            var total = 0L;

            foreach (var child in children)
            {
                total = (total + child) % Modulo;
            }

            return total;
        }
    }
}
