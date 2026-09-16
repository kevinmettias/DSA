using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostWalkInWeightedGraphBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a brute-force walk of the whole edge list per query
// against a prebuilt Union-Find component view - so a harness whose arms disagree is timing two
// different graphs. Answers come back one per query in query order, and that order is part of the
// answer: the cost at index i belongs to query i, so AnswerText.Of and not OfUnorderedSet is the
// rendering that keeps each cost scored against its own query. Setup builds the edges and the query
// list from one seeded stream, so the same NodeCount must rebuild both.
public sealed partial class MinimumCostWalkInWeightedGraphBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameGraph() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceWalk()),
            AnswerText.Of(BuildHarness().BruteForceWalk()));

    [Fact]
    public void BruteForceWalk_SeededGraphAndQueries_AgreesWithUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.UnionFind()), AnswerText.Of(harness.BruteForceWalk()));
    }

    [Fact]
    public void UnionFind_SeededGraphAndQueries_AgreesWithBruteForceWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForceWalk()), AnswerText.Of(harness.UnionFind()));
    }

    private static MinimumCostWalkInWeightedGraphBenchmarks BuildHarness()
    {
        var harness = new MinimumCostWalkInWeightedGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
