using DSAExperimentation.LeetCode.MaximumSubarrayMinProduct;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSubarrayMinProduct;

// Harness only. Both the every-subarray baseline and the monotonic-stack contribution
// sweep are MaximumSubarrayMinProductSolution's - this file just pins them to
// LeetCode's published examples plus the single-element, all-equal, monotone and
// over-the-modulus cases that exercise the span bounds at the array's edges and the
// final 1e9+7 reduction.
public sealed partial class MaximumSubarrayMinProductTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 2], 14 },
            { [2, 3, 3, 1, 2], 18 },
            { [3, 1, 5, 6, 4, 2], 60 },
            { [7], 49 },
            { [2, 2, 2], 12 },
            { [1, 2, 3, 4], 21 },
            { [100_000, 100_000], 999_999_867 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumMinProductByBruteForce_LeetCodeExamples_ReturnsLargestMinTimesSum(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarrayMinProductSolution.MaxSumMinProductByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumMinProductByMonotonicStack_LeetCodeExamples_ReturnsLargestMinTimesSum(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarrayMinProductSolution.MaxSumMinProductByMonotonicStack(nums));
}
