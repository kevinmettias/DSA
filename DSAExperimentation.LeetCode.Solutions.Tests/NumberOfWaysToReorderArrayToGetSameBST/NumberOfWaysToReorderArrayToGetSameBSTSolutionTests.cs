using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToReorderArrayToGetSameBST;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToReorderArrayToGetSameBST;

// Harness only. Both counting strategies are
// NumberOfWaysToReorderArrayToGetSameBSTSolution's - this file just pins them to
// the degenerate shapes (single element, already
// ascending, already descending) where the only reordering is the array itself and
// the answer is 0.
public sealed partial class NumberOfWaysToReorderArrayToGetSameBSTSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [3, 1, 2, 5, 4, 6], 19 },
            { [1], 0 },
            { [3, 2, 1], 0 },
            { [2, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByListSplitting_Cases_ReturnsOtherOrderCount(int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfWaysToReorderArrayToGetSameBSTSolution.CountWaysByListSplitting(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByTreeFold_Cases_ReturnsOtherOrderCount(int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfWaysToReorderArrayToGetSameBSTSolution.CountWaysByTreeFold(nums));
}
