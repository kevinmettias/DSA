using DSAExperimentation.LeetCode.Solutions.MaximumProductSubarray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumProductSubarray;

// Harness only: both strategies live in MaximumProductSubarraySolution and are
// asserted against the same cases, among them the negative-only and
// single-element cases that exercise the min/max swap on its own.
public sealed partial class MaximumProductSubarraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [-2], -2 },
            { [-1, -2, -9, -6], 108 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductByBruteForce_Cases_ReturnsMaximumProduct(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumProductSubarraySolution.MaxProductByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductByMinMaxScan_Cases_ReturnsMaximumProduct(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumProductSubarraySolution.MaxProductByMinMaxScan(nums));
}
