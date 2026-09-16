using DSAExperimentation.LeetCode.PalindromePartitioningII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioningII;

// Harness only. PalindromePartitioningIISolution owns the memoized suffix
// recurrence; this file pins it to LeetCode's published examples.
public sealed partial class PalindromePartitioningIITests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "aab", 1 },
            { "a", 0 },
            { "ab", 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCutByMemoizedSuffixRecurrence_LeetCodeExamples_ReturnsMinimumCuts(
        string text, int expected) =>
        Assert.Equal(expected, PalindromePartitioningIISolution.MinCutByMemoizedSuffixRecurrence(text));
}
