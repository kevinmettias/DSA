using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClosestNodesQueriesInABinarySearchTreeBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// what the workload's shape promises of the answer: Setup draws one query per node, and the answer
// holds one pair per drawn query.
public sealed partial class ClosestNodesQueriesInABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void InOrderBinarySearch_SeededTreeAndQueries_AnswersEveryQuery() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().InOrderBinarySearch().Length);

    private static ClosestNodesQueriesInABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new ClosestNodesQueriesInABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
