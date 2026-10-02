using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordBreakBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question - the Trie + Memoizer composition and the bottom-up reachability
// sweep - so a harness whose arms disagree is segmenting two different strings: both must report the
// same answer. The decisive value is the one its own comment names - a source that tiles one
// dictionary word and is therefore segmentable end to end, the full-scan case rather than an early
// bail-out on a dead prefix.
public sealed partial class WordBreakBenchmarksTests
{
    private const int SmallestLength = 600;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CanBreakByTrieMemoized()),
            AnswerText.Of(BuildHarness().CanBreakByTrieMemoized()));

    [Fact]
    public void CanBreakByTrieMemoized_TiledDictionaryWord_SegmentsTheWholeSource() =>
        Assert.True(BuildHarness().CanBreakByTrieMemoized());

    [Fact]
    public void IterativeReachability_TiledDictionaryWord_SegmentsTheWholeSource() =>
        Assert.True(BuildHarness().IterativeReachability());

    [Fact]
    public void IterativeReachability_AgreesWithCanBreakByTrieMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanBreakByTrieMemoized(), harness.IterativeReachability());
    }

    private static WordBreakBenchmarks BuildHarness()
    {
        var harness = new WordBreakBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
