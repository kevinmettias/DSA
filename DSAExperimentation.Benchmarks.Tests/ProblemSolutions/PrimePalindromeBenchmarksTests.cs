using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PrimePalindromeBenchmarks (ARCHITECTURE 17.9): arm agreement at the larger
// LowerBound, which the generic check - run at the smallest LowerBound only - never reaches. That
// bound lands on the palindrome-sparse stretch the class comment names, where the sequential scan
// pays for candidates the generator never visits. LowerBound is the only [Params] axis and the class
// carries no [GlobalSetup], so each axis value is its own harness.
public sealed partial class PrimePalindromeBenchmarksTests
{
    private const int LargestLowerBound = 999;

    [Fact]
    public void SequentialScan_LargestLowerBound_AgreesWithPalindromeGeneration() =>
        Assert.Equal(
            Harness(LargestLowerBound).PalindromeGeneration(),
            Harness(LargestLowerBound).SequentialScan());

    private static PrimePalindromeBenchmarks Harness(int lowerBound) => new() { LowerBound = lowerBound };
}
