using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SearchInABinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInABinarySearchTree;

// Harness only. Both strategies are SearchInABinarySearchTreeSolution's; this
// file pins them to LeetCode's published examples, including the one where the
// value is absent and the one where the value is the root itself (its own
// subtree - the whole tree - comes back).
public sealed class SearchInABinarySearchTreeTests
{
    public static TheoryData<int[], int, int?, int?, int?> Examples =>
        new()
        {
            { [4, 2, 7, 1, 3], 2, 2, 1, 3 },
            { [4, 2, 7, 1, 3], 5, null, null, null },
            { [4, 2, 7, 1, 3], 4, 4, 2, 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchBstByLinearScan_LeetCodeExamples_ReturnsSubtreeRootedAtValue(
        int[] treeValues, int val, int? expectedValue, int? expectedLeft, int? expectedRight)
    {
        var root = BuildTree(treeValues);

        var found = SearchInABinarySearchTreeSolution.SearchBstByLinearScan(root, val);

        AssertFound(found, expectedValue, expectedLeft, expectedRight);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchBstByBstDescent_LeetCodeExamples_ReturnsSubtreeRootedAtValue(
        int[] treeValues, int val, int? expectedValue, int? expectedLeft, int? expectedRight)
    {
        var root = BuildTree(treeValues);

        var found = SearchInABinarySearchTreeSolution.SearchBstByBstDescent(root, val);

        AssertFound(found, expectedValue, expectedLeft, expectedRight);
    }

    private static void AssertFound(
        BinaryTreeNode<int>? found, int? expectedValue, int? expectedLeft, int? expectedRight)
    {
        if (expectedValue is null)
        {
            Assert.Null(found);
            return;
        }

        Assert.NotNull(found);
        Assert.Equal(expectedValue, found!.Value);
        Assert.Equal(expectedLeft, found.Left?.Value);
        Assert.Equal(expectedRight, found.Right?.Value);
    }

    private static BinaryTreeNode<int> BuildTree(params int[] values)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        // presumption: allow -- every call site in this file passes a non-empty
        // values array, so at least one Insert above always ran and Root is never
        // null here.
        return tree.Root!;
    }
}
