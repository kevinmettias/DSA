using DSAExperimentation.LeetCode.Solutions.MaximumProductOfTheLengthOfTwoPalindromicSubsequences;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumProductOfTheLengthOfTwoPalindromicSubsequences;

// Harness only: both enumeration strategies live in
// MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution. Pinned here to the
// degenerate ends of the 2^n space - a single character (no second disjoint palindrome
// exists, so the answer is 0), two distinct characters (each is a length-1 palindrome),
// and three identical ones (a length-2 palindrome paired with the leftover character).
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "a", 0 },
            { "ab", 1 },
            { "aaa", 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductByBitmaskScan_Cases_ReturnsBestDisjointPalindromePairProduct(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution.MaxProductByBitmaskScan(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductByBacktrack_Cases_ReturnsBestDisjointPalindromePairProduct(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution.MaxProductByBacktrack(text));
}
