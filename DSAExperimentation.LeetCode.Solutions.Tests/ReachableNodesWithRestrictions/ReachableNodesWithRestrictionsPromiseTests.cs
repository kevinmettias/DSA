using DSAExperimentation.LeetCode.Solutions.ReachableNodesWithRestrictions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReachableNodesWithRestrictions;

public sealed partial class ReachableNodesWithRestrictionsPromiseTests
{
    public static TheoryData<int, int[][], int[], bool> Inputs =>
        new()
        {
            { 7, [[0, 1], [1, 2], [3, 1], [4, 0], [0, 5], [5, 6]], [4, 5], true },
            { 10, [[0, 1], [0, 2]], [0, 1, 2], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNodeZeroIsFree(int nodeCount, int[][] edges, int[] restricted, bool expected)
        => Assert.Equal(expected, ReachableNodesWithRestrictionsPromise.IsKeptBy(nodeCount, edges, restricted));
}
