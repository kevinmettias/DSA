using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestPathInAWeightedTreeBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a fresh BFS from the root for every [2, x]
// distance query against one Euler-tour Fenwick sweep that absorbs every weight update - so a
// harness whose arms disagree is answering two different query streams. Setup draws the tree
// and the query script from one fixed seed, so the same NodeCount must rebuild the same
// workload; otherwise two published numbers were never comparable.
//
// Answers come back one per distance query in query order, and query i's distance belongs to
// query i, so AnswerText.Of rather than OfUnorderedSet keeps each distance scored against its
// own query.
public sealed partial class ShortestPathInAWeightedTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameQueryAnswers() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().EulerFenwick()),
            AnswerText.Of(BuildHarness().EulerFenwick()));

    [Fact]
    public void BruteForceBfs_MixedWeightUpdatesAndDistanceQueries_AgreesWithEulerFenwick()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.EulerFenwick()),
            AnswerText.Of(harness.BruteForceBfs()));
    }

    [Fact]
    public void EulerFenwick_MixedWeightUpdatesAndDistanceQueries_AgreesWithBruteForceBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceBfs()),
            AnswerText.Of(harness.EulerFenwick()));
    }

    private static ShortestPathInAWeightedTreeBenchmarks BuildHarness()
    {
        var harness = new ShortestPathInAWeightedTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
