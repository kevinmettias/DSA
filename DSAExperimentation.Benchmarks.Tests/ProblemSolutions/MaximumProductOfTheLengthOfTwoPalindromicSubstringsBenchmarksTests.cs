using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarks (ARCHITECTURE
// 17.9): the answer is read off the uniform text Setup builds, an expected value the generic
// agreement check, which only compares arms with each other, cannot supply. The baseline answers
// with a single length product, compared directly.
public sealed partial class MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void NaiveSplitScan_UniformText_SplitsTheTextEvenly() =>
        Assert.Equal(ExpectedBalancedSplitProduct(SmallestLength), BuildHarness().NaiveSplitScan());

    // Setup builds the text as one repeated character, so every substring is a palindrome and any
    // two non-overlapping substrings of lengths a and b with a + b <= Length are admissible. The
    // product a * b is maximized by splitting as evenly as possible, which is the oracle below -
    // read off the fixture's layout rather than off either arm's return.
    private static long ExpectedBalancedSplitProduct(int length)
    {
        var shorterHalf = length / AlgorithmConstants.HalvingFactor;

        return (long)shorterHalf * (length - shorterHalf);
    }

    private static MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarks BuildHarness()
    {
        var harness = new MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
