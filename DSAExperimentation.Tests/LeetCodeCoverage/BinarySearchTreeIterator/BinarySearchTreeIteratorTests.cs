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
        var iterator = BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(ExampleTree());

        AssertInOrderSequence(iterator, 3, 7, 9, 15, 20);
    }

    [Fact]
    public void CreateByLeftSpineStack_SingleNode_ReturnsThatValueThenExhausts()
    {
        var iterator = BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(new BinaryTreeNode<int>(1));

        AssertInOrderSequence(iterator, 1);
    }

    [Fact]
    public void CreateByLeftSpineStack_EmptyTree_HasNoNext() =>
        Assert.False(BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(null).HasNext());

    // LeetCode's own example tree for LC 173, whose in-order walk is 3, 7, 9, 15, 20.
    private static BinaryTreeNode<int> ExampleTree() =>
        new(7)
        {
            Left = new BinaryTreeNode<int>(3),
            Right = new BinaryTreeNode<int>(15)
            {
                Left = new BinaryTreeNode<int>(9),
                Right = new BinaryTreeNode<int>(20),
            },
        };

    // Drains the iterator, checking that each value arrives in turn and that the
    // spine is spent once the last one has.
    private static void AssertInOrderSequence(
        BinarySearchTreeIteratorSolution.BstIterator iterator, params int[] expected)
    {
        foreach (var value in expected)
        {
            Assert.True(iterator.HasNext());
            Assert.Equal(value, iterator.Next());
        }

        Assert.False(iterator.HasNext());
    }
}
