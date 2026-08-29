using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenBinaryTreeToLinkedList;

public sealed partial class FlattenBinaryTreeToLinkedListTests
{
    [Fact]
    public void Flatten_ClassicExample_RewritesToPreorderRightChain()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(2) { Left = new(3), Right = new(4) }, Right = new(5) { Right = new(6) } };
        Flatten(root);
        Assert.Equal([1, 2, 3, 4, 5, 6], RightChain(root));
    }

    private static BinaryTreeNode<int>? Flatten(BinaryTreeNode<int>? node)
    {
        if (node is null) return null;
        var leftTail = Flatten(node.Left); var rightTail = Flatten(node.Right);
        if (leftTail is not null) { leftTail.Right = node.Right; node.Right = node.Left; node.Left = null; }
        return rightTail ?? leftTail ?? node;
    }
    private static int[] RightChain(BinaryTreeNode<int>? root) { var values = new List<int>(); for (var n = root; n is not null; n = n.Right) { values.Add(n.Value); Assert.Null(n.Left); } return values.ToArray(); }
}
