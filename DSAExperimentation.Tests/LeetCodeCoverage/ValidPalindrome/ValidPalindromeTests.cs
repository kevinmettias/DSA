using DSAExperimentation.LeetCode.ValidPalindrome;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidPalindrome;

// Harness only. The two-pointer scan is ValidPalindromeSolution's; this file
// pins it to LeetCode's published examples.
public sealed class ValidPalindromeTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "A man, a plan, a canal: Panama", true },
            { "race a car", false },
            { " ", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPalindromeByTwoPointerScan_LeetCodeExamples_IgnoresNonAlphanumeric(string value, bool expected) =>
        Assert.Equal(expected, ValidPalindromeSolution.IsPalindromeByTwoPointerScan(value));
}
