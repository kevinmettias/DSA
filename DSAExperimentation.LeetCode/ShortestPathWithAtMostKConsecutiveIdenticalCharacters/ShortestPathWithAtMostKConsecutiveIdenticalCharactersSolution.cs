using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// LeetCode 3970. Shortest Path With At Most K Consecutive Identical Characters:
// minimum-weight 0 -> nodeCount - 1 path whose node-label concatenation never
// runs more than maxRunLength identical characters in a row. The run length is
// state, not a fixed property of a node, so the puzzle is really a shortest path
// over the expanded (node, runLength) state graph ConsecutiveRunGraph builds -
// the same "state-expansion + fixed per-edge weight" shape
// RecoveryNode/RecoveryTopology already use for LC 3620, and unlike
// FindMinimumTimeToReachLastRoomI's travel-time puzzles, an edge's weight here
// never depends on how the state was reached, so ShortestPath.Dijkstra's
// fixed-weight IEdgeTopology applies directly - no hand-rolled relax loop needed
// for the composed arm.
internal static class ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution
{
    // Baseline: BCL PriorityQueue + Dictionary, expanding (node, runLength)
    // states on the fly instead of materializing them - "what you'd write
    // without this repo" (ARCHITECTURE.md 17.5).
    public static int MinimumPathWeightByBclPriorityQueue(
        int nodeCount, int[][] edges, string labels, int maxRunLength)
    {
        var adjacency = BuildAdjacency(nodeCount, edges);
        var distances = new Dictionary<(int Node, int Run), long> { [(0, 1)] = 0L };
        var settled = new HashSet<(int Node, int Run)>();
        var frontier = new PriorityQueue<(int Node, int Run), long>();
        frontier.Enqueue((0, 1), 0L);

        SettleAll(adjacency, labels, maxRunLength, (settled, frontier, distances));

        return BestForTarget(distances, nodeCount - 1, maxRunLength);
    }

    // The expanded-state search proper: settle each (node, run) state once, relax every
    // edge out of it, and leave `Distances` holding the best route to each state reached.
    private static void SettleAll(
        List<(int To, long Weight)>[] adjacency, string labels, int maxRunLength,
        (HashSet<(int Node, int Run)> Settled, PriorityQueue<(int Node, int Run), long> Frontier,
            Dictionary<(int Node, int Run), long> Distances) search)
    {
        while (search.Frontier.TryDequeue(out var state, out var distance))
        {
            if (!search.Settled.Add(state))
            {
                continue;
            }

            foreach (var (to, weight) in adjacency[state.Node])
            {
                var nextRun = IsSameLabel(labels[state.Node], labels[to]) ? ContinuedRun(state.Run) : 1;

                if (nextRun > maxRunLength)
                {
                    continue;
                }

                RecordRoute((to, nextRun), distance + weight, search.Distances, search.Frontier);
            }
        }
    }

    // Records a route to `state` once it is strictly shorter than the one already known,
    // and re-queues the state so the frontier can pick it up again.
    private static void RecordRoute(
        (int Node, int Run) state, long candidate,
        Dictionary<(int Node, int Run), long> distances,
        PriorityQueue<(int Node, int Run), long> frontier)
    {
        if (!distances.TryGetValue(state, out var known) || candidate < known)
        {
            distances[state] = candidate;
            frontier.Enqueue(state, candidate);
        }
    }

    // Both endpoints carry the same label, so the identical-character run carries on.
    private static bool IsSameLabel(char from, char to) => from == to;

    // A matching label continues the identical-character run rather than restarting it.
    private static int ContinuedRun(int run) => run + 1;

    private static int BestForTarget(
        Dictionary<(int Node, int Run), long> distances, int target, int maxRunLength)
    {
        var best = long.MaxValue;

        for (var run = 1; run <= maxRunLength; run++)
        {
            if (distances.TryGetValue((target, run), out var distance) && distance < best)
            {
                best = distance;
            }
        }

        return best == long.MaxValue ? LeetCodeAnswer.None : (int)best;
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
    // over ConsecutiveRunGraph's eagerly-wired state nodes, in place of the
    // baseline's hand-rolled BCL priority queue - the same swap
    // NetworkRecoveryPathwaysSolution's two arms make around
    // RecoveryNetwork/RecoveryTopology.
    public static int MinimumPathWeightByReduceGraph(
        int nodeCount, int[][] edges, string labels, int maxRunLength)
    {
        var graph = ConsecutiveRunGraph.Build(nodeCount, edges, labels, maxRunLength);

        return MinimumPathWeightByReduceGraph(graph);
    }

    public static int MinimumPathWeightByReduceGraph(ConsecutiveRunGraph graph)
    {
        var distances = ShortestPath.Dijkstra<
            ConsecutiveRunNode, ConsecutiveRunTopology, ListEdges<ConsecutiveRunNode, long>, long>(graph.Source);

        var best = long.MaxValue;
        var target = graph.NodeCount - 1;

        for (var run = 1; run <= graph.MaxRunLength; run++)
        {
            if (distances.TryGetValue(graph.States[target, run - 1], out var distance) && distance < best)
            {
                best = distance;
            }
        }

        return best == long.MaxValue ? LeetCodeAnswer.None : (int)best;
    }
}
