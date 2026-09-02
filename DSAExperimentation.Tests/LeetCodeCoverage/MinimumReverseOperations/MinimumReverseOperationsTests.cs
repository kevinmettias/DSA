using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode;
using DSAExperimentation.Tests.LeetCodeCoverage.MinimumReverseOperations.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumReverseOperations;

// LeetCode 2612. Minimum Reverse Operations: the single 1 starts at position p, and
// each operation reverses some size-K window containing it, moving it to that
// window's mirror position. The distance from p to every other position is exactly
// a shortest-path query over the implicit graph where positions are nodes and one
// reversal is one edge - Reduce.Graph's own BreadthFirstReduceOrder +
// DistanceMapReduceAlgebra already answers "distance from a root to every node" (the
// same composition WordLadderSolution/OpenTheLockSolution use), so this problem
// reduces to supplying the right IGraphTopology: ReversalTopology, whose
// ReversalChildren computes each position's reachable mirrors directly from the
// window arithmetic instead of scanning every candidate. Banned positions are simply
// excluded as children, the same way Domain.Locks.LockGraph excludes deadends.
public sealed partial class MinimumReverseOperationsTests
{
    public static TheoryData<int, int, int[], int, int[]> Examples =>
        new()
        {
            { 4, 0, [1, 2], 4, [0, -1, -1, 1] },
            { 5, 0, [2, 4], 3, [0, -1, -1, -1, -1] },
            { 4, 2, [0, 1, 3], 1, [-1, -1, 0, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForceScan_LeetCodeExamples_ReturnsShortestOperationCounts(
        int n, int p, int[] banned, int k, int[] expected) =>
        Assert.Equal(expected, MinOperationsByBruteForceScan(n, p, banned, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByReduceGraph_LeetCodeExamples_ReturnsShortestOperationCounts(
        int n, int p, int[] banned, int k, int[] expected) =>
        Assert.Equal(expected, MinOperationsByReduceGraph(n, p, banned, k));

    // Baseline: no on-demand window arithmetic - for every dequeued position, every
    // one of the n candidate destinations is tested individually (O(n) per pop), the
    // O(K)-per-pop primitive-composed arm below has to beat.
    private static int[] MinOperationsByBruteForceScan(int n, int p, int[] banned, int k)
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

    // This repo's own Reduce.Graph: ReversalChildren only ever produces the
    // positions actually reachable in one reversal, so each pop does O(K) work
    // instead of the O(n) scan MinOperationsByBruteForceScan needs.
    private static int[] MinOperationsByReduceGraph(int n, int p, int[] banned, int k)
    {
        var board = new ReversalBoard(n, k, new Set<int>(banned));
        var source = new PositionNode(p, board);

        var distanceByNode = Reduce.Graph<
            PositionNode, ReversalTopology, ReversalChildren,
            NaturalChildOrder<PositionNode, ReversalChildren>, ReversalChildren,
            BreadthFirstReduceOrder<PositionNode>,
            DistanceMapReduceAlgebra<PositionNode>, Dictionary<PositionNode, int>>(source);

        var answer = new int[n];

        for (var position = 0; position < n; position++)
        {
            answer[position] = distanceByNode.TryGetValue(new PositionNode(position, board), out var distance)
                ? distance
                : LeetCodeAnswer.None;
        }

        return answer;
    }
}
