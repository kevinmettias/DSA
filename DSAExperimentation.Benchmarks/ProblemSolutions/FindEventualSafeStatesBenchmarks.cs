using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Eventual Safe States (LC 802): the textbook per-node three-color DFS (0
// unvisited, 1 in-progress, 2 safe) against modeling the same question as Kahn's
// algorithm over the reversed graph - this repo's own TopologicalSort.TrySort - via
// a SafeStateNode whose "children" are its predecessors. Nodes split into a forward
// DAG half (always safe, funneling into a terminal node) and a ring-cycle half
// (never safe), so both strategies do real work discovering both outcomes instead
// of one trivial all-safe or all-unsafe graph.
[MemoryDiagnoser]
public class FindEventualSafeStatesBenchmarks
{
    [Params(50, 1_000)]
    public int NodeCount;

    private int[][] _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        var half = NodeCount / 2;
        _graph = new int[NodeCount][];

        for (var i = 0; i < half; i++)
        {
            var fanOut = Math.Min(3, half - 1 - i);
            _graph[i] = Enumerable.Range(i + 1, fanOut).ToArray();
        }

        for (var i = half; i < NodeCount; i++)
        {
            var next = i + 1 < NodeCount ? i + 1 : half;
            _graph[i] = [next];
        }
    }

    [Benchmark(Baseline = true)]
    public int DfsThreeColoring()
    {
        var color = new int[_graph.Length];
        var safeCount = 0;

        for (var i = 0; i < _graph.Length; i++)
        {
            if (IsSafe(i, color))
            {
                safeCount++;
            }
        }

        return safeCount;
    }

    private bool IsSafe(int node, int[] color)
    {
        if (color[node] > 0)
        {
            return color[node] == 2;
        }

        color[node] = 1;

        foreach (var next in _graph[node])
        {
            if (!IsSafe(next, color))
            {
                color[node] = 3;
                return false;
            }
        }

        color[node] = 2;
        return true;
    }

    [Benchmark]
    public int ReversedKahnsTopologicalSort()
    {
        var nodes = new SafeStateNode[_graph.Length];

        for (var i = 0; i < _graph.Length; i++)
        {
            nodes[i] = new SafeStateNode(i);
        }

        for (var i = 0; i < _graph.Length; i++)
        {
            foreach (var next in _graph[i])
            {
                nodes[next].Predecessors.Add(nodes[i]);
            }
        }

        TopologicalSort.TrySort<
            SafeStateNode, SafeStateTopology, ListChildren<SafeStateNode>,
            NaturalChildOrder<SafeStateNode, ListChildren<SafeStateNode>>, ListChildren<SafeStateNode>>(
            nodes, out var ordering);

        return ordering.Count;
    }

    // See FindEventualSafeStatesTests.Fixtures for the full explanation - repeated
    // here rather than shared because TwoSumBenchmarks/CourseScheduleIIBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private sealed class SafeStateNode(int id)
    {
        public int Id { get; } = id;

        public List<SafeStateNode> Predecessors { get; } = [];
    }

    private readonly struct SafeStateTopology : IGraphTopology<SafeStateNode, ListChildren<SafeStateNode>>
    {
        public static ListChildren<SafeStateNode> GetChildren(SafeStateNode node) => new(node.Predecessors);
    }
}
