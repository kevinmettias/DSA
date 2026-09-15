using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;
using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.RootedTreeNode>;
using XorStack = DSAExperimentation.DataStructures.Stack.Stack<(DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.RootedTreeNode Node, int Xor)>;

namespace DSAExperimentation.LeetCode.KthSmallestPathXORSum;

// LeetCode 3590. Kth Smallest Path XOR Sum: par[] describes a tree rooted at
// node 0 (DataStructures' own parent-array encoding), each node carrying a
// value vals[i]. A node's path XOR sum is the XOR of every vals[i] from the
// root down to it, inclusive. For each query [u, k], report the k-th smallest
// DISTINCT path XOR sum among the nodes in u's subtree, or -1 if fewer than k
// distinct sums exist there.
//
// Both strategies walk the same ParentArrayTree.Build tree iteratively (a
// repo Stack<T>, never recursion - LC's own worst case is a straight-line
// chain of up to 5e4 nodes, deep enough to risk overflowing the call stack,
// the same concern ShortestPathInAWeightedTreeSolution's Euler tour was
// written to avoid) and differ only in how much of that walk is shared across
// queries.
internal static class KthSmallestPathXORSumSolution
{
    // Textbook: no memoization at all. Every query redoes the full root-to-
    // everywhere XOR walk from scratch, then walks the queried subtree alone
    // to collect its values - O(n) wasted work per query even when several
    // queries repeat the same node, the arm the cached Euler-tour strategy
    // below has to beat.
    public static int[] KthSmallestXorSumByPerQueryWalk(int[] par, int[] vals, int[][] queries)
    {
        var nodes = ParentArrayTree.Build(par);

        return KthSmallestXorSumByPerQueryWalk(nodes, vals, queries);
    }

    public static int[] KthSmallestXorSumByPerQueryWalk(RootedTreeNode[] nodes, int[] vals, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var pathXor = ComputePathXor(nodes, vals);
            var (u, k) = (queries[i][0], queries[i][1]);

            var values = CollectSubtreeXor(nodes[u], pathXor);
            var distinct = new HashSet<int>(values);
            var sorted = new List<int>(distinct);
            sorted.Sort();

            answers[i] = k <= sorted.Count ? KthSmallestDistinctSum(sorted, k) : LeetCodeAnswer.None;
        }

