using DSAExperimentation.LeetCode.Solutions.MinimumXorSumOfTwoArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumXorSumOfTwoArrays;

// Harness only. Both strategies are MinimumXorSumOfTwoArraysSolution's -
// MinimumXorSumByBruteForceRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm) now faces the same cases as
// MinimumXorSumByMemoizedBitmask (previously this file's own private helper), so a
// failure names the strategy that broke. The cases pin down the single-element base
// case, pairings that can reach zero, and a pair where matching the two smallest
// values together is NOT optimal - the trap a greedy arm would fall into. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class MinimumXorSumOfTwoArraysSolutionTests
{
    public static TheoryData<int[], int[], int> Cases =>
        new()
        {
            { [7], [5], 2 },
            { [0, 0], [0, 0], 0 },
            { [1, 2, 3], [3, 2, 1], 0 },
            { [3, 5], [4, 6], 6 },
            { [1, 2, 4, 8], [8, 4, 2, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumXorSumByBruteForceRecursion_Cases_ReturnsMinimalTotal(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumXorSumOfTwoArraysSolution.MinimumXorSumByBruteForceRecursion(nums1, nums2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumXorSumByMemoizedBitmask_Cases_ReturnsMinimalTotal(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumXorSumOfTwoArraysSolution.MinimumXorSumByMemoizedBitmask(nums1, nums2);

        Assert.Equal(expected, actual);
    }
}
