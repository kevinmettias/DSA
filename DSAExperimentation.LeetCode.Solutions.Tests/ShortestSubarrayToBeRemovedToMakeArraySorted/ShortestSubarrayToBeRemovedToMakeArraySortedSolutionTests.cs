using DSAExperimentation.LeetCode.Solutions.ShortestSubarrayToBeRemovedToMakeArraySorted;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShortestSubarrayToBeRemovedToMakeArraySorted;

// Harness only. Both strategies are
// ShortestSubarrayToBeRemovedToMakeArraySortedSolution's - this file just pins them
// to the two shapes the stitching arm has to get
// right without any prefix/suffix overlap at all: a strictly decreasing array (only
// one element can survive) and an already-sorted one (nothing is removed).
public sealed partial class ShortestSubarrayToBeRemovedToMakeArraySortedSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 2, 2, 1, 1, 1], 3 },
            { [1, 2, 3, 5, 4], 1 },
            { [6, 3, 10, 11, 15, 20, 13, 22], 5 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLengthOfShortestSubarrayByBruteForce_Cases_ReturnsMinRemovalLength(
        int[] arr, int expected) =>
        Assert.Equal(
            expected,
            ShortestSubarrayToBeRemovedToMakeArraySortedSolution.FindLengthOfShortestSubarrayByBruteForce(arr));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLengthOfShortestSubarrayByBinarySearchStitch_Cases_ReturnsMinRemovalLength(
        int[] arr, int expected) =>
        Assert.Equal(
            expected,
            ShortestSubarrayToBeRemovedToMakeArraySortedSolution.FindLengthOfShortestSubarrayByBinarySearchStitch(arr));
}
