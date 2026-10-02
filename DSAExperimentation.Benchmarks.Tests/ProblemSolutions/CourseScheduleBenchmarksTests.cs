using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CourseScheduleBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. The generator claims an acyclic workload, so that claim is checked here rather than
// assumed - two arms that agree on a cyclic input would prove much less.
public sealed partial class CourseScheduleBenchmarksTests
{
    private const int SmallestCourseCount = 64;

    [Fact]
    public void Setup_GeneratesAnAcyclicWorkload() =>
        Assert.True(BuildHarness().TopologicalSort());

    [Fact]
    public void DepthFirstColoring_AgreesWithTopologicalSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TopologicalSort(), harness.DepthFirstColoring());
    }

    private static CourseScheduleBenchmarks BuildHarness()
    {
        var harness = new CourseScheduleBenchmarks { CourseCount = SmallestCourseCount };
        harness.Setup();

        return harness;
    }
}
