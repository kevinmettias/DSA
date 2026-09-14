using DSAExperimentation.LeetCode.MinimumXorSumOfTwoArrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumXorSumOfTwoArrays;

// Harness only. Both strategies are MinimumXorSumOfTwoArraysSolution's -
// MinimumXorSumByBruteForceRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm) now faces the same examples as
// MinimumXorSumByMemoizedBitmask (previously this file's own private helper), so a
// failure names the strategy that broke. Alongside LeetCode's two published examples
// the cases pin down the single-element base case, a pairing that can reach zero, and
// a pair where matching the two smallest values together is NOT optimal - the trap a
// greedy arm would fall into.
public sealed class MinimumXorSumOfTwoArraysTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 2], [2, 3], 2 },
            { [1, 0, 3], [5, 3, 4], 8 },
            { [7], [5], 2 },
            { [0, 0], [0, 0], 0 },
            { [1, 2, 3], [3, 2, 1], 0 },
            { [3, 5], [4, 6], 6 },
            { [1, 2, 4, 8], [8, 4, 2, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumXorSumByBruteForceRecursion_LeetCodeExamples_ReturnsMinimalTotal(
        int[] nums1, int[] nums2, int expected) =>
        Assert.Equal(expected, MinimumXorSumOfTwoArraysSolution.MinimumXorSumByBruteForceRecursion(nums1, nums2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumXorSumByMemoizedBitmask_LeetCodeExamples_ReturnsMinimalTotal(
        int[] nums1, int[] nums2, int expected) =>
        Assert.Equal(expected, MinimumXorSumOfTwoArraysSolution.MinimumXorSumByMemoizedBitmask(nums1, nums2));
}
