using DSAExperimentation.LeetCode.RemoveMaxNumberOfEdgesToKeepGraphFullyTraversable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveMaxNumberOfEdgesToKeepGraphFullyTraversable;

// Harness only. Both strategies are
// RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution's - this file pins them to
// LeetCode's three published examples plus two the original coverage left untested:
// Alice alone being unable to traverse (the mirror of example 3, which only fails Bob),
// and a graph whose type-3 edges alone already span every node, so every single-owner
// edge is removable.
public sealed class RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            {
                4,
                [[3, 1, 2], [3, 2, 3], [1, 1, 3], [1, 2, 4], [1, 1, 2], [2, 3, 4]],
                2
            },
            { 4, [[3, 1, 2], [3, 2, 3], [1, 1, 4], [2, 1, 4]], 0 },
            { 4, [[3, 2, 3], [1, 1, 2], [2, 3, 4]], -1 },
            { 4, [[3, 2, 3], [2, 1, 2], [1, 3, 4], [2, 3, 4]], -1 },
            {
                3,
                [[3, 1, 2], [3, 2, 3], [1, 1, 3], [2, 1, 3], [1, 1, 2]],
                3
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxNumEdgesToRemoveByFloodFill_LeetCodeExamples_ReturnsRemovableEdgeCount(
        int n, int[][] edges, int expected) =>
        Assert.Equal(
            expected,
            RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution.MaxNumEdgesToRemoveByFloodFill(n, edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxNumEdgesToRemoveByDisjointSet_LeetCodeExamples_ReturnsRemovableEdgeCount(
        int n, int[][] edges, int expected) =>
        Assert.Equal(
            expected,
            RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution.MaxNumEdgesToRemoveByDisjointSet(n, edges));
}
