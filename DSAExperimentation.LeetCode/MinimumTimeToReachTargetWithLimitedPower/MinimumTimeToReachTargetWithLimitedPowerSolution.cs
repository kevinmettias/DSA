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
        int n, int[][] edges, int power, int[] cost, int source, int target)
    {
        var adjacency = BuildAdjacency(n, edges);
        var distances = new Dictionary<(int Node, int Power), long> { [(source, power)] = 0L };
        var settled = new HashSet<(int Node, int Power)>();
        var frontier = new PriorityQueue<(int Node, int Power), long>();
        frontier.Enqueue((source, power), 0L);

        while (frontier.TryDequeue(out var state, out var time))
        {
            if (!settled.Add(state) || state.Power < cost[state.Node])
            {
                continue;
            }

            var remaining = state.Power - cost[state.Node];

            foreach (var (to, weight) in adjacency[state.Node])
            {
                var nextState = (to, remaining);
                var candidate = time + weight;

                if (!distances.TryGetValue(nextState, out var known) || candidate < known)
                {
                    distances[nextState] = candidate;
                    frontier.Enqueue(nextState, candidate);
                }
            }
        }

        return BestForTarget(distances, target, power);
    }

    private static long[] BestForTarget(Dictionary<(int Node, int Power), long> distances, int target, int power)
    {
        var (bestTime, bestPower) = (long.MaxValue, -1);

        for (var remaining = 0; remaining <= power; remaining++)
        {
            if (!distances.TryGetValue((target, remaining), out var time))
            {
                continue;
            }

            if (time < bestTime || (time == bestTime && remaining > bestPower))
            {
                (bestTime, bestPower) = (time, remaining);
            }
        }

        return bestPower < 0 ? [LeetCodeAnswer.None, LeetCodeAnswer.None] : [bestTime, bestPower];
    }

    private static List<(int To, long Weight)>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<(int To, long Weight)>[n];

        for (var i = 0; i < n; i++)
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
        int n, int[][] edges, int power, int[] cost, int source, int target) =>
        MinTimeMaxPowerByReduceGraph(PowerStateGraph.Build(n, edges, power, cost), source, target);

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

            if (time < bestTime || (time == bestTime && remaining > bestPower))
            {
                (bestTime, bestPower) = (time, remaining);
            }
        }

        return bestPower < 0 ? [LeetCodeAnswer.None, LeetCodeAnswer.None] : [bestTime, bestPower];
    }
}
