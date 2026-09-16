using DSAExperimentation.LeetCode.MinimumPartitionScore;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumPartitionScore;

// Harness only: both strategies live in MinimumPartitionScoreSolution and are
// asserted against the same examples, so a failure names the strategy that
// broke.
public sealed class MinimumPartitionScoreTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [5, 1, 2, 1], 2, 25 },
            { [1, 2, 3, 4], 1, 55 },
            { [1, 1, 1], 3, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinPartitionScoreByDictionaryMemo_LeetCodeExamples_ReturnsMinimumScore(int[] nums, int k, long expected)
    {
        var actual = MinimumPartitionScoreSolution.MinPartitionScoreByDictionaryMemo(nums, k);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinPartitionScoreByMemoizedPartition_LeetCodeExamples_ReturnsMinimumScore(int[] nums, int k, long expected)
    {
        var actual = MinimumPartitionScoreSolution.MinPartitionScoreByMemoizedPartition(nums, k);
        Assert.Equal(expected, actual);
    }
}
