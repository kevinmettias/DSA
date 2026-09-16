using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FlattenBinaryTreeToLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenBinaryTreeToLinkedList;

// Harness only. Both strategies are FlattenBinaryTreeToLinkedListSolution's; this
// file pins them to LeetCode's published examples. BinaryTreeNode<int> is internal,
// so - as in ValidateBinarySearchTreeTests - it stays out of a public TheoryData/
// [Theory] signature, and since flattening mutates its tree in place, each example
// is a private factory rebuilt fresh per [Fact].
public sealed partial class FlattenBinaryTreeToLinkedListTests
{
    [Fact]
    public void FlattenByRecursiveSplice_ClassicExample_RewritesToPreorderRightChain()
    {
        var root = ClassicTree();

        FlattenBinaryTreeToLinkedListSolution.FlattenByRecursiveSplice(root);

        Assert.Equal([1, 2, 3, 4, 5, 6], RightChain(root));
    }

    [Fact]
    public void FlattenByTopDownPreorderRelink_ClassicExample_RewritesToPreorderRightChain()
    {
        var root = ClassicTree();

        FlattenBinaryTreeToLinkedListSolution.FlattenByTopDownPreorderRelink(root);

        Assert.Equal([1, 2, 3, 4, 5, 6], RightChain(root));
    }

    [Fact]
    public void FlattenByRecursiveSplice_EmptyTree_DoesNotThrow() =>
        Assert.Null(Record.Exception(() => FlattenBinaryTreeToLinkedListSolution.FlattenByRecursiveSplice(null)));

    [Fact]
    public void FlattenByTopDownPreorderRelink_EmptyTree_DoesNotThrow() =>
        Assert.Null(Record.Exception(() => FlattenBinaryTreeToLinkedListSolution.FlattenByTopDownPreorderRelink(null)));

    [Fact]
    public void FlattenByRecursiveSplice_SingleNode_LeavesItAsTheWholeChain()
    {
        var root = new BinaryTreeNode<int>(0);

        FlattenBinaryTreeToLinkedListSolution.FlattenByRecursiveSplice(root);

        Assert.Equal([0], RightChain(root));
    }

    [Fact]
    public void FlattenByTopDownPreorderRelink_SingleNode_LeavesItAsTheWholeChain()
    {
        var root = new BinaryTreeNode<int>(0);

        FlattenBinaryTreeToLinkedListSolution.FlattenByTopDownPreorderRelink(root);

        Assert.Equal([0], RightChain(root));
    }

    [Fact]
    public void FlattenByRecursiveSplice_LeftOnlyChain_RewritesToRightOnlyChain()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(2) { Left = new(3) } };

        FlattenBinaryTreeToLinkedListSolution.FlattenByRecursiveSplice(root);

        Assert.Equal([1, 2, 3], RightChain(root));
    }

    [Fact]
    public void FlattenByTopDownPreorderRelink_LeftOnlyChain_RewritesToRightOnlyChain()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(2) { Left = new(3) } };

        FlattenBinaryTreeToLinkedListSolution.FlattenByTopDownPreorderRelink(root);

        Assert.Equal([1, 2, 3], RightChain(root));
    }

    // [1,2,5,3,4,null,6] - LeetCode's own example 1.
    private static BinaryTreeNode<int> ClassicTree() =>
        new(1) { Left = new(2) { Left = new(3), Right = new(4) }, Right = new(5) { Right = new(6) } };

    private static int[] RightChain(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();

        for (var node = root; node is not null; node = node.Right)
        {
            values.Add(node.Value);
            Assert.Null(node.Left);
        }

        return [.. values];
    }
}
