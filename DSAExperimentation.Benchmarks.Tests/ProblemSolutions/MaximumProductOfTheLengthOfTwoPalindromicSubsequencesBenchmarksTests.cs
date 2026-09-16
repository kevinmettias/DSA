using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumProductOfTheLengthOfTwoPalindromicSubsequencesBenchmarks
// (ARCHITECTURE 17.9): both arms are MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution's
// competing strategies for one question - hand-rolled bitmask enumeration of the disjoint
// subsequence pairs against this repo's own Backtrack.Search walk over the identical 2^n space - so
// a harness whose arms disagree is timing two different problems. Both answer with a single length
// product, compared directly.
public sealed partial class MaximumProductOfTheLengthOfTwoPalindromicSubsequencesBenchmarksTests
{
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameFourLetterString()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The string is private, so the rebuild is pinned through the product it produces: the same
        // Length must draw the same seeded characters over the four-letter alphabet and score the
        // same disjoint palindrome pair.
        Assert.Equal(first.BruteForce(), second.BruteForce());
        Assert.Equal(first.Backtracking(), second.Backtracking());
    }

    [Fact]
    public void BruteForce_SeededFourLetterString_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Backtracking(), harness.BruteForce());
    }

    [Fact]
    public void Backtracking_SeededFourLetterString_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.Backtracking());
    }

    private static MaximumProductOfTheLengthOfTwoPalindromicSubsequencesBenchmarks BuildHarness()
    {
        var harness = new MaximumProductOfTheLengthOfTwoPalindromicSubsequencesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
