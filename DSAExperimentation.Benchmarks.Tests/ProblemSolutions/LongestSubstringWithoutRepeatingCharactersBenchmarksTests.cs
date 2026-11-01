using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestSubstringWithoutRepeatingCharactersBenchmarks (ARCHITECTURE 17.9):
// Setup cycles through the 95 printable ASCII characters (' ' through '~'), so every window of 95 is
// repeat-free and every longer one repeats its first character: the answer is 95 - an expected value
// derived from the workload's construction rather than from either arm, which the generic agreement
// check cannot supply. The baseline returns the substring length, a scalar compared directly.
public sealed partial class LongestSubstringWithoutRepeatingCharactersBenchmarksTests
{
    private const int SmallestLength = 200;

    // ' ' through '~' is 95 characters; the smallest text is longer than one full cycle, so the
    // longest repeat-free substring is exactly one cycle.
    private const int ExpectedLongestRepeatFreeLength = '~' - ' ' + 1;

    [Fact]
    public void BruteForce_SmallestLength_SpansOnePrintableCycle() =>
        Assert.Equal(ExpectedLongestRepeatFreeLength, BuildHarness().BruteForce());

    private static LongestSubstringWithoutRepeatingCharactersBenchmarks BuildHarness()
    {
        var harness = new LongestSubstringWithoutRepeatingCharactersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
