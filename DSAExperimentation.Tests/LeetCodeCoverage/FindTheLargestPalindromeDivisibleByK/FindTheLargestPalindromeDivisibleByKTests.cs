using DSAExperimentation.LeetCode.FindTheLargestPalindromeDivisibleByK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheLargestPalindromeDivisibleByK;

// Harness only: both strategies live in FindTheLargestPalindromeDivisibleByKSolution -
// this file just pins them to LeetCode's published examples.
public sealed class FindTheLargestPalindromeDivisibleByKTests
{
    public static TheoryData<int, int, string> Examples =>
        new()
        {
            { 3, 5, "595" },
            { 1, 4, "8" },
            { 5, 6, "89898" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPalindromeByBruteForce_LeetCodeExamples_ReturnsLargestKPalindromicInteger(
        int n, int k, string expected)
    {
        var actual = FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByBruteForce(n, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPalindromeByDigitDpMemo_LeetCodeExamples_ReturnsLargestKPalindromicInteger(
        int n, int k, string expected)
    {
        var actual = FindTheLargestPalindromeDivisibleByKSolution.LargestPalindromeByDigitDpMemo(n, k);

        Assert.Equal(expected, actual);
    }
}
