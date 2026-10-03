using DSAExperimentation.LeetCode.CourseSchedule;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CourseScheduleSolution's, the same methods CourseScheduleTests
// proves correct. The workload is acyclic by construction - every prerequisite names an
// earlier-numbered course - so both arms make a complete pass instead of bailing out at the
// first cycle, and the ratio compares two full traversals of the same graph.
public class CourseScheduleBenchmarks
{
    private const int RandomSeed = 207; // LC problem number
    private const int PrerequisitesPerCourse = 2;

    private int[][] _prerequisites = [];

    [Params(64, 256)]
    public int CourseCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var prerequisites = new List<int[]>();

        for (var course = 1; course < CourseCount; course++)
        {
            for (var draw = 0; draw < PrerequisitesPerCourse && draw < course; draw++)
            {
                prerequisites.Add([course, random.Next(0, course)]);
            }
        }

        _prerequisites = [.. prerequisites];
    }

    [Benchmark(Baseline = true)]
    public bool DepthFirstColoring() =>
        CourseScheduleSolution.CanFinishByDepthFirstColoring(CourseCount, _prerequisites);

    [Benchmark]
    public bool TopologicalSort() =>
        CourseScheduleSolution.CanFinishByTopologicalSort(CourseCount, _prerequisites);
}
