using DSAExperimentation.LeetCode.MinimizeDeviationInArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeDeviationInArray;

// Harness only: both strategies live in MinimizeDeviationInArraySolution and are
// asserted against the same examples - LeetCode's own three, a single element that
// still gets doubled and halved back, and a two-element case where halving carries
// the running minimum below every value the array started with.
public sealed partial class MinimizeDeviationInArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4], 1 },
            { [4, 1, 5, 20, 3], 3 },
            { [2, 10, 8], 3 },
            { [5], 0 },
            { [3, 5], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumDeviationByLinearRescan_LeetCodeExamples_ReturnsSmallestReachableDeviation(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimizeDeviationInArraySolution.MinimumDeviationByLinearRescan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumDeviationByMaxHeap_LeetCodeExamples_ReturnsSmallestReachableDeviation(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimizeDeviationInArraySolution.MinimumDeviationByMaxHeap(nums));
}
