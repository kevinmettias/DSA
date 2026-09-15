using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CourseScheduleII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CourseScheduleIISolution's, the same methods
// CourseScheduleIITests proves correct. Each arm is handed the prepared
// List<CourseNode> its hoisted overload takes, so graph construction is
// charged to [GlobalSetup] rather than to the search being measured. Courses
// form a guaranteed-acyclic DAG (every prerequisite edge points from a lower
// id to a higher one, capped fan-out) so both strategies run their full real
// workload instead of an early cycle bailout.
[MemoryDiagnoser]
public class CourseScheduleIIBenchmarks
{
    private const int MaxFanOut = 3;

    private List<CourseNode> _courses = new();

    [Params(50, 1_000)]
    public int CourseCount { get; set; }

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
    public int[] NaiveRescan() => CourseScheduleIISolution.FindOrderByNaiveRescan(_courses);

    [Benchmark]
    public int[] KahnsTopologicalSort() => CourseScheduleIISolution.FindOrderByKahnsTopologicalSort(_courses);
}
