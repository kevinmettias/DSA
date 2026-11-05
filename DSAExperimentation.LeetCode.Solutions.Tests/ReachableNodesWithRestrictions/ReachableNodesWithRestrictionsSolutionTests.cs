using DSAExperimentation.LeetCode.Solutions.ReachableNodesWithRestrictions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReachableNodesWithRestrictions;

// Harness only: both strategies live in ReachableNodesWithRestrictionsSolution and are
// asserted here under their own names, so a failure names the strategy that broke. The
// cases are the boundaries the restriction rule
// turns on - nothing restricted at all, a single node with no edges, and a restriction
// sitting directly on node 0's only neighbour so the answer collapses to the root.
public sealed partial class ReachableNodesWithRestrictionsSolutionTests
{
    public static TheoryData<int, int[][], int[], int> Cases => new()
    {
        { 4, [[0, 1], [1, 2], [2, 3]], [], 4 },
        { 3, [[0, 1], [1, 2]], [1], 1 },
        { 1, [], [], 1 },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReachableNodesByDepthFirstFloodFill_Example_CountsNodesStillReachableFromZero(
        int nodeCount, int[][] edges, int[] restricted, int expected)
    {
        var actual = ReachableNodesWithRestrictionsSolution.ReachableNodesByDepthFirstFloodFill(
            nodeCount, edges, restricted);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReachableNodesByDisjointSet_Example_CountsNodesStillReachableFromZero(
        int nodeCount, int[][] edges, int[] restricted, int expected)
    {
        var actual = ReachableNodesWithRestrictionsSolution.ReachableNodesByDisjointSet(
            nodeCount, edges, restricted);

        Assert.Equal(expected, actual);
    }
}
