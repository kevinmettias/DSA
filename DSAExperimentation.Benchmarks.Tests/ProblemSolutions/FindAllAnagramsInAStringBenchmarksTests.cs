using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllAnagramsInAStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rebuilding a fresh frequency map at every window
// start against one sliding pass with a running match count - so a harness whose arms disagree is
// scanning two different texts. Both arms return the window starts they matched, and both walk the
// text left to right through the same window positions, so the returned order is the same positional
// answer on both sides and the lists are compared as ordered sequences. Setup draws the text from a
// fixed seed, so the same Length must rebuild the same text; every match it can produce is a window
// start inside that text.
public sealed partial class FindAllAnagramsInAStringBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // Setup's pattern is "aeiou", so a match can only start where a full five-character window fits.
    private const int PatternLength = 5;
    private const int MaxMatchIndex = SmallestLength - PatternLength;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameScannedText()
    {
        var indices = BuildHarness().PerWindowFrequencyRebuild();

        Assert.All(indices, index => Assert.InRange(index, 0, MaxMatchIndex));

        Assert.Equal(
            AnswerText.Of(BuildHarness().PerWindowFrequencyRebuild()),
            AnswerText.Of(BuildHarness().PerWindowFrequencyRebuild()));
    }

    [Fact]
    public void PerWindowFrequencyRebuild_SeededText_AgreesWithSlidingWindowFrequencyMap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SlidingWindowFrequencyMap()),
            AnswerText.Of(harness.PerWindowFrequencyRebuild()));
    }

    [Fact]
    public void SlidingWindowFrequencyMap_SeededText_AgreesWithPerWindowFrequencyRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PerWindowFrequencyRebuild()),
            AnswerText.Of(harness.SlidingWindowFrequencyMap()));
    }

    private static FindAllAnagramsInAStringBenchmarks BuildHarness()
    {
        var harness = new FindAllAnagramsInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
