using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumReverseOperations;

// LeetCode 2612. Minimum Reverse Operations: the single 1 starts at position p, and
// each operation reverses some size-K window containing it, moving it to that
// window's mirror position. The fewest operations needed to land the 1 on every
// other position is exactly a shortest-path query over the implicit graph where
// positions are nodes and one reversal is one edge - Reduce.Graph's own
// BreadthFirstReduceOrder + DistanceMapReduceAlgebra already answers "distance from
// a root to every node" (the same composition OpenTheLockSolution uses), so this
// problem reduces to supplying the right IGraphTopology: ReversalTopology, whose
// ReversalChildren computes each position's reachable mirrors directly from the
// window arithmetic instead of scanning every candidate. Banned positions are simply
// excluded as children, the same way Domain.Locks.LockGraph excludes deadends.
internal static class MinimumReverseOperationsSolution
{
    // Baseline: no on-demand window arithmetic - for every dequeued position, every
    // one of the n candidate destinations is tested individually (O(n) per pop),
    // which is the cost the primitive-composed arm below has to beat. Deliberately
    // written with BCL Queue/HashSet only: it is what you would write without this
    // repo.
    public static int[] MinOperationsByBruteForceScan(int n, int p, int[] banned, int k)
    {
        var bannedSet = new HashSet<int>(banned);
        var distances = new int[n];
        Array.Fill(distances, LeetCodeAnswer.None);
        distances[p] = 0;

        var queue = new Queue<int>();
        queue.Enqueue(p);

        while (queue.Count > 0)
        {
            var from = queue.Dequeue();

            for (var to = 0; to < n; to++)
            {
                if (distances[to] != LeetCodeAnswer.None || bannedSet.Contains(to))
                {
                    continue;
                }

                if (!IsReachableInOneReversal(from, to, n, k))
                {
                    continue;
                }

                distances[to] = distances[from] + 1;
                queue.Enqueue(to);
            }
        }

        return distances;
    }

    // A single reversal takes `from` to `to` exactly when the window that does it,
    // starting at L = (from + to - K + 1) / 2, is an integer position that both fits
    // inside the array and still covers `from`.
    private static bool IsReachableInOneReversal(int from, int to, int n, int k)
    {
        var numerator = from + to - k + 1;

        if ((numerator & 1) != 0)
        {
            return false;
        }

        var windowStart = numerator / 2;
        var earliestStart = Math.Max(0, from - k + 1);
        var latestStart = Math.Min(from, n - k);

        return windowStart >= earliestStart && windowStart <= latestStart;
    }

    // This repo's own Reduce.Graph: ReversalChildren only ever produces the positions
    // actually reachable in one reversal, so each pop does O(K) work instead of the
    // O(n) scan MinOperationsByBruteForceScan needs.
    public static int[] MinOperationsByReduceGraph(int n, int p, int[] banned, int k)
        => MinOperationsByReduceGraph(new ReversalBoard(n, k, new Set<int>(banned)), p);

    // #17.4's hoisted overload: ReversalBoard is not IEnumerable, so this can never
    // be confused with the LeetCode-shaped overload above, and a benchmark can charge
    // board construction to [GlobalSetup].
    public static int[] MinOperationsByReduceGraph(ReversalBoard board, int start)
    {
        var source = new PositionNode(start, board);

        var distanceByNode = Reduce.Graph<
            PositionNode, ReversalTopology, ReversalChildren,
            NaturalChildOrder<PositionNode, ReversalChildren>, ReversalChildren,
            BreadthFirstReduceOrder<PositionNode>,
            DistanceMapReduceAlgebra<PositionNode>, Dictionary<PositionNode, int>>(source);

        var answer = new int[board.Length];

        for (var position = 0; position < board.Length; position++)
        {
            answer[position] = distanceByNode.TryGetValue(new PositionNode(position, board), out var distance)
                ? distance
                : LeetCodeAnswer.None;
        }

        return answer;
    }
}
