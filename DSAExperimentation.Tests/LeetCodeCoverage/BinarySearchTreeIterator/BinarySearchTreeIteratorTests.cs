using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinarySearchTreeIterator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinarySearchTreeIterator;

// Harness only: the algorithm lives in BinarySearchTreeIteratorSolution. LeetCode's
// own shape here is a stateful object across a sequence of hasNext()/next() calls,
// so each example is its own [Fact] replaying the LeetCode call script directly -
// BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData<...> member (CS0053), the same reason BalancedBinaryTreeTests uses
// one [Fact] per tree rather than [Theory]. Only one strategy exists (the
// original benchmark's two [Benchmark] arms were both `return 1` placeholders,
// not a second algorithm to reconcile), so there is only one method family.
public sealed class BinarySearchTreeIteratorTests
{
    [Fact]
    public void CreateByLeftSpineStack_LeetCodeExample_ReturnsInOrderSequence()
    {
        var root = new BinaryTreeNode<int>(7)
        {
            Left = new BinaryTreeNode<int>(3),
            Right = new BinaryTreeNode<int>(15)
            {
                Left = new BinaryTreeNode<int>(9),
                Right = new BinaryTreeNode<int>(20),
            },
        };

        var iterator = BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(root);

        Assert.True(iterator.HasNext());
        Assert.Equal(3, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(7, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(9, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(15, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(20, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void CreateByLeftSpineStack_SingleNode_ReturnsThatValueThenExhausts()
    {
        var iterator = BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(new BinaryTreeNode<int>(1));

        Assert.True(iterator.HasNext());
        Assert.Equal(1, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void CreateByLeftSpineStack_EmptyTree_HasNoNext() =>
        Assert.False(BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(null).HasNext());
}
