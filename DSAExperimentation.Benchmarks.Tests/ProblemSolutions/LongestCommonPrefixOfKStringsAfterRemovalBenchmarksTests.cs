using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCommonPrefixOfKStringsAfterRemovalBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - the O(n * maxLength^2) textbook
// recount against the two Reduce.Tree passes plus a per-word walk - so a harness whose arms
// disagree is timing two different problems. Both arms return one answer per removed index, and
// the index each answer belongs to is part of it, so AnswerText.Of is the comparison. Setup
// builds the words from a fixed seed and the trie from those words, so the same WordCount must
// rebuild both; the answers themselves are per-word, not derivable from the documented shape.
public sealed partial class LongestCommonPrefixOfKStringsAfterRemovalBenchmarksTests
{
    private const int SmallestWordCount = 20;

    [Fact]
    public void Setup_SmallestWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ReduceTrie()),
            AnswerText.Of(BuildHarness().ReduceTrie()));

    [Fact]
    public void BruteForce_SmallestWordCount_AgreesWithReduceTrie()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ReduceTrie()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void ReduceTrie_SmallestWordCount_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.ReduceTrie()));
    }

    private static LongestCommonPrefixOfKStringsAfterRemovalBenchmarks BuildHarness()
    {
        var harness = new LongestCommonPrefixOfKStringsAfterRemovalBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
