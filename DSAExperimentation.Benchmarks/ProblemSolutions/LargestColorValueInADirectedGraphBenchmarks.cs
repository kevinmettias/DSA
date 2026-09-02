using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Largest Color Value in a Directed Graph (LC 1857): RepeatedRelaxation
// relaxes every edge once per node, every node, for NodeCount rounds
// (Bellman-Ford-style, ignoring any actual dependency order) to guarantee
// convergence on a DAG regardless of processing order, O(V*E).
// KahnsTopologicalSortDp instead orders nodes once via this repo's own
// TopologicalSort.TrySort and relaxes each edge exactly once in that order,
// O(V+E). Nodes form a guaranteed-acyclic DAG (every edge points from a
// lower id to a higher one, capped fan-out) so both strategies do their full
// real workload instead of an early cycle bailout.
[MemoryDiagnoser]
public class LargestColorValueInADirectedGraphBenchmarks
{
    private const int AlphabetSize = 26;
    private const int MaxFanOut = 3;

    // LC problem number, reused as the deterministic node-color seed.
    private const int RandomSeed = 1857;

    [Params(50, 1_000)]
    public int NodeCount;

    private List<ColorGraphNode> _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nodes = Enumerable.Range(0, NodeCount)
            .Select(id => new ColorGraphNode(id, random.Next(AlphabetSize)))
            .ToList();

        for (var i = 0; i < NodeCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, NodeCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _nodes[i].Successors.Add(_nodes[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int RepeatedRelaxation()
    {
        var counts = BuildInitialCounts();
        RelaxAllRounds(counts);
        return MaxCount(counts);
    }

    private Dictionary<ColorGraphNode, int[]> BuildInitialCounts()
    {
        return _nodes.ToDictionary(node => node, node =>
        {
            var array = new int[AlphabetSize];
            array[node.Color] = 1;
            return array;
        });
    }

    private void RelaxAllRounds(Dictionary<ColorGraphNode, int[]> counts)
    {
        for (var round = 0; round < _nodes.Count; round++)
        {
            foreach (var node in _nodes)
            {
                RelaxNode(counts, node);
            }
        }
    }

    private static void RelaxNode(Dictionary<ColorGraphNode, int[]> counts, ColorGraphNode node)
    {
        var nodeCounts = counts[node];
        foreach (var child in node.Successors)
        {
            RelaxEdge(counts, nodeCounts, child);
        }
    }

    private static void RelaxEdge(Dictionary<ColorGraphNode, int[]> counts, int[] nodeCounts, ColorGraphNode child)
    {
        var childCounts = counts[child];
        for (var c = 0; c < AlphabetSize; c++)
        {
            var candidate = nodeCounts[c] + (c == child.Color ? 1 : 0);
            if (candidate > childCounts[c])
            {
                childCounts[c] = candidate;
            }
        }
    }

    private static int MaxCount(Dictionary<ColorGraphNode, int[]> counts)
    {
        return counts.Values.SelectMany(nodeCounts => nodeCounts).Max();
    }

    [Benchmark]
    public int KahnsTopologicalSortDp()
    {
        TopologicalSort.TrySort<
            ColorGraphNode, ColorGraphTopology, ListChildren<ColorGraphNode>,
            NaturalChildOrder<ColorGraphNode, ListChildren<ColorGraphNode>>, ListChildren<ColorGraphNode>>(
            _nodes, out var ordering);

        var counts = _nodes.ToDictionary(node => node, _ => new int[AlphabetSize]);
        var best = 0;

        foreach (var node in ordering)
        {
            var candidate = RelaxFromNode(counts, node);
            best = Math.Max(best, candidate);
        }

        return best;
    }

    private static int RelaxFromNode(Dictionary<ColorGraphNode, int[]> counts, ColorGraphNode node)
    {
        var nodeCounts = counts[node];
        nodeCounts[node.Color]++;

        foreach (var child in node.Successors)
        {
            var childCounts = counts[child];
            for (var c = 0; c < AlphabetSize; c++)
            {
                childCounts[c] = Math.Max(childCounts[c], nodeCounts[c]);
            }
        }

        return nodeCounts[node.Color];
    }

    // See LargestColorValueInADirectedGraphTests.Fixtures for the full
    // explanation - repeated here rather than shared because
    // TwoSumBenchmarks/CourseScheduleIIBenchmarks establish this project
    // keeps its own copy of the solution rather than depending on the Tests
    // project.
    private sealed class ColorGraphNode(int id, int color)
    {
        public int Id { get; } = id;

        public int Color { get; } = color;

        public List<ColorGraphNode> Successors { get; } = [];
    }

    private readonly struct ColorGraphTopology : IGraphTopology<ColorGraphNode, ListChildren<ColorGraphNode>>
    {
        public static ListChildren<ColorGraphNode> GetChildren(ColorGraphNode node) => new(node.Successors);
    }
}
