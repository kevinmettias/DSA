using DSAExperimentation.LeetCode.SumOfSubarrayMinimums;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfSubarrayMinimums;

// Harness only: both strategies live in SumOfSubarrayMinimumsSolution and are
// asserted against the same examples - LeetCode's two published cases, a single
// element, a run of equal minimums (the </<= asymmetry that stops a tie being
// counted twice), a strictly increasing array where every element is its own
// prefix's minimum, and a strictly decreasing one where the last element is the
// minimum of every subarray that reaches it.
public sealed partial class SumOfSubarrayMinimumsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 1, 2, 4], 17 },
            { [11, 81, 94, 43, 3], 444 },
            { [7], 7 },
            { [2, 2, 2], 12 },
            { [1, 2, 3, 4], 20 },
            { [4, 3, 2, 1], 20 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumSubarrayMinsByBruteForce_LeetCodeExamples_ReturnsSummedMinimums(int[] arr, int expected) =>
        Assert.Equal(expected, SumOfSubarrayMinimumsSolution.SumSubarrayMinsByBruteForce(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumSubarrayMinsByMonotonicStack_LeetCodeExamples_ReturnsSummedMinimums(int[] arr, int expected) =>
        Assert.Equal(expected, SumOfSubarrayMinimumsSolution.SumSubarrayMinsByMonotonicStack(arr));
}
