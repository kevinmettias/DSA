using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
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
// repo Stack<T> here, PreOrderTour's own stack in the composed arm, never
// recursion - LC's own worst case is a straight-line chain of up to 5e4 nodes,
// deep enough to risk overflowing the call stack) and differ only in how much
// of that walk is shared across queries.
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

    // Composed: DataStructures' PreOrderTour lays the tree out in pre-order, so
    // subtree u is exactly the contiguous run of positions SubtreeOf(u). One pass
    // over the tour then gives every position its node's path XOR sum - a parent
    // always sits before its children, so its sum is already final when they are
    // reached - and a subtree's sums are a contiguous slice of that array. Each
    // distinct node queried gets its distinct/sorted XOR list built at most ONCE
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
        var tour = PreOrderTour.Build(nodes[0], nodes.Length);
        var pathXorAt = PathXorsInTourOrder(tour, par, vals);
        var cache = new Dictionary<int, int[]>();
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var (u, k) = (queries[i][0], queries[i][1]);

            if (!cache.TryGetValue(u, out var sorted))
            {
                var (first, last) = tour.SubtreeOf(u);
                sorted = DistinctSortedXorsInRange(pathXorAt, first, last);
                cache[u] = sorted;
            }

            answers[i] = k <= sorted.Length ? KthSmallestDistinctSum(sorted, k) : LeetCodeAnswer.None;
        }

        return answers;
    }

    // The query's rank is 1-based, so the rank-th smallest distinct sum is the entry
    // at rank - 1 in the ascending list the callers have already established is long
    // enough.
    private static int KthSmallestDistinctSum(IReadOnlyList<int> sortedDistinct, int rank) => sortedDistinct[rank - 1];

    // At each tour position, the path XOR sum of the node there: the root's own value,
    // and below it the parent's sum with the node's value folded in. The root takes
    // position 0, and every other parent sits before its children.
    private static int[] PathXorsInTourOrder(PreOrderTour tour, int[] parent, int[] vals)
    {
        var pathXorAt = new int[tour.NodeCount];
        pathXorAt[0] = vals[tour.NodeAt(0)];

        for (var position = 1; position < tour.NodeCount; position++)
        {
            var node = tour.NodeAt(position);
            var parentPosition = tour.PositionOf(parent[node]);
            pathXorAt[position] = pathXorAt[parentPosition] ^ vals[node];
        }

        return pathXorAt;
    }

    private static int[] DistinctSortedXorsInRange(int[] pathXorAt, int first, int last)
    {
        var seen = new HashMap<int, bool>();

        for (var position = first; position <= last; position++)
        {
            seen.Set(pathXorAt[position], true);
        }

        var values = new List<int>(seen.Keys);
        var array = values.ToArray();

        MergeSort.Sort(array);

        return array;
    }
}
