using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarks (ARCHITECTURE
// 17.9): both arms are MaximumProductOfTheLengthOfTwoPalindromicSubstringsSolution's competing
// strategies for one question - re-deriving the longest odd palindrome on each side of every split
// against one Manacher pass plus a forward and backward sweep over its radii - so a harness whose
// arms disagree is timing two different problems. Both answer with a single length product, compared
// directly.
public sealed partial class MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameUniformText()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedBalancedSplitProduct(SmallestLength), first.NaiveSplitScan());
        Assert.Equal(ExpectedBalancedSplitProduct(SmallestLength), second.ManacherSplitScan());
    }

    [Fact]
    public void NaiveSplitScan_UniformText_AgreesWithManacherSplitScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManacherSplitScan(), harness.NaiveSplitScan());
    }

    [Fact]
    public void ManacherSplitScan_UniformText_AgreesWithNaiveSplitScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveSplitScan(), harness.ManacherSplitScan());
    }

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
