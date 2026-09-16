using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CourseScheduleIVBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a fresh reachability search per query against one all-pairs
// closure plus a dictionary lookup per query - so a harness whose arms disagree is timing two
// different problems. Setup draws the prerequisite edges and the queries from one fixed seed, and both
// arms answer each query with exactly one boolean, so the reading's shape is one verdict per query;
// the same CourseCount must rebuild the same edges and with them the same queries, otherwise two
// published numbers were never comparable in the first place.
public sealed partial class CourseScheduleIVBenchmarksTests
{
    private const int SmallestCourseCount = 50;

    // [GlobalSetup] builds this many queries regardless of CourseCount.
    private const int QueryCount = 300;

    [Fact]
    public void Setup_SameCourseCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(QueryCount, BuildHarness().BfsPerQuery().Count);
        Assert.Equal(BuildHarness().BfsPerQuery(), BuildHarness().BfsPerQuery());
    }

    [Fact]
    public void BfsPerQuery_ThreeHundredReachabilityQueries_AgreesWithFloydWarshallAllPairs()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.FloydWarshallAllPairs()),
            AnswerText.Of(harness.BfsPerQuery()));
    }

    [Fact]
    public void FloydWarshallAllPairs_ThreeHundredReachabilityQueries_AgreesWithBfsPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BfsPerQuery()),
            AnswerText.Of(harness.FloydWarshallAllPairs()));
    }

    private static CourseScheduleIVBenchmarks BuildHarness()
    {
        var harness = new CourseScheduleIVBenchmarks { CourseCount = SmallestCourseCount };
        harness.Setup();

        return harness;
    }
}
