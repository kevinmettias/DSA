using DSAExperimentation.LeetCode.PalindromePartitioningIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioningIII;

// Harness only: both strategies live in PalindromePartitioningIIISolution and are
// asserted against the same examples, so a failure names the strategy that broke.
// The naive recursion was previously only a benchmark's baseline arm and went
// unasserted; it is under test here for the first time.
public sealed class PalindromePartitioningIIITests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            { "abc", 2, 1 },
            { "aabbc", 3, 0 },
            { "leetcode", 8, 0 },
            { "abc", 1, 1 },
            { "abc", 3, 0 },
            { "aabbc", 1, 2 },
            { "a", 1, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinChangesByNaiveRecursion_LeetCodeExamples_ReturnsMinimumCharacterChanges(
        string s, int k, int expected)
    {
        var actual = PalindromePartitioningIIISolution.MinChangesByNaiveRecursion(s, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinChangesByMemoizedRecurrence_LeetCodeExamples_ReturnsMinimumCharacterChanges(
        string s, int k, int expected)
    {
        var actual = PalindromePartitioningIIISolution.MinChangesByMemoizedRecurrence(s, k);

        Assert.Equal(expected, actual);
    }
}
