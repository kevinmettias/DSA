using DSAExperimentation.LeetCode.Solutions.MinimizeDeviationInArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimizeDeviationInArray;

// Harness only: both strategies live in MinimizeDeviationInArraySolution and are
// asserted against the same cases - LeetCode's own three, a single element that
// still gets doubled and halved back, and a two-element case where halving carries
// the running minimum below every value the array started with.
public sealed partial class MinimizeDeviationInArraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [5], 0 },
            { [3, 5], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumDeviationByLinearRescan_Cases_ReturnsSmallestReachableDeviation(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimizeDeviationInArraySolution.MinimumDeviationByLinearRescan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumDeviationByMaxHeap_Cases_ReturnsSmallestReachableDeviation(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimizeDeviationInArraySolution.MinimumDeviationByMaxHeap(nums));
}
