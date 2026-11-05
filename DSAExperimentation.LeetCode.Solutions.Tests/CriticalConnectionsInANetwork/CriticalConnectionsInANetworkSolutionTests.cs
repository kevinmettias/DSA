using DSAExperimentation.LeetCode.Solutions.CriticalConnectionsInANetwork;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CriticalConnectionsInANetwork;

// Harness only. Both strategies live in CriticalConnectionsInANetworkSolution -
// this file pins them to the shapes a bridge
// search has to get right: a ring with no critical connection at all, two cycles
// joined by a single link, and a pure chain where every connection is critical.
//
// LeetCode returns the connections in any order, so each expectation is stated in
// ascending order and the actual result is sorted the same way before comparison -
// membership, not discovery order, is the contract.
public sealed partial class CriticalConnectionsInANetworkSolutionTests
{
    public static TheoryData<int, int[][], int[][]> Cases =>
        new()
        {
            // A ring: every connection sits on the one cycle, so none is critical.
            { 4, [[0, 1], [1, 2], [2, 3], [3, 0]], [] },

            // Two triangles joined by one link - that link is the only bridge.
            { 6, [[0, 1], [1, 2], [2, 0], [3, 4], [4, 5], [5, 3], [2, 3]], [[2, 3]] },

            // A chain has no cycles at all, so every connection is critical.
            { 4, [[0, 1], [1, 2], [2, 3]], [[0, 1], [1, 2], [2, 3]] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CriticalConnectionsByEdgeRemovalScan_Cases_ReturnsEveryDisconnectingLink(
        int serverCount, int[][] connections, int[][] expected)
    {
        var found = CriticalConnectionsInANetworkSolution.CriticalConnectionsByEdgeRemovalScan(
            serverCount, connections);
        var ordered = Ordered(found);
        Assert.Equal(expected, ordered);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CriticalConnectionsByLowLinkSearch_Cases_ReturnsEveryDisconnectingLink(
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
