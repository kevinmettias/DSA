using DSAExperimentation.LeetCode.Solutions.MinimumHeightTrees;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumHeightTrees;

// Harness only: the algorithms live in MinimumHeightTreesSolution. One test method
// per strategy over one shared case - the six-node, two-center tree of LeetCode's
// second example with every edge written the other way round - so a failure names
// the strategy that broke. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MinimumHeightTreesSolutionTests
{
    public static TheoryData<int, int[][], int[]> Cases =>
        new()
        {
            { 6, [[0, 3], [1, 3], [2, 3], [4, 3], [5, 4]], [3, 4] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindRootsByHeightFromEveryNode_Cases_ReturnsMinimumHeightRoots(
        int nodeCount, int[][] edges, int[] expected) =>
        Assert.Equal(
            expected.Order(),
            MinimumHeightTreesSolution.FindRootsByHeightFromEveryNode(nodeCount, edges).Order());

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindRootsByLeafPeeling_Cases_ReturnsMinimumHeightRoots(
        int nodeCount, int[][] edges, int[] expected) =>
        Assert.Equal(
            expected.Order(),
            MinimumHeightTreesSolution.FindRootsByLeafPeeling(nodeCount, edges).Order());
}
