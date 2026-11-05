using DSAExperimentation.LeetCode.Solutions.MinimumCostToConnectTwoGroupsOfPoints;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostToConnectTwoGroupsOfPoints;

// Harness only. Both the unmemoized bitmask recursion and the Memoizer-routed one
// live in MinimumCostToConnectTwoGroupsOfPointsSolution; this file pins them to
// the degenerate shapes the original test never
// covered, so a disagreement names the strategy that broke.
public sealed partial class MinimumCostToConnectTwoGroupsOfPointsSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // A single edge is the whole answer.
            { [[7]], 7 },

            // One group-1 point must still cover every group-2 point, so it pays for
            // its own cheapest edge plus each column minimum it did not choose.
            { [[4, 9, 2]], 15 },

            // One group-2 point, so every group-1 point pays its only edge.
            { [[3], [8], [5]], 16 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ConnectTwoGroupsByBruteForceRecursion_Cases_ReturnsMinimumTotalCost(
        int[][] cost, int expected) =>
        Assert.Equal(
            expected, MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByBruteForceRecursion(cost));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ConnectTwoGroupsByMemoizedBitmask_Cases_ReturnsMinimumTotalCost(
        int[][] cost, int expected) =>
        Assert.Equal(
            expected, MinimumCostToConnectTwoGroupsOfPointsSolution.ConnectTwoGroupsByMemoizedBitmask(cost));
}