        return answers;
    }

    private static int[] ComputePathXor(RootedTreeNode[] nodes, int[] vals)
    {
        var pathXor = new int[nodes.Length];
        var stack = new XorStack();
        stack.Push((nodes[0], vals[0]));

        while (stack.TryPop(out var frame))
        {
            pathXor[frame.Node.Id] = frame.Xor;

            foreach (var child in frame.Node.Children)
            {
                stack.Push((child, frame.Xor ^ vals[child.Id]));
            }
        }

        return pathXor;
    }

    private static List<int> CollectSubtreeXor(RootedTreeNode root, int[] pathXor)
    {
        var values = new List<int>();
        var stack = new NodeStack();
        stack.Push(root);

        while (stack.TryPop(out var node))
        {
            values.Add(pathXor[node.Id]);

            foreach (var child in node.Children)
            {
                stack.Push(child);
            }
        }

        return values;
    }

    // Composed: one iterative pre-order walk (repo Stack<T> again) builds every
    // node's path XOR sum AND its Euler-tour [timeIn, timeOut] range in a single
    // pass - subtree u is then exactly the contiguous visitOrder slice
    // [timeIn[u], timeOut[u]], the same "stack-based pre-order never starts a
    // sibling before finishing the current subtree" fact
    // ShortestPathInAWeightedTreeSolution.BuildEulerTour relies on. Each distinct
    // node queried gets its distinct/sorted XOR list built at most ONCE
    // (memoized in a cache keyed by node id) via this repo's own
    // HashMap<TKey, TValue> for dedup and MergeSort over an
    // ArrayIndexedSequence<int> for the sort - repeat queries against the same
    // subtree then cost O(1) instead of a fresh O(subtree size) walk.
    public static int[] KthSmallestXorSumByEulerTourCache(int[] par, int[] vals, int[][] queries)
    {
        var nodes = ParentArrayTree.Build(par);

        return KthSmallestXorSumByEulerTourCache(nodes, par, vals, queries);
    }

    public static int[] KthSmallestXorSumByEulerTourCache(
        RootedTreeNode[] nodes, int[] par, int[] vals, int[][] queries)
    {
        var (pathXor, visitOrder, timeIn, timeOut) = BuildEulerTourWithPathXor(nodes, par, vals);
        var cache = new Dictionary<int, int[]>();
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var (u, k) = (queries[i][0], queries[i][1]);

            if (!cache.TryGetValue(u, out var sorted))
            {
                sorted = DistinctSortedXorsInRange(pathXor, visitOrder, timeIn[u], timeOut[u]);
                cache[u] = sorted;
            }

            answers[i] = k <= sorted.Length ? KthSmallestDistinctSum(sorted, k) : LeetCodeAnswer.None;
        }

        return answers;
    }

    // The query's k is 1-based, so the k-th smallest distinct sum is the entry at
    // k - 1 in the ascending list the callers have already established is long
    // enough.
    private static int KthSmallestDistinctSum(IReadOnlyList<int> sortedDistinct, int k) => sortedDistinct[k - 1];

    private static (int[] PathXor, int[] VisitOrder, int[] TimeIn, int[] TimeOut) BuildEulerTourWithPathXor(
        RootedTreeNode[] nodes, int[] parent, int[] vals)
    {
        var n = nodes.Length;
        var pathXor = new int[n];
        var visitOrder = new int[n];
        var timeIn = new int[n];
        var stack = new XorStack();
        stack.Push((nodes[0], vals[0]));
        var timer = 0;

        while (stack.TryPop(out var frame))
        {
            timer = RecordVisit((pathXor, visitOrder, timeIn), frame, timer);

            foreach (var child in frame.Node.Children)
            {
                stack.Push((child, frame.Xor ^ vals[child.Id]));
            }
        }

        var timeOut = BuildTimeOutRanges(visitOrder, parent, timeIn);

        return (pathXor, visitOrder, timeIn, timeOut);
    }

    // Writes one visit of the pre-order walk into the three parallel tour arrays: the
    // node's entry time, its position in the visit order, and its root-to-node XOR sum.
    // Returns the timer slot the next visit will take.
    private static int RecordVisit(
        (int[] PathXor, int[] VisitOrder, int[] TimeIn) tour, (RootedTreeNode Node, int Xor) frame, int timer)
    {
        tour.TimeIn[frame.Node.Id] = timer;
        tour.VisitOrder[timer] = frame.Node.Id;
        tour.PathXor[frame.Node.Id] = frame.Xor;

        return timer + 1;
    }

    // The [timeIn, timeOut] range each node's subtree occupies: the last of its
    // descendants in the visit order, found by closing the range from its size.
    private static int[] BuildTimeOutRanges(int[] visitOrder, int[] parent, int[] timeIn)
    {
        var size = BuildSubtreeSizes(visitOrder, parent);
        var timeOut = new int[size.Length];

        for (var id = 0; id < timeOut.Length; id++)
        {
            timeOut[id] = timeIn[id] + size[id] - 1;
        }

        return timeOut;
    }

    // How many nodes each node's subtree holds: one reverse sweep of the visit order
    // accumulates every node's count into its parent, since a child is always visited
    // after the node that heads its subtree.
    private static int[] BuildSubtreeSizes(int[] visitOrder, int[] parent)
    {
        var size = new int[visitOrder.Length];
        Array.Fill(size, 1);

        for (var i = size.Length - 1; i >= 1; i--)
        {
            var id = visitOrder[i];
            size[parent[id]] += size[id];
        }

        return size;
    }

    private static int[] DistinctSortedXorsInRange(int[] pathXor, int[] visitOrder, int start, int end)
    {
        var seen = new HashMap<int, bool>();

        for (var t = start; t <= end; t++)
        {
            seen.Set(pathXor[visitOrder[t]], true);
        }

        var values = new List<int>(seen.Keys);
        var array = values.ToArray();

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(array));

        return array;
    }
}
