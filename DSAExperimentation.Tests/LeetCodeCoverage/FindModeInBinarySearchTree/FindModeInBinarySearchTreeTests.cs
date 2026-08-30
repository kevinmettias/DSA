using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindModeInBinarySearchTree;

// LeetCode 501. Find Mode in Binary Search Tree: an in-order walk of a BST visits
// equal values consecutively, so tracking a running streak length while walking
// finds every most-frequent value in one O(n) pass with no extra map - this
// repo's own InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, the same
// composition KthSmallestElementInABSTTests uses, collecting modes into a
// DynamicArray<int> instead of a BCL List<int>.
public sealed partial class FindModeInBinarySearchTreeTests
{
    [Fact]
    public void FindMode_SingleModeFromADuplicateInARightSubtree_ReturnsThatValue()
    {
        // [1,null,2,2] -> [2]
        var root = new BinaryTreeNode<int>(1) { Right = new(2) { Left = new(2) } };

        Assert.Equal([2], FindMode(root));
    }

    [Fact]
    public void FindMode_MultipleValuesTiedForMostFrequent_ReturnsAllOfThem()
    {
        var root = new BinaryTreeNode<int>(2)
        {
            Left = new(1) { Left = new(1) },
            Right = new(3) { Right = new(3) },
        };

        Assert.Equal([1, 3], FindMode(root));
    }

    [Fact]
    public void FindMode_SingleNode_ReturnsItsValue()
    {
        var root = new BinaryTreeNode<int>(7);

        Assert.Equal([7], FindMode(root));
    }

    private static int[] FindMode(BinaryTreeNode<int> root)
    {
        State.Modes.Value = new DynamicArray<int>();
        State.CurrentValue.Value = 0;
        State.CurrentCount.Value = 0;
        State.MaxCount.Value = 0;

        InOrderTraversal.Walk<int, ModeHooks>(root);

        var modes = State.Modes.Value;
        var result = new int[modes.Count];

        for (var i = 0; i < modes.Count; i++)
        {
            result[i] = modes.Get(i);
        }

        return result;
    }

    private readonly struct ModeHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.CurrentCount.Value == 0 || node.Value != State.CurrentValue.Value)
            {
                State.CurrentValue.Value = node.Value;
                State.CurrentCount.Value = 0;
            }

            State.CurrentCount.Value++;

            if (State.CurrentCount.Value > State.MaxCount.Value)
            {
                State.MaxCount.Value = State.CurrentCount.Value;
                State.Modes.Value = new DynamicArray<int>();
                State.Modes.Value.Add(node.Value);
            }
            else if (State.CurrentCount.Value == State.MaxCount.Value)
            {
                State.Modes.Value!.Add(node.Value);
            }
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Modes = new();
        public static readonly AsyncLocal<int> CurrentValue = new();
        public static readonly AsyncLocal<int> CurrentCount = new();
        public static readonly AsyncLocal<int> MaxCount = new();
    }
}
