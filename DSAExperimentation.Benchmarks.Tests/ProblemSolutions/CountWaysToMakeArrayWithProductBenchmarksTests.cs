using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountWaysToMakeArrayWithProductBenchmarks (ARCHITECTURE 17.9): the generic arm
// check compares the arms only with each other, so what stays here is the workload's documented shape.
// The query array is private, but its length is exactly what the answer reports: both arms answer one
// way count per query, in query order, so the documented QueryCount pins the result's own length.
public sealed partial class CountWaysToMakeArrayWithProductBenchmarksTests
{
    private const int SmallestQueryCount = 200;

    [Fact]
    public void TrialDivisionPerQuery_SmallestQueryCount_AnswersOneCountPerQuery() =>
        Assert.Equal(SmallestQueryCount, BuildHarness().TrialDivisionPerQuery().Length);

    private static CountWaysToMakeArrayWithProductBenchmarks BuildHarness()
    {
        var harness = new CountWaysToMakeArrayWithProductBenchmarks { QueryCount = SmallestQueryCount };
        harness.Setup();

        return harness;
    }
}
