using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestNodesQueriesInABinarySearchTree;

// LeetCode 2476. Closest Nodes Queries in a Binary Search Tree: an in-order walk
// (this repo's own InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, the
// same composition FindModeInBinarySearchTreeTests/KthSmallestElementInABSTTests
// already use) collects every node value into a sorted DynamicArray<int> once,
// then each query answers with BinarySearch.LowerBound's insertion point - the
// same floor/ceiling-from-an-insertion-point move ClosestRoomTests already proves
// out, here reporting both neighbors instead of picking whichever is nearer, and
// -1 when a neighbor on that side doesn't exist.
public sealed class ClosestNodesQueriesInABinarySearchTreeTests
{
    [Fact]
    public void ClosestNodes_LeetCodeExample1_ReturnsFloorAndCeilingPerQuery()
    {
        // [6,2,13,1,4,9,15,null,null,null,null,null,null,14]
        var root = new BinaryTreeNode<int>(6)
        {
            Left = new(2) { Left = new(1), Right = new(4) },
            Right = new(13) { Left = new(9), Right = new(15) { Left = new(14) } },
        };
        int[] queries = [2, 5, 16];

        var (mins, maxs) = ClosestNodes(root, queries);

        Assert.Equal([2, 4, 15], mins);
        Assert.Equal([2, 6, -1], maxs);
    }

    [Fact]
    public void ClosestNodes_QueriesBeyondBothEnds_ReturnMinusOneForTheMissingNeighbor()
    {
        var root = new BinaryTreeNode<int>(5) { Left = new(3), Right = new(8) };
        int[] queries = [1, 10, 5];

        var (mins, maxs) = ClosestNodes(root, queries);

        Assert.Equal([-1, 8, 5], mins);
        Assert.Equal([3, -1, 5], maxs);
    }

    private static (int[] Mins, int[] Maxs) ClosestNodes(BinaryTreeNode<int> root, int[] queries)
    {
        State.Values.Value = new DynamicArray<int>();
        InOrderTraversal.Walk<int, CollectHooks>(root);
        var values = State.Values.Value;

        var mins = new int[queries.Length];
        var maxs = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var (floor, ceiling) = FloorAndCeiling(values, queries[i]);
            mins[i] = floor;
            maxs[i] = ceiling;
        }

        return (mins, maxs);
    }

    private static (int Floor, int Ceiling) FloorAndCeiling(DynamicArray<int> values, int query)
    {
        var index = BinarySearch.LowerBound(new DynamicArraySequence<int>(values), query);

        if (index < values.Count && values.Get(index) == query)
        {
            return (query, query);
        }

        var floor = index > 0 ? values.Get(index - 1) : -1;
        var ceiling = index < values.Count ? values.Get(index) : -1;
        return (floor, ceiling);
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
