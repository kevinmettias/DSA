using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllAnagramsInAStringBenchmarks (ARCHITECTURE 17.9): the generic arms check
// only compares the arms with each other, so this holds the baseline's answer to the workload's own
// shape - every match it can produce is a window start inside the text Setup draws.
public sealed partial class FindAllAnagramsInAStringBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // Setup's pattern is "aeiou", so a match can only start where a full five-character window fits.
    private const int PatternLength = 5;
    private const int MaxMatchIndex = SmallestLength - PatternLength;

    [Fact]
    public void PerWindowFrequencyRebuild_SeededText_MatchesOnlyWindowStartsInsideTheText() =>
        Assert.All(BuildHarness().PerWindowFrequencyRebuild(), index => Assert.InRange(index, 0, MaxMatchIndex));

    private static FindAllAnagramsInAStringBenchmarks BuildHarness()
    {
        var harness = new FindAllAnagramsInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
