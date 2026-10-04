using DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArray;

namespace DSAExperimentation.LeetCode.Tests.FindMinimumInRotatedSortedArray;

// Harness only: both strategies live in FindMinimumInRotatedSortedArraySolution
// and are asserted against the same examples, including the unrotated array and
// the two-element rotation that exercise the low-side rule at its edges.
public sealed partial class FindMinimumInRotatedSortedArraySolutionTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 4, 5, 1, 2], 1 },
            { [4, 5, 6, 7, 0, 1, 2], 0 },
            { [11, 13, 15, 17], 11 },
            { [1], 1 },
            { [2, 1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinByLinearScan_LeetCodeExamples_ReturnsMinimum(int[] nums, int expected) =>
        Assert.Equal(expected, FindMinimumInRotatedSortedArraySolution.FindMinByLinearScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinByPredicateSearch_LeetCodeExamples_ReturnsMinimum(int[] nums, int expected) =>
        Assert.Equal(expected, FindMinimumInRotatedSortedArraySolution.FindMinByPredicateSearch(nums));
}
