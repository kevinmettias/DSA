using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInABinarySearchTree;

// LeetCode 700. Search in a Binary Search Tree: the same "compare against the
// current node, descend left or right" walk this repo's own
// BinarySearchTree<TValue>.Has already performs internally, but returning the
// matching BinaryTreeNode<TValue> itself instead of a bare bool - so its own
// subtree comes along for free, exactly what LeetCode expects back. The same
// FindNode shape LowestCommonAncestorOfBstTests already uses, minus its "value is
// always present" presumption, since this problem's own examples include a val
// that is not in the tree.
public sealed partial class SearchInABinarySearchTreeTests
{
    [Fact]
    public void SearchBst_ValuePresent_ReturnsSubtreeRootedAtThatValue()
    {
        var root = BuildTree(4, 2, 7, 1, 3);

        var found = SearchBst(root, 2);

        Assert.NotNull(found);
        Assert.Equal(2, found!.Value);
        Assert.Equal(1, found.Left!.Value);
        Assert.Equal(3, found.Right!.Value);
    }

    [Fact]
    public void SearchBst_ValueAbsent_ReturnsNull()
    {
        var root = BuildTree(4, 2, 7, 1, 3);

        var found = SearchBst(root, 5);

        Assert.Null(found);
    }

    [Fact]
    public void SearchBst_ValueIsRoot_ReturnsWholeTree()
    {
        var root = BuildTree(4, 2, 7, 1, 3);

        var found = SearchBst(root, 4);

        Assert.Same(root, found);
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

    private static BinaryTreeNode<int>? SearchBst(BinaryTreeNode<int>? root, int val)
    {
        var node = root;

        while (node is not null && node.Value != val)
        {
            node = val < node.Value ? node.Left : node.Right;
        }

        return node;
    }
}
