using DSAExperimentation.LeetCode.MaximumSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSubarray;

// Harness only. Both strategies live in MaximumSubarraySolution and are
// asserted against the same examples, including a single-element array and
// an all-positive array where the whole array is the best subarray.
public sealed partial class MaximumSubarrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [-2, 1, -3, 4, -1, 2, 1, -5, 4], 6 },
            { [1], 1 },
            { [5, 4, -1, 7, 8], 23 },
            { [-1], -1 },
            { [-2, -1], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubArrayByBruteForce_LeetCodeExamples_ReturnsBestContiguousSum(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarraySolution.MaxSubArrayByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubArrayByKadaneScan_LeetCodeExamples_ReturnsBestContiguousSum(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarraySolution.MaxSubArrayByKadaneScan(nums));
}
