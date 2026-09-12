using DSAExperimentation.LeetCode.RedundantConnection;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RedundantConnection;

// LeetCode 684. Redundant Connection: given a tree with one extra edge added, find
// the extra edge - the first edge that connects two nodes already in the same
// DisjointSet component. Harness only: the strategy is
// RedundantConnectionSolution's.
public sealed class RedundantConnectionTests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[1, 2], [1, 3], [2, 3]], [2, 3] },
            { [[1, 2], [2, 3], [3, 4], [1, 4], [1, 5]], [1, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRedundantEdgeByDisjointSet_LeetCodeExamples_ReturnsTheCycleClosingEdge(
        int[][] edges, int[] expected) =>
        Assert.Equal(expected, RedundantConnectionSolution.FindRedundantEdgeByDisjointSet(edges));
}
