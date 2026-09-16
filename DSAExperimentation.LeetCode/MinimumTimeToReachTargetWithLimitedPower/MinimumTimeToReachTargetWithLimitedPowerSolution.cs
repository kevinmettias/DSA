using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumTimeToReachTargetWithLimitedPower;

// LeetCode 3977. Minimum Time to Reach Target With Limited Power: minimum-time
// source -> target path where leaving any node u always spends cost[u] power,
// regardless of which outgoing edge is taken, and among the minimum-time paths
// report the largest power left over. Remaining power is state, not a fixed
// property of a node, so this is a shortest path over the expanded
// (node, remainingPower) state graph PowerStateGraph builds - the same
// state-expansion shape LC 3970's ConsecutiveRunGraph uses, and again an edge's
// weight never depends on how the state was reached (only whether it may be
// taken at all does), so ShortestPath.Dijkstra's fixed-weight IEdgeTopology
// applies directly. The tie-break (maximum remaining power among the paths
// that achieve the minimum time) is answered for free by the state expansion
// itself: it is just "scan every remainingPower level target was settled at,
// keep the one with the smallest time, then the largest power."
internal static class MinimumTimeToReachTargetWithLimitedPowerSolution
{
    // Baseline: BCL PriorityQueue + Dictionary, expanding (node, remainingPower)
    // states on the fly instead of materializing them - "what you'd write
    // without this repo" (ARCHITECTURE.md 17.5).
    public static long[] MinTimeMaxPowerByBclPriorityQueue(
        (int NodeCount, int[][] Edges, int Power, int[] Cost) input, int source, int target)
    {
        var adjacency = BuildAdjacency(input.NodeCount, input.Edges);
        var distances = new Dictionary<(int Node, int Power), long> { [(source, input.Power)] = 0L };
        var settled = new HashSet<(int Node, int Power)>();
        var frontier = new PriorityQueue<(int Node, int Power), long>();
        frontier.Enqueue((source, input.Power), 0L);

        RelaxStates(adjacency, input.Cost, (distances, settled, frontier));

        return BestForTarget(distances, target, input.Power);
    }

    // Drain the state frontier, settling each (node, remainingPower) state once and
    // relaxing the edges out of it - the whole Dijkstra expansion, run in place over
    // the three collections the caller owns.
    private static void RelaxStates(
        List<(int To, long Weight)>[] adjacency,
        int[] cost,
        (Dictionary<(int Node, int Power), long> Times, HashSet<(int Node, int Power)> Settled, PriorityQueue<(int Node, int Power), long> Frontier) search)
    {
        while (search.Frontier.TryDequeue(out var state, out var time))
        {
            if (!search.Settled.Add(state) || state.Power < cost[state.Node])
            {
                continue;
            }

            var remaining = state.Power - cost[state.Node];

            foreach (var (to, weight) in adjacency[state.Node])
            {
                var nextState = (to, remaining);
                var candidate = time + weight;

                if (!search.Times.TryGetValue(nextState, out var known) || candidate < known)
                {
                    search.Times[nextState] = candidate;
                    search.Frontier.Enqueue(nextState, candidate);
                }
            }
        }
    }

    // A strictly faster arrival at the target, or an equally fast one with more
    // power left over - the problem's own tie-break.
    private static bool IsBetterArrival(long time, long bestTime, int remaining, int bestPower)
        => time < bestTime || (time == bestTime && remaining > bestPower);

    // The answer pair LeetCode reads back from this problem: the minimum time,
    // then the largest power left over among the paths that achieve it.
    private static long[] TimeAndPower(long time, int power) => [time, power];

    // No path reaches the target at any remaining-power level, so neither entry
    // of the answer pair carries a time or a power.
    private static long[] NoPath() => [LeetCodeAnswer.None, LeetCodeAnswer.None];

    private static long[] BestForTarget(Dictionary<(int Node, int Power), long> distances, int target, int power)
    {
        var (bestTime, bestPower) = (long.MaxValue, -1);

        for (var remaining = 0; remaining <= power; remaining++)
        {
            if (!distances.TryGetValue((target, remaining), out var time))
            {
                continue;
            }

            if (IsBetterArrival(time, bestTime, remaining, bestPower))
            {
                (bestTime, bestPower) = (time, remaining);
            }
        }

        return bestPower < 0 ? NoPath() : TimeAndPower(bestTime, bestPower);
    }

    private static List<(int To, long Weight)>[] BuildAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<(int To, long Weight)>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add((edge[1], edge[2]));
        }

        return adjacency;
    }

    // Composed: this repo's own Dijkstra (Algorithms.ShortestPaths.ShortestPath)
    // over PowerStateGraph's eagerly-wired state nodes, in place of the
    // baseline's hand-rolled BCL priority queue - the same swap
    // NetworkRecoveryPathwaysSolution's two arms make around
    // RecoveryNetwork/RecoveryTopology.
    public static long[] MinTimeMaxPowerByReduceGraph(
        (int NodeCount, int[][] Edges, int Power, int[] Cost) input, int source, int target)
    {
        var graph = PowerStateGraph.Build(input.NodeCount, input.Edges, input.Power, input.Cost);

        return MinTimeMaxPowerByReduceGraph(graph, source, target);
    }

    public static long[] MinTimeMaxPowerByReduceGraph(PowerStateGraph graph, int source, int target)
    {
        var distances = ShortestPath.Dijkstra<
            PowerStateNode, PowerStateTopology, ListEdges<PowerStateNode, long>, long>(graph.SourceState(source));

        var (bestTime, bestPower) = (long.MaxValue, -1);

        for (var remaining = 0; remaining <= graph.Power; remaining++)
        {
            if (!distances.TryGetValue(graph.States[target, remaining], out var time))
            {
                continue;
            }

            if (IsBetterArrival(time, bestTime, remaining, bestPower))
            {
                (bestTime, bestPower) = (time, remaining);
            }
        }

        return bestPower < 0 ? NoPath() : TimeAndPower(bestTime, bestPower);
    }
}
