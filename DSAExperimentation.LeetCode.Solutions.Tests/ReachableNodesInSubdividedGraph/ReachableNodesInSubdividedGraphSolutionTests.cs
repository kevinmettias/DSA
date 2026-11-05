using DSAExperimentation.LeetCode.Solutions.ReachableNodesInSubdividedGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReachableNodesInSubdividedGraph;

// Harness only. The weighted original graph is SubdividedGraph and both strategies -
// the materializing BFS baseline and the Dijkstra-plus-arithmetic composition - are
// ReachableNodesInSubdividedGraphSolution's; this file just pins them to a variant
// of LeetCode's first example plus the boundary cases the pre-migration test
// carried. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class ReachableNodesInSubdividedGraphSolutionTests
{
    public static TheoryData<int[][], int, int, int> Cases =>
        new()
        {
            // LeetCode's first example with the subdivision counts of its two short
            // edges swapped (1-2 has 1, 0-2 has 2); the answer is still 13.
            { [[0, 1, 10], [1, 2, 1], [0, 2, 2]], 6, 3, 13 },

            // No budget at all: the source and nothing else.
            { [[0, 1, 10], [1, 2, 1], [0, 2, 2]], 0, 3, 1 },

            // No subdivisions anywhere: degenerates to plain unweighted reachability.
            { [[0, 1, 0], [1, 2, 0], [0, 2, 0]], 1, 3, 3 },

            // Budget large enough to reach every node of every chain.
            { [[0, 1, 3], [1, 2, 2]], 100, 3, 8 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountReachableNodesByMaterializedBfs_Cases_CountsOriginalAndSubdivisionNodes(
        int[][] edges, int maxMoves, int nodeCount, int expected)
    {
        var actual = ReachableNodesInSubdividedGraphSolution.CountReachableNodesByMaterializedBfs(
            edges, maxMoves, nodeCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountReachableNodesByDijkstra_Cases_CountsOriginalAndSubdivisionNodes(
        int[][] edges, int maxMoves, int nodeCount, int expected)
    {
        var actual = ReachableNodesInSubdividedGraphSolution.CountReachableNodesByDijkstra(
            edges, maxMoves, nodeCount);

        Assert.Equal(expected, actual);
    }
}
