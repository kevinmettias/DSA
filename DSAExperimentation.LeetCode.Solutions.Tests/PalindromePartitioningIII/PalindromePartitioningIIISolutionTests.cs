using DSAExperimentation.LeetCode.Solutions.PalindromePartitioningIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PalindromePartitioningIII;

// Harness only: both strategies live in PalindromePartitioningIIISolution and are
// asserted against the same cases, so a failure names the strategy that broke.
// The naive recursion was previously only a benchmark's baseline arm and went
// unasserted; it is under test here for the first time.
public sealed partial class PalindromePartitioningIIISolutionTests
{
    public static TheoryData<string, int, int> Cases =>
        new()
        {
            { "abc", 1, 1 },
            { "abc", 3, 0 },
            { "aabbc", 1, 2 },
            { "a", 1, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinChangesByNaiveRecursion_Cases_ReturnsMinimumCharacterChanges(
        string text, int partitionCount, int expected)
    {
        var actual = PalindromePartitioningIIISolution.MinChangesByNaiveRecursion(text, partitionCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinChangesByMemoizedRecurrence_Cases_ReturnsMinimumCharacterChanges(
        string text, int partitionCount, int expected)
    {
        var actual = PalindromePartitioningIIISolution.MinChangesByMemoizedRecurrence(text, partitionCount);

        Assert.Equal(expected, actual);
    }
}
