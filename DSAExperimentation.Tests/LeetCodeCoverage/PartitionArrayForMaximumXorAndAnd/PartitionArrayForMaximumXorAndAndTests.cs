using DSAExperimentation.LeetCode.PartitionArrayForMaximumXorAndAnd;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionArrayForMaximumXorAndAnd;

// Harness only. Both strategies are PartitionArrayForMaximumXorAndAndSolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class PartitionArrayForMaximumXorAndAndTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [2, 3], 5 },
            { [1, 3, 2], 6 },
            { [2, 3, 6, 7], 15 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPartitionValueByBruteForce_LeetCodeExamples_ReturnsMaximumXorAndAndValue(
        int[] nums, long expected) =>
        Assert.Equal(expected, PartitionArrayForMaximumXorAndAndSolution.MaxPartitionValueByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPartitionValueBySubsetXorBasis_LeetCodeExamples_ReturnsMaximumXorAndAndValue(
        int[] nums, long expected) =>
        Assert.Equal(expected, PartitionArrayForMaximumXorAndAndSolution.MaxPartitionValueBySubsetXorBasis(nums));
}
