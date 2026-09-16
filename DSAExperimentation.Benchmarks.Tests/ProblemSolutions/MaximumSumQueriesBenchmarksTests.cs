using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSumQueriesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the per-query rescan of every index against the descending-sweep
// segment tree - so a harness whose arms disagree is timing two different problems. The arms answer
// each query in the input's own order and the problem pins that order, so the answers are compared
// positionally. Setup draws both arrays and the query batch from one fixed seed, so the same
// Length/QueryCount pair must rebuild the same workload; otherwise two published numbers were never
// comparable in the first place.
public sealed partial class MaximumSumQueriesBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int SmallestQueryCount = 200;

    [Fact]
    public void Setup_SameLengthAndQueryCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceScan()),
            AnswerText.Of(BuildHarness().BruteForceScan()));

    [Fact]
    public void BruteForceScan_SeededQueryBatch_AgreesWithSweepWithSegmentTree()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SweepWithSegmentTree()),
            AnswerText.Of(harness.BruteForceScan()));
    }

    [Fact]
    public void SweepWithSegmentTree_SeededQueryBatch_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceScan()),
            AnswerText.Of(harness.SweepWithSegmentTree()));
    }

    private static MaximumSumQueriesBenchmarks BuildHarness()
    {
        var harness = new MaximumSumQueriesBenchmarks
        {
            Length = SmallestLength,
            QueryCount = SmallestQueryCount,
        };
        harness.Setup();

        return harness;
    }
}
