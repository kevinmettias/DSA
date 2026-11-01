using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// what the workload's shape promises of the answer: one verdict per drawn query.
public sealed partial class CheckingExistenceOfEdgeLengthLimitedPathsBenchmarksTests
{
    private const int SmallestNodeCount = 100;

    // Setup draws two queries per node, so the answer must hold exactly this many verdicts - one per
    // query, none dropped and none invented.
    private const int QueryCountPerNodeMultiplier = 2;

    [Fact]
    public void DfsPerQuery_SeededMultigraphAndQueries_AnswersEveryQuery() =>
        Assert.Equal(SmallestNodeCount * QueryCountPerNodeMultiplier, BuildHarness().DfsPerQuery().Length);

    private static CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks BuildHarness()
    {
        var harness = new CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
