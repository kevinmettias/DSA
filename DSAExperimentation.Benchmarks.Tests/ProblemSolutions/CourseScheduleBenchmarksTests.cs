using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CourseScheduleBenchmarks (ARCHITECTURE 17.9): the generic arm check compares
// the arms only with each other, and two arms that agree on a cyclic input would prove much less. The
// generator claims an acyclic workload, so that claim is checked here rather than assumed.
public sealed partial class CourseScheduleBenchmarksTests
{
    private const int SmallestCourseCount = 64;

    [Fact]
    public void Setup_GeneratesAnAcyclicWorkload() =>
        Assert.True(BuildHarness().TopologicalSort());

    private static CourseScheduleBenchmarks BuildHarness()
    {
        var harness = new CourseScheduleBenchmarks { CourseCount = SmallestCourseCount };
        harness.Setup();

        return harness;
    }
}
