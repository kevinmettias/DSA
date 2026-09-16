using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumGeneticDifferenceQueryBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumGeneticDifferenceQuerySolution's competing strategies for one question - the naive
// per-query ancestor climb against one offline bit-trie DFS - so a harness whose arms disagree is
// timing two different problems. Each arm answers one value per query in the queries' original
// order, which is part of the answer: value i belongs to query i, so AnswerText.Of and not
// OfUnorderedSet is the rendering that keeps each answer scored against its own query.
public sealed partial class MaximumGeneticDifferenceQueryBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameChainTreeAndQueries()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // One answer per query is the shape both arms promise, so the rebuilt workload is pinned
        // through the answers it produces: the same NodeCount must draw the same seeded query
        // list over the same chain parent[] and answer it identically.
        Assert.Equal(SmallestNodeCount, first.NaiveAncestorWalk().Length);
        Assert.Equal(AnswerText.Of(first.BitTrieOfflineDfs()), AnswerText.Of(second.BitTrieOfflineDfs()));
    }

    [Fact]
    public void NaiveAncestorWalk_SeededChainTree_AgreesWithBitTrieOfflineDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BitTrieOfflineDfs()), AnswerText.Of(harness.NaiveAncestorWalk()));
    }

    [Fact]
    public void BitTrieOfflineDfs_SeededChainTree_AgreesWithNaiveAncestorWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.NaiveAncestorWalk()), AnswerText.Of(harness.BitTrieOfflineDfs()));
    }

    private static MaximumGeneticDifferenceQueryBenchmarks BuildHarness()
    {
        var harness = new MaximumGeneticDifferenceQueryBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
