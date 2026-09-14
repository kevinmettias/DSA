using DSAExperimentation.LeetCode.PalindromePartitioningIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioningIV;

// Harness only: both strategies live in PalindromePartitioningIVSolution and are
// asserted against the same examples, so a failure names the strategy that broke.
// The unmemoized recursion was previously only a benchmark's baseline arm and went
// unasserted; it is under test here for the first time.
public sealed class PalindromePartitioningIVTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "abcbdd", true },
            { "bcbddxy", false },
            { "abc", true },
            { "aba", true },
            { "aaaa", true },
            { "abcde", false },
            { "ab", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckPartitioningByNaiveRecursion_LeetCodeExamples_ReturnsWhetherThreeWaySplitExists(
        string s, bool expected) =>
        Assert.Equal(expected, PalindromePartitioningIVSolution.CheckPartitioningByNaiveRecursion(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckPartitioningByMemoizedRecurrence_LeetCodeExamples_ReturnsWhetherThreeWaySplitExists(
        string s, bool expected) =>
        Assert.Equal(expected, PalindromePartitioningIVSolution.CheckPartitioningByMemoizedRecurrence(s));
}
