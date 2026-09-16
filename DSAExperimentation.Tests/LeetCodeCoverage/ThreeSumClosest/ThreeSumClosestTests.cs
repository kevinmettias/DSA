using DSAExperimentation.LeetCode.ThreeSumClosest;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeSumClosest;

// Harness only. Both strategies are ThreeSumClosestSolution's; this file pins them
// to LeetCode's published examples.
public sealed class ThreeSumClosestTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [-1, 2, 1, -4], 1, 2 },
            { [0, 0, 0], 1, 0 },
            { [1, 1, 1, 0], -100, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestSumByBruteForce_LeetCodeExamples_ReturnsNearestTripletSum(
        int[] nums, int target, int expected)
    {
        var actual = ThreeSumClosestSolution.ClosestSumByBruteForce(nums, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClosestSumByMergeSortTwoPointers_LeetCodeExamples_ReturnsNearestTripletSum(
        int[] nums, int target, int expected)
    {
        var actual = ThreeSumClosestSolution.ClosestSumByMergeSortTwoPointers(nums, target);

        Assert.Equal(expected, actual);
    }
}
