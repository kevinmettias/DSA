using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllElementsInTwoBinarySearchTrees;

// LeetCode 1305. All Elements in Two Binary Search Trees: an in-order walk of a BST
// visits values in ascending order, so collecting each tree's in-order sequence
// (this repo's own InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, the same
// composition KthSmallestElementInABSTTests/FindModeInBinarySearchTreeTests use,
// buffered into a DynamicArray<int>) and merging the two already-sorted sequences
// with a plain two-pointer merge is O(n+m) - no full sort needed, unlike the
// textbook "dump everything into one list and sort it" approach.
public sealed partial class AllElementsInTwoBinarySearchTreesTests
{
    [Fact]
    public void GetAllElements_TwoSmallTrees_ReturnsMergedAscendingValues()
    {
        // root1 = [2,1,4], root2 = [1,0,3] -> [0,1,1,2,3,4]
        var root1 = new BinaryTreeNode<int>(2) { Left = new(1), Right = new(4) };
        var root2 = new BinaryTreeNode<int>(1) { Left = new(0), Right = new(3) };

        Assert.Equal([0, 1, 1, 2, 3, 4], GetAllElements(root1, root2));
    }

    [Fact]
    public void GetAllElements_TreesSharingDuplicateValues_KeepsEveryOccurrence()
    {
        // root1 = [1,null,8], root2 = [8,1] -> [1,1,8,8]
        var root1 = new BinaryTreeNode<int>(1) { Right = new(8) };
        var root2 = new BinaryTreeNode<int>(8) { Left = new(1) };

        Assert.Equal([1, 1, 8, 8], GetAllElements(root1, root2));
    }

    [Fact]
    public void GetAllElements_OneTreeEmpty_ReturnsOtherTreesElementsInOrder()
    {
        var root2 = new BinaryTreeNode<int>(5) { Left = new(2), Right = new(9) };

        Assert.Equal([2, 5, 9], GetAllElements(null, root2));
    }

    private static int[] GetAllElements(BinaryTreeNode<int>? root1, BinaryTreeNode<int>? root2)
    {
        var first = CollectInOrder(root1);
        var second = CollectInOrder(root2);
        return MergeSortedLists(first, second);
    }

    private static DynamicArray<int> CollectInOrder(BinaryTreeNode<int>? root)
    {
        State.Values.Value = new DynamicArray<int>();
        InOrderTraversal.Walk<int, CollectHooks>(root);
        return State.Values.Value;
    }

    private static int[] MergeSortedLists(DynamicArray<int> first, DynamicArray<int> second)
    {
        var result = new int[first.Count + second.Count];
        var i = 0;
        var j = 0;
        var k = 0;

        while (i < first.Count && j < second.Count)
        {
            result[k++] = first.Get(i) <= second.Get(j) ? first.Get(i++) : second.Get(j++);
        }

        while (i < first.Count)
        {
            result[k++] = first.Get(i++);
        }

        while (j < second.Count)
        {
            result[k++] = second.Get(j++);
        }

        return result;
    }

    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Values.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Values = new();
    }
}
