using DSAExperimentation.LeetCode.Solutions.CountVisitedNodesInADirectedGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountVisitedNodesInADirectedGraph;

// Harness only. Both strategies live in
// CountVisitedNodesInADirectedGraphSolution - including the per-start forward walk,
// which used to exist only as an unasserted benchmark baseline - and this file pins
// them to the same cases so a failure names the strategy that broke.
public sealed partial class CountVisitedNodesInADirectedGraphSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            // Two disjoint two-node cycles, so no node ever reaches the other pair.
            { [1, 0, 3, 2], [2, 2, 2, 2] },

            // A two-node cycle with a two-node tail feeding into it, which is the
            // case that forces the outward propagation to run more than one layer.
            { [1, 0, 0, 2], [2, 2, 3, 4] },

            // A three-node cycle with a two-node tail: node 4 answers 1 + node 3's
            // answer, which is itself 1 + the cycle's length.
            { [1, 2, 0, 0, 3], [3, 3, 3, 4, 5] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountVisitedNodesByPerStartWalk_Cases_ReturnsExpectedVisitCounts(
        int[] edges, int[] expected) =>
        Assert.Equal(
            expected,
            CountVisitedNodesInADirectedGraphSolution.CountVisitedNodesByPerStartWalk(edges));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountVisitedNodesBySccAndReverseBfs_Cases_ReturnsExpectedVisitCounts(
        int[] edges, int[] expected) =>
        Assert.Equal(
            expected,
            CountVisitedNodesInADirectedGraphSolution.CountVisitedNodesBySccAndReverseBfs(edges));
}
