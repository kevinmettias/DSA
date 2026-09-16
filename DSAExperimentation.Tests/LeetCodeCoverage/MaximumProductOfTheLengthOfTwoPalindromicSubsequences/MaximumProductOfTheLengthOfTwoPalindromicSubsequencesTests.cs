using DSAExperimentation.LeetCode.MaximumProductOfTheLengthOfTwoPalindromicSubsequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductOfTheLengthOfTwoPalindromicSubsequences;

// Harness only: both enumeration strategies live in
// MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution. Pinned here to LeetCode's
// three published examples plus the degenerate ends of the 2^n space - a single character
// (no second disjoint palindrome exists, so the answer is 0), two distinct characters
// (each is a length-1 palindrome), and three identical ones (a length-2 palindrome paired
// with the leftover character).
public sealed partial class MaximumProductOfTheLengthOfTwoPalindromicSubsequencesTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "leetcodecom", 9 },
            { "bb", 1 },
            { "accbcaxxcxx", 25 },
            { "a", 0 },
            { "ab", 1 },
            { "aaa", 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByBitmaskScan_LeetCodeExamples_ReturnsBestDisjointPalindromePairProduct(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution.MaxProductByBitmaskScan(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByBacktrack_LeetCodeExamples_ReturnsBestDisjointPalindromePairProduct(
        string text, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductOfTheLengthOfTwoPalindromicSubsequencesSolution.MaxProductByBacktrack(text));
}
