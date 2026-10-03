using DSAExperimentation.LeetCode.PalindromePartitioningII;

namespace DSAExperimentation.LeetCode.Tests.PalindromePartitioningII;

// Harness only. Both strategies belong to PalindromePartitioningIISolution - the
// memoized suffix recurrence and the bottom-up cut table; this file pins them to
// LeetCode's published examples.
public sealed partial class PalindromePartitioningIISolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCutByIterativeDynamicProgramming_LeetCodeExamples_ReturnsMinimumCuts(
        string text, int expected) =>
        Assert.Equal(expected, PalindromePartitioningIISolution.MinCutByIterativeDynamicProgramming(text));
}
