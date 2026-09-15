using DSAExperimentation.LeetCode.ReachableNodesWithRestrictions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReachableNodesWithRestrictions;

// Harness only: both strategies live in ReachableNodesWithRestrictionsSolution and are
// asserted here under their own names, so a failure names the strategy that broke. The
// cases are LeetCode's two published examples plus the boundaries the restriction rule
// turns on - nothing restricted at all, a single node with no edges, and a restriction
// sitting directly on node 0's only neighbour so the answer collapses to the root.
public sealed class ReachableNodesWithRestrictionsTests
{
    public static TheoryData<int, int[][], int[], int> Examples => new()
    {
        { 7, [[0, 1], [1, 2], [3, 1], [4, 0], [0, 5], [5, 6]], [4, 5], 4 },
        { 7, [[0, 1], [0, 2], [0, 5], [0, 4], [3, 2], [6, 5]], [4, 2, 1], 3 },
        { 4, [[0, 1], [1, 2], [2, 3]], [], 4 },
        { 3, [[0, 1], [1, 2]], [1], 1 },
        { 1, [], [], 1 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReachableNodesByDepthFirstFloodFill_Example_CountsNodesStillReachableFromZero(
        int nodeCount, int[][] edges, int[] restricted, int expected) =>
        Assert.Equal(
            expected,
            ReachableNodesWithRestrictionsSolution.ReachableNodesByDepthFirstFloodFill(nodeCount, edges, restricted));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReachableNodesByDisjointSet_Example_CountsNodesStillReachableFromZero(
        int nodeCount, int[][] edges, int[] restricted, int expected) =>
        Assert.Equal(
            expected,
            ReachableNodesWithRestrictionsSolution.ReachableNodesByDisjointSet(nodeCount, edges, restricted));
}
