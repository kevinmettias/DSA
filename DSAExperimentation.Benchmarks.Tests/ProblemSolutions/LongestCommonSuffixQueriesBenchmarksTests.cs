using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCommonSuffixQueriesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n * word length) scan of every container
// word against the suffix trie walk - so a harness whose arms disagree is timing two different
// problems. Both arms return one container index per query, and the query each index answers is
// part of it, so AnswerText.Of is the comparison. Setup draws the container words and the query
// words from one seeded stream and builds the trie from the container, so the same ContainerSize
// must rebuild all three.
public sealed partial class LongestCommonSuffixQueriesBenchmarksTests
{
    private const int SmallestContainerSize = 30;

    [Fact]
    public void Setup_SmallestContainerSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().Trie()),
            AnswerText.Of(BuildHarness().Trie()));

    [Fact]
    public void BruteForce_SmallestContainerSize_AgreesWithTrie()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.Trie()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void Trie_SmallestContainerSize_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.Trie()));
    }

    private static LongestCommonSuffixQueriesBenchmarks BuildHarness()
    {
        var harness = new LongestCommonSuffixQueriesBenchmarks { ContainerSize = SmallestContainerSize };
        harness.Setup();

        return harness;
    }
}
