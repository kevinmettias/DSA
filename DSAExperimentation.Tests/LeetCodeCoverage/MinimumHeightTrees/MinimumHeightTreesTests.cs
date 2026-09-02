using DSAExperimentation.LeetCode.MinimumHeightTrees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumHeightTrees;

// Harness only: the algorithms live in MinimumHeightTreesSolution. One test method
// per strategy over one shared set of LeetCode's own examples, so a failure names
// the strategy that broke.
public sealed class MinimumHeightTreesTests
{
    public static TheoryData<int, int[][], int[]> Examples =>
        new()
        {
            { 1, [], [0] },
            { 4, [[1, 0], [1, 2], [1, 3]], [1] },
            { 6, [[0, 3], [1, 3], [2, 3], [4, 3], [5, 4]], [3, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRootsByHeightFromEveryNode_LeetCodeExamples_ReturnsMinimumHeightRoots(
        int n, int[][] edges, int[] expected) =>
        Assert.Equal(
            expected.Order(),
            MinimumHeightTreesSolution.FindRootsByHeightFromEveryNode(n, edges).Order());

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRootsByLeafPeeling_LeetCodeExamples_ReturnsMinimumHeightRoots(
        int n, int[][] edges, int[] expected) =>
        Assert.Equal(
            expected.Order(),
            MinimumHeightTreesSolution.FindRootsByLeafPeeling(n, edges).Order());
}
