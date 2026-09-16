using DSAExperimentation.LeetCode.ReachableNodesInSubdividedGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReachableNodesInSubdividedGraph;

// Harness only. The weighted original graph is SubdividedGraph and both strategies -
// the materializing BFS baseline and the Dijkstra-plus-arithmetic composition - are
// ReachableNodesInSubdividedGraphSolution's; this file just pins them to LeetCode's
// published examples plus the boundary cases the pre-migration test carried.
public sealed class ReachableNodesInSubdividedGraphTests
{
    public static TheoryData<int[][], int, int, int> Examples =>
        new()
        {
            // LeetCode example 1.
            { [[0, 1, 10], [1, 2, 1], [0, 2, 2]], 6, 3, 13 },

            // LeetCode example 2.
            { [[0, 1, 4], [1, 2, 6], [0, 2, 8], [1, 3, 1]], 10, 4, 23 },

            // LeetCode example 3: node 0 is isolated, so only the source is reachable.
            { [[1, 2, 4], [1, 4, 5], [1, 3, 1], [2, 3, 4], [3, 4, 5]], 17, 5, 1 },

            // No budget at all: the source and nothing else.
            { [[0, 1, 10], [1, 2, 1], [0, 2, 2]], 0, 3, 1 },

            // No subdivisions anywhere: degenerates to plain unweighted reachability.
            { [[0, 1, 0], [1, 2, 0], [0, 2, 0]], 1, 3, 3 },

            // Budget large enough to reach every node of every chain.
            { [[0, 1, 3], [1, 2, 2]], 100, 3, 8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountReachableNodesByMaterializedBfs_LeetCodeExamples_CountsOriginalAndSubdivisionNodes(
        int[][] edges, int maxMoves, int nodeCount, int expected)
    {
        var actual = ReachableNodesInSubdividedGraphSolution.CountReachableNodesByMaterializedBfs(
            edges, maxMoves, nodeCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountReachableNodesByDijkstra_LeetCodeExamples_CountsOriginalAndSubdivisionNodes(
        int[][] edges, int maxMoves, int nodeCount, int expected)
    {
        var actual = ReachableNodesInSubdividedGraphSolution.CountReachableNodesByDijkstra(
            edges, maxMoves, nodeCount);

        Assert.Equal(expected, actual);
    }
}
