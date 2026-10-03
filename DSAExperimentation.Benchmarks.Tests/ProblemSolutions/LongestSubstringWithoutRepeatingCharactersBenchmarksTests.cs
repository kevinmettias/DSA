using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestSubstringWithoutRepeatingCharactersBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - the O(n^2) scan whose inner loop
// breaks on the first repeat against the sliding window over a last-seen map - so a harness whose
// arms disagree is timing two different problems. Both arms return the substring length, a scalar
// compared directly. Setup cycles through the 95 printable ASCII characters (' ' through '~'), so
// every window of 95 is repeat-free and every longer one repeats its first character: the answer
// is 95, the decisive value both arms must reach, and the same Length must rebuild the text.
public sealed partial class LongestSubstringWithoutRepeatingCharactersBenchmarksTests
{
    private const int SmallestLength = 200;

    // ' ' through '~' is 95 characters; the smallest text is longer than one full cycle, so the
    // longest repeat-free substring is exactly one cycle.
    private const int ExpectedLongestRepeatFreeLength = '~' - ' ' + 1;

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
