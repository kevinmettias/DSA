using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumGeneticDifferenceQueryBenchmarks (ARCHITECTURE 17.9): Setup draws one
// query per node, and each arm answers one value per query in the queries' original order - value i
// belongs to query i. That shape is a property of the workload the generic agreement check, which
// only compares arms with each other, cannot supply, so the baseline's answer count is pinned to it.
public sealed partial class MaximumGeneticDifferenceQueryBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // One answer per query is the shape both arms promise, and Setup draws NodeCount queries.
    [Fact]
    public void NaiveAncestorWalk_SeededChainTree_AnswersEveryQuery() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().NaiveAncestorWalk().Length);

    private static MaximumGeneticDifferenceQueryBenchmarks BuildHarness()
    {
        var harness = new MaximumGeneticDifferenceQueryBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
