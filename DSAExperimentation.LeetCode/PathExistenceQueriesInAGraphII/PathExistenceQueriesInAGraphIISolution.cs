using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphII;

// LeetCode 3534. Path Existence Queries in a Graph II: an undirected edge joins i
// and j exactly when |nums[i] - nums[j]| <= maxDiff; answer, per query, the fewest
// edges between two nodes (-1 if none exists).
//
// Sorting nodes by value (SortedByValueGraph) turns "neighbors of a node" into a
// single contiguous range [p, far(p)] in sorted order, where far(p) is the
// rightmost index whose value is still within maxDiff. far() is non-decreasing in
// p (a later position's value is >= an earlier one's, so its own maxDiff window
// reaches at least as far) - which is the one fact that makes both strategies
// work: it means the farthest ANY node in [p, far(p)] can reach in one more hop is
// far(far(p)) itself (attained at the range's right end, since far is
// non-decreasing), so L hops from p reach exactly far^(L)(p), never more. Shortest
// distance from a to b (a <= b in sorted order) is the least L with far^(L)(a) >=
// b.
//
// MinDistancesByRangeBfs computes that L the textbook way - real BFS, expanding
// each dequeued node's whole reachable range on the fly, discovering far() as a
// two-pointer scan. MinDistancesByBinaryLifting instead composes far() once via
// Algorithms.Searching.BinarySearch.UpperBound (nums is already sorted, so "the
// last index within maxDiff" is exactly an upper-bound lookup) and precomputes
// far^(2^k) doubling tables, turning far^(L)(a) >= b into an O(log n)-per-query
// walk instead of an O(n)-per-query BFS.
internal static class PathExistenceQueriesInAGraphIISolution
{
    // Plain BCL Queue/bool[], adjacency discovered by expanding outward from each
    // dequeued node until its value window closes - what you would write without
    // this repo's BinarySearch, no doubling trick.
    public static int[] MinDistancesByRangeBfs(int n, int[] nums, int maxDiff, int[][] queries)
    {
        var graph = SortedByValueGraph.Build(nums);

        return MinDistancesByRangeBfs(graph.SortedValues, graph.PositionOf, maxDiff, queries);
    }

    public static int[] MinDistancesByRangeBfs(int[] sortedValues, int[] positionOf, int maxDiff, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var a = positionOf[queries[q][0]];
            var b = positionOf[queries[q][1]];
            answers[q] = ShortestHopsByBfs(sortedValues, maxDiff, a, b);
        }

        return answers;
    }

    private static int ShortestHopsByBfs(int[] sortedValues, int maxDiff, int start, int target)
    {
        if (start == target)
        {
            return 0;
        }

        var n = sortedValues.Length;
        var visited = new bool[n];
        var queue = new Queue<int>();
        visited[start] = true;
        queue.Enqueue(start);
        var distance = 0;

        while (queue.Count > 0)
        {
            distance++;

            for (var levelSize = queue.Count; levelSize > 0; levelSize--)
            {
                var node = queue.Dequeue();
                var (lo, hi) = ReachableRange(sortedValues, maxDiff, node);

                for (var next = lo; next <= hi; next++)
                {
                    if (next == target)
                    {
                        return distance;
                    }

                    if (!visited[next])
                    {
                        visited[next] = true;
                        queue.Enqueue(next);
                    }
                }
            }
        }

        return -1;
    }

    private static (int Lo, int Hi) ReachableRange(int[] sortedValues, int maxDiff, int node)
    {
        var lo = node;
        var hi = node;

        while (lo - 1 >= 0 && sortedValues[node] - sortedValues[lo - 1] <= maxDiff)
        {
            lo--;
        }

        while (hi + 1 < sortedValues.Length && sortedValues[hi + 1] - sortedValues[node] <= maxDiff)
        {
            hi++;
        }

        return (lo, hi);
    }

    // The composed answer: far() via BinarySearch.UpperBound, then a binary-
    // lifting table over far()'s repeated composition, so each query becomes a
    // doubling walk instead of a fresh graph search.
    public static int[] MinDistancesByBinaryLifting(int n, int[] nums, int maxDiff, int[][] queries)
    {
        var graph = SortedByValueGraph.Build(nums);

        return MinDistancesByBinaryLifting(graph, maxDiff, queries);
    }

    public static int[] MinDistancesByBinaryLifting(SortedByValueGraph graph, int maxDiff, int[][] queries)
    {
        var far = BuildFarPointers(graph.SortedValues, maxDiff);
        var levels = LevelsFor(far.Length);
        var up = BuildDoublingTable(far, levels);

        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var a = graph.PositionOf[queries[q][0]];
            var b = graph.PositionOf[queries[q][1]];

            if (a > b)
            {
                (a, b) = (b, a);
            }

            answers[q] = ShortestHopsByDoubling(up, levels, a, b);
        }

        return answers;
    }

    // far(p) = rightmost sorted index whose value is still within maxDiff of
    // sortedValues[p] - the last index UpperBound places before it overshoots.
    private static int[] BuildFarPointers(int[] sortedValues, int maxDiff)
    {
        var sequence = new ArraySequence<int>(sortedValues);
        var far = new int[sortedValues.Length];

        for (var p = 0; p < sortedValues.Length; p++)
        {
            far[p] = BinarySearch.UpperBound(sequence, sortedValues[p] + maxDiff) - 1;
        }

        return far;
    }

    private static int LevelsFor(int n)
    {
        var levels = 1;

        while (1 << levels < n)
        {
            levels++;
        }

        return levels + 1;
    }

    private static int[][] BuildDoublingTable(int[] far, int levels)
    {
        var up = new int[levels][];
        up[0] = far;

        for (var level = 1; level < levels; level++)
        {
            up[level] = new int[far.Length];

            for (var p = 0; p < far.Length; p++)
            {
                up[level][p] = up[level - 1][up[level - 1][p]];
            }
        }

        return up;
    }

    // Greedily consumes the largest jump that still lands short of target, same
    // shape as a Kth-ancestor binary-lifting walk run in reverse (counting up to a
    // target instead of counting down from a known depth). One hop beyond that
    // point either reaches target or the range has plateaued for good - far() is
    // non-decreasing, so a hop that fails to make progress never will again.
    private static int ShortestHopsByDoubling(int[][] up, int levels, int start, int target)
    {
        if (start == target)
        {
            return 0;
        }

        var cur = start;
        var hops = 0;

        for (var level = levels - 1; level >= 0; level--)
        {
            if (up[level][cur] < target)
            {
                cur = up[level][cur];
                hops += 1 << level;
            }
        }

        hops++;
        cur = up[0][cur];

        return cur >= target ? hops : -1;
    }
}
