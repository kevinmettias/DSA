using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CourseScheduleIIBenchmarks (ARCHITECTURE 17.9): the generic arm check compares
// the arms only with each other, so what stays here is the one order the workload's shape allows,
// derived without either arm. The course list is private, and the workload's documented shape - a
// guaranteed-acyclic DAG whose every prerequisite edge points from a lower id to a higher one, with
// fan-out capped at 3 - forces the ordering as well: course 0 is the only one with no prerequisite,
// and after it each course i is the only one whose prerequisites 1..i-1 are all placed, so 0, 1, ..
// CourseCount - 1 is the only valid order there is to return.
public sealed partial class CourseScheduleIIBenchmarksTests
{
    private const int SmallestCourseCount = 50;

    [Fact]
    public void NaiveRescan_CappedFanOutDag_ReturnsEveryCourseInIdOrder() =>
        Assert.Equal(EveryCourseInIdOrder(SmallestCourseCount), BuildHarness().NaiveRescan());

    private static CourseScheduleIIBenchmarks BuildHarness()
    {
        var harness = new CourseScheduleIIBenchmarks { CourseCount = SmallestCourseCount };
        harness.Setup();

        return harness;
    }

    private static int[] EveryCourseInIdOrder(int courseCount) => [.. Enumerable.Range(0, courseCount)];
}
