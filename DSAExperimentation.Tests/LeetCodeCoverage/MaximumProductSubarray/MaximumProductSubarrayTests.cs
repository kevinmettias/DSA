using DSAExperimentation.LeetCode.MaximumProductSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductSubarray;

// Harness only: both strategies live in MaximumProductSubarraySolution and are
// asserted against the same examples, including the negative-only and
// single-element cases that exercise the min/max swap on its own.
public sealed partial class MaximumProductSubarrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 3, -2, 4], 6 },
            { [-2, 0, -1], 0 },
            { [-2, 3, -4], 24 },
            { [-2], -2 },
            { [0, 2], 2 },
            { [-1, -2, -9, -6], 108 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByBruteForce_LeetCodeExamples_ReturnsMaximumProduct(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumProductSubarraySolution.MaxProductByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductByMinMaxScan_LeetCodeExamples_ReturnsMaximumProduct(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumProductSubarraySolution.MaxProductByMinMaxScan(nums));
}
