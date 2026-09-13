using DSAExperimentation.LeetCode.PrimePalindrome;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrimePalindrome;

// Harness only: both strategies live in PrimePalindromeSolution and answer the same
// examples. The sequential scan was previously the benchmark's unasserted baseline -
// it is asserted here, including on 9999, where the answer sits five digits up
// because no four-digit palindrome is ever prime.
public sealed class PrimePalindromeTests
{
    public static TheoryData<int, long> Examples =>
        new()
        {
            { 1, 2L },
            { 2, 2L },
            { 3, 3L },
            { 6, 7L },
            { 8, 11L },
            { 12, 101L },
            { 13, 101L },
            { 100, 101L },
            { 9999, 10301L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestPrimePalindromeBySequentialScan_LeetCodeExamples_ReturnsSmallestPrimePalindromeAtLeastN(
        int n,
        long expected) =>
        Assert.Equal(expected, PrimePalindromeSolution.SmallestPrimePalindromeBySequentialScan(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestPrimePalindromeByPalindromeGeneration_LeetCodeExamples_ReturnsSmallestPrimePalindromeAtLeastN(
        int n,
        long expected) =>
        Assert.Equal(expected, PrimePalindromeSolution.SmallestPrimePalindromeByPalindromeGeneration(n));
}
