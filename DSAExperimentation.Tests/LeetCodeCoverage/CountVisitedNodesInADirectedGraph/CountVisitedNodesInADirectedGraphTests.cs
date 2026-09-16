using DSAExperimentation.LeetCode.CountVisitedNodesInADirectedGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountVisitedNodesInADirectedGraph;

// Harness only. Both strategies live in
// CountVisitedNodesInADirectedGraphSolution - including the per-start forward walk,
// which used to exist only as an unasserted benchmark baseline - and this file pins
// them to the same examples so a failure names the strategy that broke.
public sealed partial class CountVisitedNodesInADirectedGraphTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LeetCode example 1: the cycle 0 -> 1 -> 2 -> 0, with 3 hanging off it.
            { [1, 2, 0, 0], [3, 3, 3, 4] },

            // LeetCode example 2: one cycle spanning every node.
            { [1, 2, 3, 4, 0], [5, 5, 5, 5, 5] },

            // The smallest graph the constraints allow: a single two-node cycle.
            { [1, 0], [2, 2] },

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
    [MemberData(nameof(Examples))]
    public void CountVisitedNodesByPerStartWalk_LeetCodeExamples_ReturnsExpectedVisitCounts(
        int[] edges, int[] expected) =>
        Assert.Equal(
            expected,
            CountVisitedNodesInADirectedGraphSolution.CountVisitedNodesByPerStartWalk(edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVisitedNodesBySccAndReverseBfs_LeetCodeExamples_ReturnsExpectedVisitCounts(
        int[] edges, int[] expected) =>
        Assert.Equal(
            expected,
            CountVisitedNodesInADirectedGraphSolution.CountVisitedNodesBySccAndReverseBfs(edges));
}
