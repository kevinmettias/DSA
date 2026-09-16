using DSAExperimentation.LeetCode.ShortestSubarrayToBeRemovedToMakeArraySorted;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestSubarrayToBeRemovedToMakeArraySorted;

// Harness only. Both strategies are
// ShortestSubarrayToBeRemovedToMakeArraySortedSolution's - this file just pins them
// to LeetCode's published examples plus the two shapes the stitching arm has to get
// right without any prefix/suffix overlap at all: a strictly decreasing array (only
// one element can survive) and an already-sorted one (nothing is removed).
public sealed partial class ShortestSubarrayToBeRemovedToMakeArraySortedTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 10, 4, 2, 3, 5], 3 },
            { [5, 4, 3, 2, 1], 4 },
            { [1, 2, 3], 0 },
            { [1], 0 },
            { [2, 2, 2, 1, 1, 1], 3 },
            { [1, 2, 3, 5, 4], 1 },
            { [6, 3, 10, 11, 15, 20, 13, 22], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLengthOfShortestSubarrayByBruteForce_LeetCodeExamples_ReturnsMinRemovalLength(
        int[] arr, int expected) =>
        Assert.Equal(
            expected,
            ShortestSubarrayToBeRemovedToMakeArraySortedSolution.FindLengthOfShortestSubarrayByBruteForce(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLengthOfShortestSubarrayByBinarySearchStitch_LeetCodeExamples_ReturnsMinRemovalLength(
        int[] arr, int expected) =>
        Assert.Equal(
            expected,
            ShortestSubarrayToBeRemovedToMakeArraySortedSolution.FindLengthOfShortestSubarrayByBinarySearchStitch(arr));
}
