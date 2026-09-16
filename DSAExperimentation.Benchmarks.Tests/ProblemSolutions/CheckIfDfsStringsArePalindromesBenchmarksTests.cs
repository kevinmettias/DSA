using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfDfsStringsArePalindromesBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - rebuilding each node's dfs string from scratch
// against one tour that answers every node's query from two rolling hashes - so a harness whose arms
// disagree is timing two different problems. Both arms answer with one flag per node in node order,
// which is part of this answer: flag i belongs to node i, so AnswerText.Of and not OfUnorderedSet
// is the rendering that keeps each flag scored against its own node.
public sealed partial class CheckIfDfsStringsArePalindromesBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameChainTreeAndCharacters()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The tree and the per-node characters are private, so the rebuild is pinned through the
        // flags they produce: one flag per node is the shape the answer promises, and the same
        // NodeCount must draw the same seeded character string over the same chain and answer with
        // the same 200 flags.
        Assert.Equal(SmallestNodeCount, first.BruteForce().Length);
        Assert.Equal(AnswerText.Of(first.EulerTourRollingHash()), AnswerText.Of(second.EulerTourRollingHash()));
    }

    [Fact]
    public void BruteForce_SeededChainTree_AgreesWithEulerTourRollingHash()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.EulerTourRollingHash()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void EulerTourRollingHash_SeededChainTree_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.EulerTourRollingHash()));
    }

    private static CheckIfDfsStringsArePalindromesBenchmarks BuildHarness()
    {
        var harness = new CheckIfDfsStringsArePalindromesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
