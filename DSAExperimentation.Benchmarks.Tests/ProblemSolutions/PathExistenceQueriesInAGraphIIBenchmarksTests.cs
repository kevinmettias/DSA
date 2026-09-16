using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathExistenceQueriesInAGraphIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are PathExistenceQueriesInAGraphIISolution's, competing answerers for the same query script -
// a breadth-first walk that discovers each node's reachable range on the fly against a doubling
// table over precomputed far() pointers - so a harness whose arms disagree is timing two different
// problems. Setup builds both the value-sorted graph witness and the query script from one fixed
// seed, so the same NodeCount must rebuild both, and each arm gets the prepared shape its own
// hoisted overload takes.
public sealed partial class PathExistenceQueriesInAGraphIIBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(AnswerText.Of(BuildHarness().RangeBfs()), AnswerText.Of(BuildHarness().RangeBfs()));

    [Fact]
    public void RangeBfs_SeededSortedValueQueries_AgreesWithBinaryLifting()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BinaryLifting()), AnswerText.Of(harness.RangeBfs()));
    }

    [Fact]
    public void BinaryLifting_SeededSortedValueQueries_AgreesWithRangeBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.RangeBfs()), AnswerText.Of(harness.BinaryLifting()));
    }

    private static PathExistenceQueriesInAGraphIIBenchmarks BuildHarness()
    {
        var harness = new PathExistenceQueriesInAGraphIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
