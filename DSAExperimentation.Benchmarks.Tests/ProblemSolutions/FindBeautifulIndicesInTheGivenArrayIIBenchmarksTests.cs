using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindBeautifulIndicesInTheGivenArrayIIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a naive substring scan against a Z-function
// search - so a harness whose arms disagree is searching two different haystacks. Both arms return
// the matching start indices, a positional answer both compute left to right, so the lists are
// compared as ordered sequences. Setup is #3008's own version of the worst-case construction: the
// haystack is 'a' alone and both patterns are that character repeated with a mismatched final one,
// which makes every naive scan attempt run almost the whole pattern before failing. The patterns
// never occur, so the answer is empty on both arms - honest but weak agreement, since an empty
// result is also what a strategy that gave up entirely would return - so the emptiness is asserted
// against the fixture's construction directly rather than hidden in the agreement alone.
public sealed partial class FindBeautifulIndicesInTheGivenArrayIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorstCaseHaystack()
    {
        // Setup's patterns each end in a character the haystack never contains, so neither occurs
        // and a correct search has no index to report.
        Assert.Empty(BuildHarness().BruteForce());

        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));
    }

    [Fact]
    public void BruteForce_NoOccurrenceWorstCase_AgreesWithZFunction()
    {
        var harness = BuildHarness();

        Assert.Empty(harness.BruteForce());

        Assert.Equal(AnswerText.Of(harness.ZFunction()), AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void ZFunction_NoOccurrenceWorstCase_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Empty(harness.ZFunction());

        Assert.Equal(AnswerText.Of(harness.BruteForce()), AnswerText.Of(harness.ZFunction()));
    }

    private static FindBeautifulIndicesInTheGivenArrayIIBenchmarks BuildHarness()
    {
        var harness = new FindBeautifulIndicesInTheGivenArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
