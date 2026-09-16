using DSAExperimentation.LeetCode.CriticalConnectionsInANetwork;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CriticalConnectionsInANetwork;

// Harness only. Both strategies live in CriticalConnectionsInANetworkSolution -
// this file pins them to LeetCode's published examples plus the shapes a bridge
// search has to get right: a ring with no critical connection at all, two cycles
// joined by a single link, and a pure chain where every connection is critical.
//
// LeetCode returns the connections in any order, so each expectation is stated in
// ascending order and the actual result is sorted the same way before comparison -
// membership, not discovery order, is the contract.
public sealed class CriticalConnectionsInANetworkTests
{
    public static TheoryData<int, int[][], int[][]> Examples =>
        new()
        {
            // LC example 1: only the pendant link to server 3 is critical.
            { 4, [[0, 1], [1, 2], [2, 0], [1, 3]], [[1, 3]] },

            // LC example 2: a lone connection is always critical.
            { 2, [[0, 1]], [[0, 1]] },

            // A ring: every connection sits on the one cycle, so none is critical.
            { 4, [[0, 1], [1, 2], [2, 3], [3, 0]], [] },

            // Two triangles joined by one link - that link is the only bridge.
            { 6, [[0, 1], [1, 2], [2, 0], [3, 4], [4, 5], [5, 3], [2, 3]], [[2, 3]] },

            // A chain has no cycles at all, so every connection is critical.
            { 4, [[0, 1], [1, 2], [2, 3]], [[0, 1], [1, 2], [2, 3]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CriticalConnectionsByEdgeRemovalScan_LeetCodeExamples_ReturnsEveryDisconnectingLink(
        int serverCount, int[][] connections, int[][] expected)
    {
        var found = CriticalConnectionsInANetworkSolution.CriticalConnectionsByEdgeRemovalScan(
            serverCount, connections);
        var ordered = Ordered(found);
        Assert.Equal(expected, ordered);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CriticalConnectionsByLowLinkSearch_LeetCodeExamples_ReturnsEveryDisconnectingLink(
        int serverCount, int[][] connections, int[][] expected)
    {
        var found = CriticalConnectionsInANetworkSolution.CriticalConnectionsByLowLinkSearch(
            serverCount, connections);
        var ordered = Ordered(found);
        Assert.Equal(expected, ordered);
    }

    private static int[][] Ordered(int[][] connections) =>
        [.. connections.OrderBy(connection => connection[0]).ThenBy(connection => connection[1])];
}
