using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordBreakIIBenchmarks (ARCHITECTURE 17.9): both arms are competing
// segmentation strategies for one source and one dictionary, so a harness whose arms disagree is
// timing two different problems. Note what the arms agree ON: each returns the COUNT of sentences,
// not the sentences themselves, so agreement witnesses that both found the same number of
// segmentations and cannot witness that they are the same segmentations. The workload is a source
// tiling one dictionary word, whose only segmentation is that tiling - the decisive count the
// assertions below pin, so the weak comparison is anchored to something independent.
public sealed partial class WordBreakIIBenchmarksTests
{
    private const int SmallestLength = 600;
    private const int ExpectedSentenceCount = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().HashSetUnboundedScan()),
            AnswerText.Of(BuildHarness().HashSetUnboundedScan()));

    [Fact]
    public void HashSetUnboundedScan_AgreesWithTriePrunedMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashSetUnboundedScan(), harness.TriePrunedMemoized());
    }

    [Fact]
    public void TriePrunedMemoized_AgreesWithHashSetUnboundedScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TriePrunedMemoized(), harness.HashSetUnboundedScan());
    }

    [Fact]
    public void HashSetUnboundedScan_SingleDictionaryWord_FindsOneSentence() =>
        Assert.Equal(ExpectedSentenceCount, BuildHarness().HashSetUnboundedScan());

    [Fact]
    public void TriePrunedMemoized_SingleDictionaryWord_FindsOneSentence() =>
        Assert.Equal(ExpectedSentenceCount, BuildHarness().TriePrunedMemoized());

    private static WordBreakIIBenchmarks BuildHarness()
    {
        var harness = new WordBreakIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
