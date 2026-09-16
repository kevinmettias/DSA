using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfPrefixScoresOfStringsBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfPrefixScoresOfStringsSolution's - the whole-word-list rescan against the prefix-counting
// LowercaseTrie - so a harness whose arms disagree is timing two different questions. Each arm returns
// one score per word, and the problem pins those scores to the order of the input words, so the outer
// order is part of the answer and the comparison is order-sensitive; the word list itself is a seeded
// draw, so a rebuild at the same WordCount has to yield the same scores.
public sealed partial class SumOfPrefixScoresOfStringsBenchmarksTests
{
    private const int SmallestWordCount = 300;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().StartsWithScan()),
            AnswerText.Of(BuildHarness().StartsWithScan()));

    [Fact]
    public void StartsWithScan_SeededWordList_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PrefixCountingTrie()),
            AnswerText.Of(harness.StartsWithScan()));
    }

    [Fact]
    public void PrefixCountingTrie_SeededWordList_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.StartsWithScan()),
            AnswerText.Of(harness.PrefixCountingTrie()));
    }

    private static SumOfPrefixScoresOfStringsBenchmarks BuildHarness()
    {
        var harness = new SumOfPrefixScoresOfStringsBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
