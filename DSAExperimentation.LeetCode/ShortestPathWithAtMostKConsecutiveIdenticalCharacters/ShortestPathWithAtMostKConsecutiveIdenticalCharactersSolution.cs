using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// LeetCode 3970. Shortest Path With At Most K Consecutive Identical Characters:
// minimum-weight 0 -> n-1 path whose node-label concatenation never runs more
// than k identical characters in a row. The run length is state, not a fixed
// property of a node, so the puzzle is really a shortest path over the expanded
// (node, runLength) state graph ConsecutiveRunGraph builds - the same
// "state-expansion + fixed per-edge weight" shape RecoveryNode/RecoveryTopology
// already use for LC 3620, and unlike FindMinimumTimeToReachLastRoomI's
// travel-time puzzles, an edge's weight here never depends on how the state was
// reached, so ShortestPath.Dijkstra's fixed-weight IEdgeTopology applies
// directly - no hand-rolled relax loop needed for the composed arm.
internal static class ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution
{
    // Baseline: BCL PriorityQueue + Dictionary, expanding (node, runLength)
    // states on the fly instead of materializing them - "what you'd write
    // without this repo" (ARCHITECTURE.md 17.5).
    public static int MinimumPathWeightByBclPriorityQueue(int n, int[][] edges, string labels, int k)
    {
        var adjacency = BuildAdjacency(n, edges);
        var distances = new Dictionary<(int Node, int Run), long> { [(0, 1)] = 0L };
        var settled = new HashSet<(int Node, int Run)>();
        var frontier = new PriorityQueue<(int Node, int Run), long>();
        frontier.Enqueue((0, 1), 0L);

        while (frontier.TryDequeue(out var state, out var distance))
        {
            if (!settled.Add(state))
            {
                continue;
            }

            foreach (var (to, weight) in adjacency[state.Node])
            {
                var nextRun = labels[state.Node] == labels[to] ? state.Run + 1 : 1;

                if (nextRun > k)
                {
                    continue;
                }

                var nextState = (to, nextRun);
                var candidate = distance + weight;

                if (!distances.TryGetValue(nextState, out var known) || candidate < known)
                {
                    distances[nextState] = candidate;
                    frontier.Enqueue(nextState, candidate);
                }
            }
        }

        return BestForTarget(distances, n - 1, k);
    }

    private static int BestForTarget(Dictionary<(int Node, int Run), long> distances, int target, int k)
    {
        var best = long.MaxValue;

        for (var run = 1; run <= k; run++)
        {
            if (distances.TryGetValue((target, run), out var distance) && distance < best)
            {
                best = distance;
            }
        }

        return best == long.MaxValue ? LeetCodeAnswer.None : (int)best;
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
    // over ConsecutiveRunGraph's eagerly-wired state nodes, in place of the
    // baseline's hand-rolled BCL priority queue - the same swap
    // NetworkRecoveryPathwaysSolution's two arms make around
    // RecoveryNetwork/RecoveryTopology.
    public static int MinimumPathWeightByReduceGraph(int n, int[][] edges, string labels, int k) =>
        MinimumPathWeightByReduceGraph(ConsecutiveRunGraph.Build(n, edges, labels, k));

    public static int MinimumPathWeightByReduceGraph(ConsecutiveRunGraph graph)
    {
        var distances = ShortestPath.Dijkstra<
            ConsecutiveRunNode, ConsecutiveRunTopology, ListEdges<ConsecutiveRunNode, long>, long>(graph.Source);

        var best = long.MaxValue;
        var target = graph.NodeCount - 1;

        for (var run = 1; run <= graph.K; run++)
        {
            if (distances.TryGetValue(graph.States[target, run - 1], out var distance) && distance < best)
            {
                best = distance;
            }
        }

        return best == long.MaxValue ? LeetCodeAnswer.None : (int)best;
    }
}
