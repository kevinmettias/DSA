using DSAExperimentation.LeetCode.RedundantConnection;

namespace DSAExperimentation.LeetCode.Tests.RedundantConnection;

// LeetCode 684. Redundant Connection: given a tree with one extra edge added, find
// the extra edge - the first edge that connects two nodes already in the same
// DisjointSet component. Harness only: the strategy is
// RedundantConnectionSolution's.
public sealed partial class RedundantConnectionSolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRedundantEdgeByPathSearch_LeetCodeExamples_ReturnsTheCycleClosingEdge(
        int[][] edges, int[] expected) =>
        Assert.Equal(expected, RedundantConnectionSolution.FindRedundantEdgeByPathSearch(edges));

    // The two arms are competing strategies for one question, so they must name the same edge -
    // not merely both name *an* edge that closes a cycle. The second example is the one that
    // separates them: [1,4] closes a cycle only after the forest has been built through
    // [1,2],[2,3],[3,4], which the walk has to discover while the disjoint set already knows it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRedundantEdge_AgreeOnEveryExample(int[][] edges, int[] expected) =>
        Assert.Equal(
            RedundantConnectionSolution.FindRedundantEdgeByDisjointSet(edges),
            RedundantConnectionSolution.FindRedundantEdgeByPathSearch(edges));
}
