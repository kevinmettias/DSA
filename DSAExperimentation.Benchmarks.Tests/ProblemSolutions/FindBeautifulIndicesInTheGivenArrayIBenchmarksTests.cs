using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindBeautifulIndicesInTheGivenArrayIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a naive substring scan against a
// prefix-function search - so a harness whose arms disagree is searching two different haystacks.
// Both arms return the matching start indices, a positional answer both compute left to right, so
// the lists are compared as ordered sequences. Setup builds the searched text from 'a' alone and
// both patterns as that character repeated with a mismatched final one, which is the documented
// worst case: the patterns never occur, so every naive start position is compared almost the whole
// pattern through before failing. The answer is therefore empty on both arms - honest but weak
// agreement, since an empty result is also what a strategy that gave up entirely would return - so
// the emptiness is asserted against the fixture's construction directly rather than hidden in the
// agreement alone.
public sealed partial class FindBeautifulIndicesInTheGivenArrayIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorstCaseText()
    {
        // Setup's patterns each end in a character the searched text never contains, so neither
        // occurs and a correct search has no index to report.
        Assert.Empty(BuildHarness().BruteForce());

        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));
    }

    [Fact]
    public void BruteForce_NoOccurrenceWorstCase_AgreesWithPrefixFunctionSearch()
    {
        var harness = BuildHarness();

        Assert.Empty(harness.BruteForce());

        Assert.Equal(
            AnswerText.Of(harness.PrefixFunctionSearch()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void PrefixFunctionSearch_NoOccurrenceWorstCase_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Empty(harness.PrefixFunctionSearch());

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.PrefixFunctionSearch()));
    }

    private static FindBeautifulIndicesInTheGivenArrayIBenchmarks BuildHarness()
    {
        var harness = new FindBeautifulIndicesInTheGivenArrayIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
