using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ParallelCoursesIIBenchmarks (ARCHITECTURE 17.9): the baseline's semester count
// against one derived from the workload's shape, which the generic check cannot supply - it holds the
// memoized recursion to the baseline. Setup only clears the prerequisite list, and the benchmark takes
// at most two courses a semester, so with no prerequisites every semester can take a full pair:
// SmallestCourseCount courses need exactly half as many semesters.
public sealed partial class ParallelCoursesIIBenchmarksTests
{
    private const int SmallestCourseCount = 6;
    private const int CoursesPerSemester = 2;

    // No prerequisites, so no semester is ever blocked from taking its full pair.
    private const int ExpectedSemesterCount = SmallestCourseCount / CoursesPerSemester;

    [Fact]
    public void BruteForceRecursion_SmallestCourseCount_ReturnsTheFewestSemesters() =>
        Assert.Equal(ExpectedSemesterCount, BuildHarness().BruteForceRecursion(SmallestCourseCount));

    private static ParallelCoursesIIBenchmarks BuildHarness()
    {
        var harness = new ParallelCoursesIIBenchmarks();
        harness.Setup();

        return harness;
    }
}
