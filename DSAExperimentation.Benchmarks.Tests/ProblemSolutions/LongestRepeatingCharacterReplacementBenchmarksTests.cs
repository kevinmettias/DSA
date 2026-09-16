using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestRepeatingCharacterReplacementBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the O(n^2) scan that never breaks
// early against the sliding window over a letter count map - so a harness whose arms disagree is
// timing two different problems. Both arms return the longest run length, a scalar compared
// directly. Setup makes the text a single repeated character, so no window ever breaks early and
// the answer is the whole length: that length is the decisive value both arms must reach, and the
// same Length must rebuild the text.
public sealed partial class LongestRepeatingCharacterReplacementBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every rotation of Setup's text is already one repeating character, so nothing is ever
    // replaced and the longest run is the whole text.
    private const int ExpectedLongestRunLength = SmallestLength;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestRunLength, BuildHarness().SlidingWindowHashMap());
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithSlidingWindowHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestRunLength, harness.BruteForce());
        Assert.Equal(harness.SlidingWindowHashMap(), harness.BruteForce());
    }

    [Fact]
    public void SlidingWindowHashMap_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestRunLength, harness.SlidingWindowHashMap());
        Assert.Equal(harness.BruteForce(), harness.SlidingWindowHashMap());
    }

    private static LongestRepeatingCharacterReplacementBenchmarks BuildHarness()
    {
        var harness = new LongestRepeatingCharacterReplacementBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
