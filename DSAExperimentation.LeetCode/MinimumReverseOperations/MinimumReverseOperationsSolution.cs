using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumReverseOperations;

// LeetCode 2612. Minimum Reverse Operations: the single 1 starts at a given position,
// and each operation reverses some window of a fixed size containing it, moving it to
// that window's mirror position. The fewest operations needed to land the 1 on every
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
    // one of the nodeCount candidate destinations is tested individually (O(nodeCount)
    // per pop), which is the cost the primitive-composed arm below has to beat.
    // Deliberately written with BCL Queue/HashSet only: it is what you would write
    // without this repo.
    public static int[] MinOperationsByBruteForceScan(int nodeCount, int startPosition, int[] banned, int windowSize)
    {
        var bannedSet = new HashSet<int>(banned);

        return ScanDistancesFrom(nodeCount, startPosition, windowSize, bannedSet);
    }

    // The scan itself: expand every position the frontier ever holds, testing each of
    // the nodeCount candidate destinations individually - the O(nodeCount) per pop the
    // composed arm below replaces with direct window arithmetic.
    private static int[] ScanDistancesFrom(
        int nodeCount, int start, int windowSize, HashSet<int> bannedSet)
    {
        var distances = DistancesFrom(start, nodeCount);
        var frontier = new Queue<int>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            var from = frontier.Dequeue();

            for (var to = 0; to < nodeCount; to++)
            {
                if (IsVisitedOrBanned(to, distances, bannedSet)
                    || !IsReachableInOneReversal(from, to, nodeCount, windowSize))
                {
                    continue;
                }

                distances[to] = distances[from] + 1;
                frontier.Enqueue(to);
            }
        }

        return distances;
    }

    // Every position still unreached, with `start` already sitting at distance 0.
    private static int[] DistancesFrom(int start, int nodeCount)
    {
        var distances = new int[nodeCount];
        Array.Fill(distances, LeetCodeAnswer.None);
        distances[start] = 0;

        return distances;
    }

    // Whether a candidate destination is already settled or simply off the board's
    // allowed positions - either way there is nothing left to try there.
    private static bool IsVisitedOrBanned(int to, int[] distances, HashSet<int> bannedSet) =>
        distances[to] != LeetCodeAnswer.None || bannedSet.Contains(to);

    // A single reversal takes `from` to `to` exactly when the window that does it,
    // starting at (from + to - windowSize + 1) / 2, is an integer position that both
    // fits inside the array and still covers `from`.
    private static bool IsReachableInOneReversal(int from, int to, int nodeCount, int windowSize)
    {
        var numerator = from + to - windowSize + 1;

        if ((numerator & 1) != 0)
        {
            return false;
        }

        var windowStart = numerator / 2;
        var earliestStart = Math.Max(0, from - windowSize + 1);
        var latestStart = Math.Min(from, nodeCount - windowSize);

        return windowStart >= earliestStart && windowStart <= latestStart;
    }

    // This repo's own Reduce.Graph: ReversalChildren only ever produces the positions
    // actually reachable in one reversal, so each pop does O(windowSize) work instead
    // of the O(nodeCount) scan MinOperationsByBruteForceScan needs.
    public static int[] MinOperationsByReduceGraph(int nodeCount, int startPosition, int[] banned, int windowSize)
        => MinOperationsByReduceGraph(new ReversalBoard(nodeCount, windowSize, new Set<int>(banned)), startPosition);

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
