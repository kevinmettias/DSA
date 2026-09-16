using DSAExperimentation.LeetCode.DivideNodesIntoTheMaximumNumberOfGroups;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DivideNodesIntoTheMaximumNumberOfGroups;

// Harness only. Both strategies are
// DivideNodesIntoTheMaximumNumberOfGroupsSolution's - the hand-rolled
// color/union-find/distance arrays and the BipartiteCheck + KeyedDisjointSet +
// Reduce.Graph composition - pinned here to LeetCode's published examples plus a
// two-component graph (whose bests must be summed independently rather than
// taken from one BFS root across the whole graph), a single edge, and a star
// (whose best root is a leaf, not the centre).
public sealed class DivideNodesIntoTheMaximumNumberOfGroupsTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            { 6, [[1, 2], [1, 4], [1, 5], [2, 6], [2, 3], [4, 6]], 4 },
            { 3, [[1, 2], [2, 3], [3, 1]], -1 },
            { 8, [[1, 2], [2, 3], [4, 5], [5, 6], [6, 7], [7, 8]], 8 },
            { 2, [[1, 2]], 2 },
            { 4, [[1, 2], [1, 3], [1, 4]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MagnificentSetsByArrayAdjacencyBfs_LeetCodeExamples_ReturnsSummedBestGroupingPerComponent(
        int n, int[][] edges, int expected)
    {
        var actual = DivideNodesIntoTheMaximumNumberOfGroupsSolution.MagnificentSetsByArrayAdjacencyBfs(n, edges);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MagnificentSetsByReducePrimitives_LeetCodeExamples_ReturnsSummedBestGroupingPerComponent(
        int n, int[][] edges, int expected)
    {
        var actual = DivideNodesIntoTheMaximumNumberOfGroupsSolution.MagnificentSetsByReducePrimitives(n, edges);
        Assert.Equal(expected, actual);
    }
}
