using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ClosestNodesQueriesInABinarySearchTree;

// LeetCode 2476. Closest Nodes Queries in a Binary Search Tree: for each query,
// report the largest node value <= it and the smallest node value >= it, each
// LeetCodeAnswer.None when no node on that side exists.
//
// Both strategies answer the same floor/ceiling pair per query; they differ in
// whether the BST's ordering is exploited at all. The baseline rescans every node
// for every query and never looks at the ordering; the composed strategy spends
// one in-order walk turning the tree into an ascending buffer and then bisects it.
// O(n * queries) against O(n + queries * log n).
internal static class ClosestNodesQueriesInABinarySearchTreeSolution
{
    // The textbook answer: for each query walk the whole tree, keeping the best
    // value seen on either side of it. Deliberately written without this repo's
    // primitives - a plain recursive walk and no ordering assumption at all - it
    // is the arm the composed strategy below has to justify itself against. Only
    // the tree it is handed is a repo type.
    public static int[][] ClosestNodesByLinearScan(BinaryTreeNode<int>? root, int[] queries)
    {
        var answer = new int[queries.Length][];

        for (var i = 0; i < queries.Length; i++)
        {
            var floor = LeetCodeAnswer.None;
            var ceiling = LeetCodeAnswer.None;
            Scan(root, queries[i], ref floor, ref ceiling);
            answer[i] = [floor, ceiling];
        }

        return answer;
    }

    // This repo's own InOrderTraversal/IInOrderHooks composition - the same one
    // AllElementsInTwoBinarySearchTreesSolution and KthSmallestElementInABSTSolution
    // use - collects every value into an already-ascending DynamicArray<int> once,
    // after which each query is one BinarySearch.LowerBound insertion point: the
    // element at it is the ceiling, the one before it is the floor. That is the
    // same floor/ceiling-from-an-insertion-point move ClosestRoomSolution makes,
    // here reporting both neighbours rather than picking whichever is nearer.
    public static int[][] ClosestNodesByInOrderBinarySearch(BinaryTreeNode<int>? root, int[] queries)
    {
        State.Values.Value = new DynamicArray<int>();
        InOrderTraversal.Walk<int, CollectHooks>(root);
        var values = State.Values.Value;

        var answer = new int[queries.Length][];

        for (var i = 0; i < queries.Length; i++)
        {
            var (floor, ceiling) = FloorAndCeiling(values, queries[i]);
            answer[i] = [floor, ceiling];
        }

        return answer;
    }

    private static (int Floor, int Ceiling) FloorAndCeiling(DynamicArray<int> values, int query)
    {
        var index = BinarySearch.LowerBound(new DynamicArraySequence<int>(values), query);

        // The query is itself a node value: it is its own floor and its own ceiling.
        if (index < values.Count && values.Get(index) == query)
        {
            return (query, query);
        }

        var floor = index > 0 ? values.Get(index - 1) : LeetCodeAnswer.None;
        var ceiling = index < values.Count ? values.Get(index) : LeetCodeAnswer.None;
        return (floor, ceiling);
    }

    private static void Scan(BinaryTreeNode<int>? node, int query, ref int floor, ref int ceiling)
    {
        if (node is null)
        {
            return;
        }

        if (ImprovesFloor(node.Value, query, floor))
        {
            floor = node.Value;
        }

        if (ImprovesCeiling(node.Value, query, ceiling))
        {
            ceiling = node.Value;
        }

        Scan(node.Left, query, ref floor, ref ceiling);
        Scan(node.Right, query, ref floor, ref ceiling);
    }

    // A node improves the floor when it does not overshoot the query and either no
    // floor has been seen yet or this value sits closer to the query from below.
    private static bool ImprovesFloor(int value, int query, int floor) =>
        value <= query && (floor == LeetCodeAnswer.None || value > floor);

    // The mirror of the above on the other side: the best ceiling is the smallest node
    // value still at or above the query.
    private static bool ImprovesCeiling(int value, int query, int ceiling) =>
        value >= query && (ceiling == LeetCodeAnswer.None || value < ceiling);

    // Hooks are static, so the buffer being filled lives in AsyncLocal state
    // alongside the walk - the same arrangement AllElementsInTwoBinarySearchTrees-
    // Solution uses for its per-tree buffer.
    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Values.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Values = new();
    }
}
