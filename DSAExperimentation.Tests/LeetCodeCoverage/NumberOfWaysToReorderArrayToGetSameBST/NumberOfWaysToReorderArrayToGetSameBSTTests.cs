using DSAExperimentation.LeetCode.NumberOfWaysToReorderArrayToGetSameBST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToReorderArrayToGetSameBST;

// Harness only. Both counting strategies are
// NumberOfWaysToReorderArrayToGetSameBSTSolution's - this file just pins them to
// LeetCode's published examples plus the degenerate shapes (single element, already
// ascending, already descending) where the only reordering is the array itself and
// the answer is 0.
public sealed partial class NumberOfWaysToReorderArrayToGetSameBSTTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 1, 3], 1 },
            { [3, 4, 5, 1, 2], 5 },
            { [1, 2, 3], 0 },
            { [3, 1, 2, 5, 4, 6], 19 },
            { [1], 0 },
            { [3, 2, 1], 0 },
            { [2, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByListSplitting_LeetCodeExamples_ReturnsOtherOrderCount(int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfWaysToReorderArrayToGetSameBSTSolution.CountWaysByListSplitting(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByTreeFold_LeetCodeExamples_ReturnsOtherOrderCount(int[] nums, int expected) =>
        Assert.Equal(expected, NumberOfWaysToReorderArrayToGetSameBSTSolution.CountWaysByTreeFold(nums));
}
