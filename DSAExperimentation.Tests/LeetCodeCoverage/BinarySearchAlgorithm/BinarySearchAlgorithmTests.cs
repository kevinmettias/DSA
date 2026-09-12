using DSAExperimentation.LeetCode.BinarySearchAlgorithm;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinarySearchAlgorithm;

// Harness only. Both strategies are BinarySearchAlgorithmSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class BinarySearchAlgorithmTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [-1, 0, 3, 5, 9, 12], 9, 4 },
            { [-1, 0, 3, 5, 9, 12], 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindIndexByLinearScan_LeetCodeExamples_ReturnsIndexOrNegativeOne(
        int[] nums, int target, int expected) =>
        Assert.Equal(expected, BinarySearchAlgorithmSolution.FindIndexByLinearScan(nums, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindIndexByBinarySearch_LeetCodeExamples_ReturnsIndexOrNegativeOne(
        int[] nums, int target, int expected) =>
        Assert.Equal(expected, BinarySearchAlgorithmSolution.FindIndexByBinarySearch(nums, target));
}
