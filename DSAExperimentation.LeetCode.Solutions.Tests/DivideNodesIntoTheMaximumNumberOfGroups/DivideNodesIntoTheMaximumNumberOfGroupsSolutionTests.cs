using DSAExperimentation.LeetCode.Solutions.DivideNodesIntoTheMaximumNumberOfGroups;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DivideNodesIntoTheMaximumNumberOfGroups;

// Harness only. Both strategies are
// DivideNodesIntoTheMaximumNumberOfGroupsSolution's - the hand-rolled
// color/union-find/distance arrays and the BipartiteCheck + KeyedDisjointSet +
// Reduce.Graph composition - pinned here to a
// two-component graph (whose bests must be summed independently rather than
// taken from one BFS root across the whole graph), a single edge, and a star
// (whose best root is a leaf, not the centre).
public sealed partial class DivideNodesIntoTheMaximumNumberOfGroupsSolutionTests
{
    public static TheoryData<int, int[][], int> Cases =>
        new()
        {
            { 8, [[1, 2], [2, 3], [4, 5], [5, 6], [6, 7], [7, 8]], 8 },
            { 2, [[1, 2]], 2 },
            { 4, [[1, 2], [1, 3], [1, 4]], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MagnificentSetsByArrayAdjacencyBfs_Cases_ReturnsSummedBestGroupingPerComponent(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = DivideNodesIntoTheMaximumNumberOfGroupsSolution.MagnificentSetsByArrayAdjacencyBfs(nodeCount, edges);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MagnificentSetsByReducePrimitives_Cases_ReturnsSummedBestGroupingPerComponent(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = DivideNodesIntoTheMaximumNumberOfGroupsSolution.MagnificentSetsByReducePrimitives(nodeCount, edges);
        Assert.Equal(expected, actual);
    }
}
