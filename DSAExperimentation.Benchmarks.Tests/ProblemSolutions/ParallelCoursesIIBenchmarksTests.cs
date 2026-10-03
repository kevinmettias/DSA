using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ParallelCoursesIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one minimum semester count - the brute-force recursion against the memoized one - so
// a harness whose arms disagree is timing two different problems. Setup only clears the prerequisite
// list, and the benchmark takes at most two courses a semester, so with no prerequisites every
// semester can take a full pair: SmallestCourseCount courses need exactly half as many semesters.
// Both arms are held to that fixed count as well as to each other.
public sealed partial class ParallelCoursesIIBenchmarksTests
{
    private const int SmallestCourseCount = 6;
    private const int CoursesPerSemester = 2;

    // No prerequisites, so no semester is ever blocked from taking its full pair.
    private const int ExpectedSemesterCount = SmallestCourseCount / CoursesPerSemester;

    [Fact]
    public void Setup_SameCourseCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForceRecursion(SmallestCourseCount),
            BuildHarness().BruteForceRecursion(SmallestCourseCount));

    [Fact]
    public void BruteForceRecursion_SmallestCourseCount_ReturnsTheFewestSemesters()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSemesterCount, harness.BruteForceRecursion(SmallestCourseCount));
        Assert.Equal(
            harness.MemoizedRecursion(SmallestCourseCount),
            harness.BruteForceRecursion(SmallestCourseCount));
    }

    [Fact]
    public void MemoizedRecursion_SmallestCourseCount_ReturnsTheFewestSemesters()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSemesterCount, harness.MemoizedRecursion(SmallestCourseCount));
        Assert.Equal(
            harness.BruteForceRecursion(SmallestCourseCount),
            harness.MemoizedRecursion(SmallestCourseCount));
    }

    private static ParallelCoursesIIBenchmarks BuildHarness()
    {
        var harness = new ParallelCoursesIIBenchmarks();
        harness.Setup();

        return harness;
    }
}
