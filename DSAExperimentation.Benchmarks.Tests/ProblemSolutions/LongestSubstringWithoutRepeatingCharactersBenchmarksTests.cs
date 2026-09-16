using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestSubstringWithoutRepeatingCharactersBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - the O(n^2) scan whose inner loop
// breaks on the first repeat against the sliding window over a last-seen map - so a harness whose
// arms disagree is timing two different problems. Both arms return the substring length, a scalar
// compared directly. Setup builds the text from all-distinct characters, so no window ever breaks
// early and the answer is the whole length: that length is the decisive value both arms must
// reach, and the same Length must rebuild the text.
public sealed partial class LongestSubstringWithoutRepeatingCharactersBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup's characters are all distinct, so the whole text is a repeat-free substring.
    private const int ExpectedLongestRepeatFreeLength = SmallestLength;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestRepeatFreeLength, BuildHarness().SlidingWindowHashMap());
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithSlidingWindowHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestRepeatFreeLength, harness.BruteForce());
        Assert.Equal(harness.SlidingWindowHashMap(), harness.BruteForce());
    }

    [Fact]
    public void SlidingWindowHashMap_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestRepeatFreeLength, harness.SlidingWindowHashMap());
        Assert.Equal(harness.BruteForce(), harness.SlidingWindowHashMap());
    }

    private static LongestSubstringWithoutRepeatingCharactersBenchmarks BuildHarness()
    {
        var harness = new LongestSubstringWithoutRepeatingCharactersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
