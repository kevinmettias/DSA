using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Engines.Dags;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Ways to Arrive at Destination (LC 1976): both variants share the same
// Dijkstra-computed distances-to-destination (precomputed once in Setup, not timed
// - NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks' own precedent for this
// split), so the benchmarked difference is purely how shortest paths from node 0
// are counted afterward. The graph gives each node i a "step 1" edge to i-1 and a
// "step 2" edge to i-2, both tying for shortest distance i (matching
// NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks' own recurrence exactly) -
// so the same downstream node is reached two ways at every layer, making
// NaiveDfs's unmemoized recount genuinely exponential (Fibonacci-shaped). DagFold
// composes this repo's own memoized fold over the identical (node, edges, Dist)
// data, turning the same walk into one Combine per distinct node.
[MemoryDiagnoser]
public class NumberOfWaysToArriveAtDestinationBenchmarks
{
    // The graph's "step 2" edges skip one node (i to i-2) with a matching weight of 2.
    private const int TwoStepOffset = 2;

    // Kept modest (<=30), same reasoning as
    // NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks: NaiveDfs's blowup here
    // really is O(golden-ratio^N).
    [Params(20, 30)]
    public int N;

    private WaysNode _start = null!;

    [GlobalSetup]
    public void Setup()
    {
        var nodes = Enumerable.Range(0, N + 1).Select(id => new WaysNode(id)).ToArray();

        for (var i = 1; i <= N; i++)
        {
            AddEdge(nodes[i], nodes[i - 1], weight: 1);

            if (i >= TwoStepOffset)
            {
                AddEdge(nodes[i], nodes[i - TwoStepOffset], weight: TwoStepOffset);
            }
        }

        var distances = ShortestPath.Dijkstra<
            WaysNode, WaysEdgeTopology, ListEdges<WaysNode, long>, long>(nodes[N]);

        foreach (var node in nodes)
        {
            node.Dist = distances[node];
        }

        _start = nodes[0];
    }

    private static void AddEdge(WaysNode a, WaysNode b, long weight)
    {
        a.Edges.Add((weight, b));
        b.Edges.Add((weight, a));
    }

    [Benchmark(Baseline = true)]
    public long NaiveDfs() => CountPaths(_start);

    private static long CountPaths(WaysNode node)
    {
        if (node.Dist == 0)
        {
            return 1;
        }

        var total = 0L;

        foreach (var (weight, target) in node.Edges)
        {
            if (node.Dist - weight == target.Dist)
            {
                total += CountPaths(target);
            }
        }

        return total;
    }

    [Benchmark]
    public long DagFoldMemoized()
        => DagFold.Fold<
            WaysNode, WaysChildTopology, ListChildren<WaysNode>,
            NaturalChildOrder<WaysNode, ListChildren<WaysNode>>, ListChildren<WaysNode>,
            WaysCountAlgebra, long>(_start);

    // See NumberOfWaysToArriveAtDestinationTests.Fixtures for the full explanation
    // - repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own copy
    // of the solution rather than depending on the Tests project.
    private sealed class WaysNode(int id)
    {
        public int Id { get; } = id;

        public List<(long Weight, WaysNode Target)> Edges { get; } = [];

        public long Dist { get; set; }
    }

    private readonly struct WaysEdgeTopology : IEdgeTopology<WaysNode, ListEdges<WaysNode, long>, long>
    {
        public static ListEdges<WaysNode, long> GetEdges(WaysNode node) => new(node.Edges);
    }

    private readonly struct WaysChildTopology : IDagTopology<WaysNode, ListChildren<WaysNode>>
    {
        public static ListChildren<WaysNode> GetChildren(WaysNode node)
        {
            var children = new List<WaysNode>();

            foreach (var (weight, target) in node.Edges)
            {
                if (node.Dist - weight == target.Dist)
                {
                    children.Add(target);
                }
            }

            return new ListChildren<WaysNode>(children);
        }
    }

    private readonly struct WaysCountAlgebra : IFoldAlgebra<WaysNode, long>
    {
        private const long Modulo = 1_000_000_007;

        public static long Empty => 0;

        public static long Combine(WaysNode node, IReadOnlyList<long> children)
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
