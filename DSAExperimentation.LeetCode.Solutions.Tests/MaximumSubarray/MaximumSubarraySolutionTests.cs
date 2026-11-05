using DSAExperimentation.LeetCode.Solutions.MaximumSubarray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumSubarray;

// Harness only. Both strategies live in MaximumSubarraySolution and are
// asserted against the same cases, among them a single-element array and
// an all-positive array where the whole array is the best subarray.
public sealed partial class MaximumSubarraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [-1], -1 },
            { [-2, -1], -1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSubArrayByBruteForce_Cases_ReturnsBestContiguousSum(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarraySolution.MaxSubArrayByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSubArrayByKadaneScan_Cases_ReturnsBestContiguousSum(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarraySolution.MaxSubArrayByKadaneScan(nums));
}
