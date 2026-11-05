using DSAExperimentation.LeetCode.Solutions.MaximumSubarrayMinProduct;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumSubarrayMinProduct;

// Harness only. Both the every-subarray baseline and the monotonic-stack contribution
// sweep are MaximumSubarrayMinProductSolution's - this file just pins them to
// the single-element, all-equal, monotone and
// over-the-modulus cases that exercise the span bounds at the array's edges and the
// final 1e9+7 reduction.
public sealed partial class MaximumSubarrayMinProductSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [7], 49 },
            { [2, 2, 2], 12 },
            { [1, 2, 3, 4], 21 },
            { [100_000, 100_000], 999_999_867 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSumMinProductByBruteForce_Cases_ReturnsLargestMinTimesSum(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarrayMinProductSolution.MaxSumMinProductByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSumMinProductByMonotonicStack_Cases_ReturnsLargestMinTimesSum(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSubarrayMinProductSolution.MaxSumMinProductByMonotonicStack(nums));
}
