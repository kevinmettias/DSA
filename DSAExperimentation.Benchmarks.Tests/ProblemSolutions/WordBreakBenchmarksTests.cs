using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordBreakBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to
// agreeing with each other, which a shared wrong verdict would pass, so this pins the baseline's
// verdict. The decisive value is the one the benchmark's own comment names - a source that tiles one
// dictionary word and is therefore segmentable end to end, the full-scan case rather than an early
// bail-out on a dead prefix.
public sealed partial class WordBreakBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void IterativeReachability_TiledDictionaryWord_SegmentsTheWholeSource() =>
        Assert.True(BuildHarness().IterativeReachability());

    private static WordBreakBenchmarks BuildHarness()
    {
        var harness = new WordBreakBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
