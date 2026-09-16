using DSAExperimentation.LeetCode.MinimumNumberOfVerticesToReachAllNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfVerticesToReachAllNodes;

// Harness only. Both strategies are
// MinimumNumberOfVerticesToReachAllNodesSolution's - LeetCode's two published
// examples plus the edgeless graph (every node is its own source) are asserted
// against each, so the nested-scan baseline is now held to the same answers as the
// marking pass.
public sealed class MinimumNumberOfVerticesToReachAllNodesTests
{
    public static TheoryData<int, int[][], int[]> Examples =>
        new()
        {
            { 6, [[0, 1], [0, 2], [2, 5], [3, 4], [4, 2]], [0, 3] },
            { 5, [[0, 1], [2, 1], [3, 1], [1, 4], [2, 4]], [0, 2, 3] },
            { 3, [], [0, 1, 2] },
            { 1, [], [0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSmallestSetOfVerticesByNestedScan_LeetCodeExamples_ReturnsEveryZeroInDegreeNode(
        int nodeCount, int[][] edges, int[] expected)
    {
        var actual = MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByNestedScan(nodeCount, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSmallestSetOfVerticesByInDegreeSet_LeetCodeExamples_ReturnsEveryZeroInDegreeNode(
        int nodeCount, int[][] edges, int[] expected)
    {
        var actual = MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByInDegreeSet(nodeCount, edges);

        Assert.Equal(expected, actual);
    }
}
