using DSAExperimentation.LeetCode.CountPartitionsWithMaxMinDifferenceAtMostK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountPartitionsWithMaxMinDifferenceAtMostK;

// Harness only. Both strategies are
// CountPartitionsWithMaxMinDifferenceAtMostKSolution's - this file just pins them to
// LeetCode's published examples.
public sealed partial class CountPartitionsWithMaxMinDifferenceAtMostKTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [9, 4, 1, 3, 7], 4, 6 },
            { [3, 3, 4], 0, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPartitionsByBruteForce_LeetCodeExamples_ReturnsValidPartitionCount(
        int[] nums, int maxDifference, int expected)
    {
        var actual = CountPartitionsWithMaxMinDifferenceAtMostKSolution.CountPartitionsByBruteForce(nums, maxDifference);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPartitionsBySlidingWindowDeque_LeetCodeExamples_ReturnsValidPartitionCount(
        int[] nums, int maxDifference, int expected)
    {
        var actual = CountPartitionsWithMaxMinDifferenceAtMostKSolution.CountPartitionsBySlidingWindowDeque(nums, maxDifference);
        Assert.Equal(expected, actual);
    }
}
