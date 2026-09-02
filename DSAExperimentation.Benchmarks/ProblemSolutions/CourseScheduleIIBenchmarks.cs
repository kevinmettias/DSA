using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Course Schedule II (LC 210): a naive topological sort - rescanning every
// remaining course for one with zero remaining prerequisites on each step, O(V^2 +
// V*E) - against this repo's own Kahn's-algorithm TopologicalSort.TrySort, O(V+E)
// via a queue of already-zero-in-degree courses. Courses form a guaranteed-acyclic
// DAG (every prerequisite edge points from a lower id to a higher one, capped
// fan-out) so both strategies run their full real workload instead of an early
// cycle bailout.
[MemoryDiagnoser]
public class CourseScheduleIIBenchmarks
{
    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int CourseCount;

    private List<CourseNode> _courses = null!;

    [GlobalSetup]
    public void Setup()
    {
        _courses = Enumerable.Range(0, CourseCount).Select(id => new CourseNode(id)).ToList();

        for (var i = 0; i < CourseCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, CourseCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _courses[i].EnabledCourses.Add(_courses[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRescan()
    {
        var inDegree = _courses.ToDictionary(course => course, _ => 0);
        foreach (var course in _courses)
        {
            foreach (var next in course.EnabledCourses)
            {
                inDegree[next]++;
            }
        }

        var remaining = new List<CourseNode>(_courses);
        var order = new List<CourseNode>(_courses.Count);

        while (remaining.Count > 0)
        {
            if (!TryAdvanceNaiveRescan(remaining, inDegree, order))
            {
                break;
            }
        }

        return order.Count;
    }

    private static bool TryAdvanceNaiveRescan(
        List<CourseNode> remaining, Dictionary<CourseNode, int> inDegree, List<CourseNode> order)
    {
        var next = remaining.FirstOrDefault(course => inDegree[course] == 0);

        if (next is null)
        {
            return false;
        }

        order.Add(next);
        remaining.Remove(next);

        foreach (var dependent in next.EnabledCourses)
        {
            inDegree[dependent]--;
        }

        return true;
    }

    [Benchmark]
    public int KahnsTopologicalSort()
    {
        TopologicalSort.TrySort<
            CourseNode, CourseTopology, ListChildren<CourseNode>,
            NaturalChildOrder<CourseNode, ListChildren<CourseNode>>, ListChildren<CourseNode>>(
            _courses, out var ordering);

        return ordering.Count;
    }

    // See CourseScheduleIITests.Fixtures for the full explanation - repeated here
    // rather than shared because TwoSumBenchmarks/MedianOfTwoSortedArraysBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private sealed class CourseNode(int id)
    {
        public int Id { get; } = id;

        public List<CourseNode> EnabledCourses { get; } = [];
    }

    private readonly struct CourseTopology : IGraphTopology<CourseNode, ListChildren<CourseNode>>
    {
        public static ListChildren<CourseNode> GetChildren(CourseNode node) => new(node.EnabledCourses);
    }
}
