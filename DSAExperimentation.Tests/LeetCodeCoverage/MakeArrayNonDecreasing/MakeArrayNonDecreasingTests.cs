using DSAExperimentation.LeetCode.MakeArrayNonDecreasing;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MakeArrayNonDecreasing;

// Harness only. Both strategies are MakeArrayNonDecreasingSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class MakeArrayNonDecreasingTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [4, 2, 5, 3, 5], 3 },
            { [1, 2, 3], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSizeByPrefixDynamicProgramming_LeetCodeExamples_ReturnsLargestNonDecreasingSize(
        int[] nums, int expected) =>
        Assert.Equal(expected, MakeArrayNonDecreasingSolution.MaxSizeByPrefixDynamicProgramming(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSizeByGreedyScan_LeetCodeExamples_ReturnsLargestNonDecreasingSize(
        int[] nums, int expected) =>
        Assert.Equal(expected, MakeArrayNonDecreasingSolution.MaxSizeByGreedyScan(nums));
}
