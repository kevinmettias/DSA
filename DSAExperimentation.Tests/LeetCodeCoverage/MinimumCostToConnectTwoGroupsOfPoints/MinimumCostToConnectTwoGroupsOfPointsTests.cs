using DSAExperimentation.LeetCode.MinimumCostToConnectTwoGroupsOfPoints;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToConnectTwoGroupsOfPoints;

// Harness only. Both the unmemoized bitmask recursion and the Memoizer-routed one
// live in MinimumCostToConnectTwoGroupsOfPointsSolution; this file pins them to
// LeetCode's published examples plus the degenerate shapes the original test never
// covered, so a disagreement names the strategy that broke.
public sealed partial class MinimumCostToConnectTwoGroupsOfPointsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: pair (0,0) and (1,1) for 15 + 2.
            { [[15, 96], [36, 2]], 17 },

            // LeetCode example 2: group-2 point 2 is covered by point 1's edge, so
            // nothing pays for it twice.
            { [[1, 3, 5], [4, 1, 1], [1, 5, 3]], 4 },

            // LeetCode example 3: five group-1 points over three group-2 points.
            { [[2, 5, 1], [3, 4, 7], [8, 1, 2], [6, 2, 4], [3, 8, 8]], 10 },

            // A single edge is the whole answer.
            { [[7]], 7 },

            // One group-1 point must still cover every group-2 point, so it pays for
            // its own cheapest edge plus each column minimum it did not choose.
            { [[4, 9, 2]], 15 },

            // One group-2 point, so every group-1 point pays its only edge.
            { [[3], [8], [5]], 16 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectTwoGroupsByBruteForceRecursion_LeetCodeExamples_ReturnsMinimumTotalCost(
        int[][] cost, int expected) =>
        Assert.Equal(
            expected, MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByBruteForceRecursion(cost));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectTwoGroupsByMemoizedBitmask_LeetCodeExamples_ReturnsMinimumTotalCost(
        int[][] cost, int expected) =>
        Assert.Equal(
            expected, MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByMemoizedBitmask(cost));
}
