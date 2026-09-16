using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PrimePalindromeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the smallest prime palindrome at or above the lower bound - so a
// harness whose arms disagree is timing two different problems. LowerBound is the only [Params] axis
// and the class carries no [GlobalSetup], so each axis value is its own harness. Both axis values are
// asserted: the larger one lands on the palindrome-sparse stretch the class comment names, where the
// sequential scan pays for candidates the generator never visits.
public sealed partial class PrimePalindromeBenchmarksTests
{
    private const int SmallestLowerBound = 13;

    private const int LargestLowerBound = 999;

    [Fact]
    public void SequentialScan_AgreesWithPalindromeGeneration()
    {
        Assert.Equal(
            Harness(SmallestLowerBound).PalindromeGeneration(),
            Harness(SmallestLowerBound).SequentialScan());
        Assert.Equal(
            Harness(LargestLowerBound).PalindromeGeneration(),
            Harness(LargestLowerBound).SequentialScan());
    }

    [Fact]
    public void PalindromeGeneration_AgreesWithSequentialScan()
    {
        Assert.Equal(
            Harness(SmallestLowerBound).SequentialScan(),
            Harness(SmallestLowerBound).PalindromeGeneration());
        Assert.Equal(
            Harness(LargestLowerBound).SequentialScan(),
            Harness(LargestLowerBound).PalindromeGeneration());
    }

    private static PrimePalindromeBenchmarks Harness(int lowerBound) => new() { LowerBound = lowerBound };
}
