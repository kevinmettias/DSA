using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestRepeatingCharacterReplacementBenchmarks (ARCHITECTURE 17.9): Setup
// makes the text a single repeated character, so no window ever breaks early and the answer is the
// whole length - an expected value derived from the workload's construction rather than from either
// arm, which the generic agreement check cannot supply. The baseline returns the longest run length,
// a scalar compared directly.
public sealed partial class LongestRepeatingCharacterReplacementBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every rotation of Setup's text is already one repeating character, so nothing is ever
    // replaced and the longest run is the whole text.
    private const int ExpectedLongestRunLength = SmallestLength;

    [Fact]
    public void BruteForce_SmallestLength_SpansTheWholeText() =>
        Assert.Equal(ExpectedLongestRunLength, BuildHarness().BruteForce());

    private static LongestRepeatingCharacterReplacementBenchmarks BuildHarness()
    {
        var harness = new LongestRepeatingCharacterReplacementBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
