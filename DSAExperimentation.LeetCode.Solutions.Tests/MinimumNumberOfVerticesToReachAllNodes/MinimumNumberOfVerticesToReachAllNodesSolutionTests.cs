using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfVerticesToReachAllNodes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfVerticesToReachAllNodes;

// Harness only. Both strategies are
// MinimumNumberOfVerticesToReachAllNodesSolution's - two edgeless graphs (every node
// is its own source) are asserted against each, so the nested-scan baseline is now
// held to the same answers as the marking pass. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MinimumNumberOfVerticesToReachAllNodesSolutionTests
{
    public static TheoryData<int, int[][], int[]> Cases =>
        new()
        {
            { 3, [], [0, 1, 2] },
            { 1, [], [0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindSmallestSetOfVerticesByNestedScan_Cases_ReturnsEveryZeroInDegreeNode(
        int nodeCount, int[][] edges, int[] expected)
    {
        var actual = MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByNestedScan(nodeCount, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindSmallestSetOfVerticesByInDegreeSet_Cases_ReturnsEveryZeroInDegreeNode(
        int nodeCount, int[][] edges, int[] expected)
    {
        var actual = MinimumNumberOfVerticesToReachAllNodesSolution.FindSmallestSetOfVerticesByInDegreeSet(nodeCount, edges);

        Assert.Equal(expected, actual);
    }
}
