using DSAExperimentation.LeetCode.Solutions.RemoveMaxNumberOfEdgesToKeepGraphFullyTraversable;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveMaxNumberOfEdgesToKeepGraphFullyTraversable;

// Harness only. Both strategies are
// RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution's - this file pins them to
// two cases the original coverage left untested: Alice alone being unable to
// traverse (LeetCode's third example fails both Alice and Bob), and a graph whose
// type-3 edges alone already span every node, so every single-owner edge is
// removable. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolutionTests
{
    public static TheoryData<int, int[][], int> Cases =>
        new()
        {
            { 4, [[3, 2, 3], [2, 1, 2], [1, 3, 4], [2, 3, 4]], -1 },
            {
                3,
                [[3, 1, 2], [3, 2, 3], [1, 1, 3], [2, 1, 3], [1, 1, 2]],
                3
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxNumberOfEdgesToRemoveByFloodFill_Cases_ReturnsRemovableEdgeCount(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution.MaxNumberOfEdgesToRemoveByFloodFill(nodeCount, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxNumberOfEdgesToRemoveByDisjointSet_Cases_ReturnsRemovableEdgeCount(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution.MaxNumberOfEdgesToRemoveByDisjointSet(nodeCount, edges);

        Assert.Equal(expected, actual);
    }
}
