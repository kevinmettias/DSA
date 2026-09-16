using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PathExistenceQueriesInAGraphIBenchmarks (ARCHITECTURE 17.9): its two arms
// are PathExistenceQueriesInAGraphISolution's, competing answerers for the same query script - a
// fresh breadth-first search per query against one precomputed disjoint-set labelling - so a
// harness whose arms disagree is timing two different problems. Setup builds nums, the query
// script and the prebuilt groups from one fixed seed, so the same NodeCount must rebuild all
// three, and both arms read the same query script.
public sealed partial class PathExistenceQueriesInAGraphIBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceBfs()),
            AnswerText.Of(BuildHarness().BruteForceBfs()));

    [Fact]
    public void BruteForceBfs_SeededProximityQueries_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DisjointSet()), AnswerText.Of(harness.BruteForceBfs()));
    }

    [Fact]
    public void DisjointSet_SeededProximityQueries_AgreesWithBruteForceBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForceBfs()), AnswerText.Of(harness.DisjointSet()));
    }

    private static PathExistenceQueriesInAGraphIBenchmarks BuildHarness()
    {
        var harness = new PathExistenceQueriesInAGraphIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
