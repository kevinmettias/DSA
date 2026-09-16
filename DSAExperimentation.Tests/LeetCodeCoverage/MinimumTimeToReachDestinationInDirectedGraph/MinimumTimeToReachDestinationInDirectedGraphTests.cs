using DSAExperimentation.LeetCode.MinimumTimeToReachDestinationInDirectedGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToReachDestinationInDirectedGraph;

// Harness only. Both strategies are
// MinimumTimeToReachDestinationInDirectedGraphSolution's - this file just pins
// them to LeetCode's published examples, including the unreachable case where
// node 0 has no outgoing edge at all.
public sealed class MinimumTimeToReachDestinationInDirectedGraphTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            { 3, [[0, 1, 0, 1], [1, 2, 2, 5]], 3 },
            { 4, [[0, 1, 0, 3], [1, 3, 7, 8], [0, 2, 1, 5], [2, 3, 4, 7]], 5 },
            { 3, [[1, 0, 1, 3], [1, 2, 3, 5]], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByBclPriorityQueue_LeetCodeExamples_ReturnsEarliestArrivalAtLastNode(
        int vertexCount, int[][] edges, int expected)
    {
        var actual =
            MinimumTimeToReachDestinationInDirectedGraphSolution.MinimumTimeByBclPriorityQueue(vertexCount, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByHeap_LeetCodeExamples_ReturnsEarliestArrivalAtLastNode(
        int vertexCount, int[][] edges, int expected)
    {
        var actual = MinimumTimeToReachDestinationInDirectedGraphSolution.MinimumTimeByHeap(vertexCount, edges);

        Assert.Equal(expected, actual);
    }
}
