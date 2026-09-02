using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.NetworkRecoveryPathways;

// LeetCode 3620. Network Recovery Pathways: among all 0 -> n-1 paths that stay
// on online intermediate nodes and whose total edge cost is <= k, maximize the
// path's score (its minimum edge cost). Whether a path of score >= T exists is
// monotonic in T - raising the threshold only ever removes edges, so a feasible
// threshold's own total cost can only rise - so the answer reduces to
// binary-searching the largest feasible T, one min-cost-path query per probe
// (MinimumTimeToTransportAllIndividualsSolution's own "graph search wrapped in
// domain-specific driving logic" shape, here with the driving logic being the
// binary search rather than a bitmask frontier).
internal static class NetworkRecoveryPathwaysSolution
{
    // Textbook baseline: BCL Dictionary adjacency rebuilt per probe and BCL's
    // own PriorityQueue for Dijkstra - the arm the repo's own ShortestPath
    // engine below has to justify itself against.
    public static int FindMaxPathScoreByBruteForceDijkstra(int[][] edges, bool[] online, long k) =>
        FindMaxPathScoreByBruteForceDijkstra(RecoveryNetwork.Build(online.Length, edges, online), k);

    public static int FindMaxPathScoreByBruteForceDijkstra(RecoveryNetwork network, long k) =>
        BinarySearchMaxScore(network.MaxCost, threshold => IsFeasibleByBruteForceDijkstra(network, threshold, k));

    private static bool IsFeasibleByBruteForceDijkstra(RecoveryNetwork network, long threshold, long k)
    {
        var n = network.Nodes.Length;
        var adjacency = new Dictionary<int, List<(int To, long Weight)>>();

        foreach (var (from, to, weight) in network.OnlineEdges)
        {
            if (weight < threshold)
            {
                continue;
            }

            if (!adjacency.TryGetValue(from, out var neighbors))
            {
                neighbors = [];
                adjacency[from] = neighbors;
            }

            neighbors.Add((to, weight));
        }

        var distances = new Dictionary<int, long> { [0] = 0L };
        var settled = new HashSet<int>();
        var queue = new PriorityQueue<int, long>();
        queue.Enqueue(0, 0L);

        while (queue.TryDequeue(out var node, out _))
        {
            if (!settled.Add(node))
            {
                continue;
            }

            if (node == n - 1 || !adjacency.TryGetValue(node, out var neighbors))
            {
                continue;
            }

            var distance = distances[node];

            foreach (var (to, weight) in neighbors)
            {
                var candidate = distance + weight;

                if (!distances.TryGetValue(to, out var known) || candidate < known)
                {
                    distances[to] = candidate;
                    queue.Enqueue(to, candidate);
                }
            }
        }

        return distances.TryGetValue(n - 1, out var best) && best <= k;
    }

    // This repo's own Dijkstra (Algorithms.ShortestPaths.ShortestPath) over a
    // RecoveryNetwork rebuilt per probe via RecoveryNode/RecoveryTopology's
    // IEdgeTopology witness, in place of the baseline's hand-rolled BCL
    // priority queue - the same swap
    // MinimumTimeToTransportAllIndividualsSolution's two arms make around
    // TransportGraph/TransportTopology.
    public static int FindMaxPathScoreByReduceGraph(int[][] edges, bool[] online, long k) =>
        FindMaxPathScoreByReduceGraph(RecoveryNetwork.Build(online.Length, edges, online), k);

    public static int FindMaxPathScoreByReduceGraph(RecoveryNetwork network, long k) =>
        BinarySearchMaxScore(network.MaxCost, threshold => IsFeasibleByReduceGraph(network, threshold, k));

    private static bool IsFeasibleByReduceGraph(RecoveryNetwork network, long threshold, long k)
    {
        network.Rebuild(threshold);

        var distances = ShortestPath
            .Dijkstra<RecoveryNode, RecoveryTopology, ListEdges<RecoveryNode, long>, long>(network.Source);

        return distances.TryGetValue(network.Destination, out var best) && best <= k;
    }

    // Shared by both strategies: the largest threshold whose feasibility probe
    // succeeds, or LeetCodeAnswer.None if even threshold 0 (every online edge
    // eligible) is infeasible.
    private static int BinarySearchMaxScore(long maxCost, Func<long, bool> feasible)
    {
        var lo = 0L;
        var hi = maxCost;
        var answer = (long)LeetCodeAnswer.None;

        while (lo <= hi)
        {
            var mid = lo + (hi - lo) / 2;

            if (feasible(mid))
            {
                answer = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }

        return (int)answer;
    }
}
