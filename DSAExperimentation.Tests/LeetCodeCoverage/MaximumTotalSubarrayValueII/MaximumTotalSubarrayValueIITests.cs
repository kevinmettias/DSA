using DSAExperimentation.LeetCode.MaximumTotalSubarrayValueII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumTotalSubarrayValueII;

// Harness only. Both strategies are MaximumTotalSubarrayValueIISolution's - this
// file just pins them to LeetCode's published examples.
public sealed class MaximumTotalSubarrayValueIITests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 3, 2], 2, 4L },
            { [4, 2, 5, 1], 3, 12L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxTotalValueByBruteForce_LeetCodeExamples_ReturnsMaximumTotalValue(int[] nums, int k, long expected) =>
        Assert.Equal(expected, MaximumTotalSubarrayValueIISolution.MaxTotalValueByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxTotalValueBySegmentTreeHeap_LeetCodeExamples_ReturnsMaximumTotalValue(int[] nums, int k, long expected) =>
        Assert.Equal(expected, MaximumTotalSubarrayValueIISolution.MaxTotalValueBySegmentTreeHeap(nums, k));
}
