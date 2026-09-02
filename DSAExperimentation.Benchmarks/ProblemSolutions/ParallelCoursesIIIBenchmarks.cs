using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Parallel Courses III (LC 2050): RepeatedRelaxation relaxes every edge once per
// node, every node, for NodeCount rounds (Bellman-Ford-style, ignoring any actual
// dependency order) to guarantee the max-finish-time DP converges on a DAG
// regardless of processing order, O(V*E) - the same shape
// LargestColorValueInADirectedGraphBenchmarks uses for its own per-color counts.
// KahnsTopologicalSortDp instead orders nodes once via this repo's own
// TopologicalSort.TrySort and relaxes each edge exactly once in that order,
// O(V+E). Nodes form a guaranteed-acyclic DAG (every edge points from a lower id
// to a higher one, capped fan-out) so both strategies do their full real
// workload instead of an early cycle bailout.
[MemoryDiagnoser]
public class ParallelCoursesIIIBenchmarks
{
    private const int MaxDuration = 100;

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2050;

    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int NodeCount;

    private List<CourseTimeNode> _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nodes = Enumerable.Range(0, NodeCount)
            .Select(id => new CourseTimeNode(id, random.Next(1, MaxDuration)))
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
        var finish = _nodes.ToDictionary(node => node, node => node.Time);

        for (var round = 0; round < _nodes.Count; round++)
        {
            foreach (var node in _nodes)
            {
                RelaxSuccessors(node, finish);
            }
        }

        return finish.Values.Max();
    }

    private static void RelaxSuccessors(CourseTimeNode node, Dictionary<CourseTimeNode, int> finish)
    {
        var nodeFinish = finish[node];
        foreach (var child in node.Successors)
        {
            var candidate = nodeFinish + child.Time;
            if (candidate > finish[child])
            {
                finish[child] = candidate;
            }
        }
    }

    [Benchmark]
    public int KahnsTopologicalSortDp()
    {
        TopologicalSort.TrySort<
            CourseTimeNode, CourseTimeTopology, ListChildren<CourseTimeNode>,
            NaturalChildOrder<CourseTimeNode, ListChildren<CourseTimeNode>>, ListChildren<CourseTimeNode>>(
            _nodes, out var ordering);

        var readyAt = _nodes.ToDictionary(node => node, _ => 0);
        var best = 0;

        foreach (var node in ordering)
        {
            var finish = readyAt[node] + node.Time;
            best = Math.Max(best, finish);

            foreach (var child in node.Successors)
            {
                readyAt[child] = Math.Max(readyAt[child], finish);
            }
        }

        return best;
    }

    // See ParallelCoursesIIITests.Fixtures for the full explanation - repeated
    // here rather than shared because TwoSumBenchmarks/CourseScheduleIIBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private sealed class CourseTimeNode(int id, int time)
    {
        public int Id { get; } = id;

        public int Time { get; } = time;

        public List<CourseTimeNode> Successors { get; } = [];
    }

    private readonly struct CourseTimeTopology : IGraphTopology<CourseTimeNode, ListChildren<CourseTimeNode>>
    {
        public static ListChildren<CourseTimeNode> GetChildren(CourseTimeNode node) => new(node.Successors);
    }
}
