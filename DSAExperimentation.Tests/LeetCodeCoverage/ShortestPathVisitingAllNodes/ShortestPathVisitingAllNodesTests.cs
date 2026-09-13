using DSAExperimentation.LeetCode.ShortestPathVisitingAllNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathVisitingAllNodes;

// Harness only. The (node, visited-mask) state space is VisitStateGraph's and
// both searches are ShortestPathVisitingAllNodesSolution's - the true
// multi-source BFS baseline and the Reduce.Graph composition - pinned here to
// LeetCode's published examples plus the degenerate shapes: one node (already
// done, zero steps), one edge, and a path graph.
public sealed class ShortestPathVisitingAllNodesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 2, 3], [0], [0], [0]], 4 },
            { [[1], [0, 2, 4], [1, 3, 4], [2], [1, 2]], 4 },
            { [[1], [0, 2], [1]], 2 },
            { [[]], 0 },
            { [[1], [0]], 1 },
            { [[1, 2], [0, 2], [0, 1]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathLengthByMutationQueue_LeetCodeExamples_ReturnsShortestWalkVisitingEveryNode(
        int[][] graph, int expected) =>
        Assert.Equal(expected, ShortestPathVisitingAllNodesSolution.ShortestPathLengthByMutationQueue(graph));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathLengthByReduceGraph_LeetCodeExamples_ReturnsShortestWalkVisitingEveryNode(
        int[][] graph, int expected) =>
        Assert.Equal(expected, ShortestPathVisitingAllNodesSolution.ShortestPathLengthByReduceGraph(graph));
}
