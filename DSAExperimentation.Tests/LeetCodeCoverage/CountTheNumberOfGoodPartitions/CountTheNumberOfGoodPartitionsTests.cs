using DSAExperimentation.LeetCode.CountTheNumberOfGoodPartitions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfGoodPartitions;

// Harness only: both strategies live in CountTheNumberOfGoodPartitionsSolution.
// One test method per strategy over one shared set of LeetCode's published
// examples, so a failure names the strategy that broke.
public sealed partial class CountTheNumberOfGoodPartitionsTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 2, 3, 4], 8 },
            { [1, 1, 1, 1], 1 },
            { [1, 2, 1, 3], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodPartitionsByBruteForce_LeetCodeExamples_ReturnsGoodPartitionCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountTheNumberOfGoodPartitionsSolution.CountGoodPartitionsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodPartitionsByLastOccurrenceMerge_LeetCodeExamples_ReturnsGoodPartitionCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountTheNumberOfGoodPartitionsSolution.CountGoodPartitionsByLastOccurrenceMerge(nums));
}
