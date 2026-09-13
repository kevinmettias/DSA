using DSAExperimentation.LeetCode.MaximumSumCircularSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumCircularSubarray;

// Harness only. Both strategies live in MaximumSumCircularSubarraySolution and are
// asserted against the same examples: LeetCode's three published ones, plus a
// single-element array, an all-negative array whose answer is its least-negative
// element (the case the complement trick gets wrong without its fallback), and a
// wraparound case where the best subarray really does straddle the end.
public sealed class MaximumSumCircularSubarrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, -2, 3, -2], 3 },
            { [5, -3, 5], 10 },
            { [-3, -2, -3], -2 },
            { [-1], -1 },
            { [3], 3 },
            { [-5, -2, -8, -1], -1 },
            { [2, -4, 1, 3], 6 },
            { [1, 2, 3, 4], 10 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarraySumCircularByBruteForce_LeetCodeExamples_ReturnsBestCircularSum(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSumCircularSubarraySolution.MaxSubarraySumCircularByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarraySumCircularByTwoPassKadane_LeetCodeExamples_ReturnsBestCircularSum(
        int[] nums, int expected) =>
        Assert.Equal(expected, MaximumSumCircularSubarraySolution.MaxSubarraySumCircularByTwoPassKadane(nums));
}
