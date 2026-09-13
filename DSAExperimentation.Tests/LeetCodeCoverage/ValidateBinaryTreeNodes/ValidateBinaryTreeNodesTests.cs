using DSAExperimentation.LeetCode.ValidateBinaryTreeNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateBinaryTreeNodes;

// Harness only. Both strategies are ValidateBinaryTreeNodesSolution's - the
// DisjointSet one-pass check and the naive per-root re-traversal that used to live
// untested in the benchmark - pinned here to LeetCode's published examples plus the
// two failure shapes the union-find arm exists to catch (a node with two parents,
// and a cycle among non-root nodes).
public sealed class ValidateBinaryTreeNodesTests
{
    public static TheoryData<int, int[], int[], bool> Examples =>
        new()
        {
            { 4, [1, -1, 3, -1], [2, -1, -1, -1], true },
            { 4, [1, -1, 3, -1], [-1, -1, -1, -1], false },
            { 2, [1, 0], [-1, -1], false },
            { 3, [2, 2, -1], [-1, -1, -1], false },
            { 3, [1, 2, 0], [-1, -1, -1], false },
            { 1, [-1], [-1], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidateByDisjointSet_LeetCodeExamples_ReturnsWhetherNodesFormOneTree(
        int n, int[] leftChild, int[] rightChild, bool expected) =>
        Assert.Equal(expected, ValidateBinaryTreeNodesSolution.ValidateByDisjointSet(n, leftChild, rightChild));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidateByRootScan_LeetCodeExamples_ReturnsWhetherNodesFormOneTree(
        int n, int[] leftChild, int[] rightChild, bool expected) =>
        Assert.Equal(expected, ValidateBinaryTreeNodesSolution.ValidateByRootScan(n, leftChild, rightChild));
}
