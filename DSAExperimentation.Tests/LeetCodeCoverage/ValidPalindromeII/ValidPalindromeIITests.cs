using DSAExperimentation.LeetCode.ValidPalindromeII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidPalindromeII;

// Harness only. Both strategies are ValidPalindromeIISolution's - this file
// just pins them to LeetCode's published examples.
public sealed class ValidPalindromeIITests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "aba", true },
            { "abca", true },
            { "abcdef", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidPalindromeByBruteForceDeletion_LeetCodeExamples_ReturnsWhetherAtMostOneDeletionWorks(
        string s, bool expected) =>
        Assert.Equal(expected, ValidPalindromeIISolution.IsValidPalindromeByBruteForceDeletion(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidPalindromeByMismatchSkip_LeetCodeExamples_ReturnsWhetherAtMostOneDeletionWorks(
        string s, bool expected) =>
        Assert.Equal(expected, ValidPalindromeIISolution.IsValidPalindromeByMismatchSkip(s));
}
