using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CourseScheduleIVBenchmarks (ARCHITECTURE 17.9): the generic arm check compares
// the arms only with each other, so what stays here is the workload's documented shape. Setup draws a
// fixed number of queries, and both arms answer each query with exactly one boolean, so the reading's
// shape is one verdict per query.
public sealed partial class CourseScheduleIVBenchmarksTests
{
    private const int SmallestCourseCount = 50;

    // [GlobalSetup] builds this many queries regardless of CourseCount.
    private const int QueryCount = 300;

    [Fact]
    public void BfsPerQuery_ThreeHundredReachabilityQueries_AnswersOneVerdictPerQuery() =>
        Assert.Equal(QueryCount, BuildHarness().BfsPerQuery().Count);

    private static CourseScheduleIVBenchmarks BuildHarness()
    {
        var harness = new CourseScheduleIVBenchmarks { CourseCount = SmallestCourseCount };
        harness.Setup();

        return harness;
    }
}
