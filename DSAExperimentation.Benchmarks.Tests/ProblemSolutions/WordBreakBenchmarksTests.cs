using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordBreakBenchmarks (ARCHITECTURE 17.9). The class has a single arm, so there
// is no second answer to compare against: the assertion is the decisive value its own comment names,
// a source that tiles one dictionary word and is therefore segmentable end to end - the full-scan
// case, not an early bail-out on a dead prefix.
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

    private static WordBreakBenchmarks BuildHarness()
    {
        var harness = new WordBreakBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
