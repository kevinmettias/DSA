using DSAExperimentation.LeetCode.Solutions.SumOfSubarrayRanges;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumOfSubarrayRanges;

// Harness only: both strategies live in SumOfSubarrayRangesSolution and are
// asserted against the same cases - LeetCode's three published cases, a single
// element (no subarray has a nonzero range), a run of equal values (the strict
// pop condition that stops a tie being attributed twice, once through the maximum
// sweep and once through the minimum sweep), and a strictly increasing and a
// strictly decreasing array, which are mirror images of each other. The
// brute-force arm was previously an unasserted benchmark baseline; it is under
// test here for the first time.
public sealed partial class SumOfSubarrayRangesSolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [2, 2, 2], 0L },
            { [1, 2, 3, 4], 10L },
            { [4, 3, 2, 1], 10L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SubarrayRangesByBruteForce_Cases_ReturnsSumOfRanges(int[] nums, long expected) =>
        Assert.Equal(expected, SumOfSubarrayRangesSolution.SubarrayRangesByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SubarrayRangesByMonotonicStack_Cases_ReturnsSumOfRanges(int[] nums, long expected) =>
        Assert.Equal(expected, SumOfSubarrayRangesSolution.SubarrayRangesByMonotonicStack(nums));
}
