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
    public static int FindMaxPathScoreByBruteForceDijkstra(int[][] edges, bool[] online, long k)
    {
        var network = RecoveryNetwork.Build(online.Length, edges, online);
        return FindMaxPathScoreByBruteForceDijkstra(network, k);
    }

    public static int FindMaxPathScoreByBruteForceDijkstra(RecoveryNetwork network, long k) =>
        BinarySearchMaxScore(network.MaxCost, new FeasibilityByBruteForceDijkstra(network, k));

    private static bool IsFeasibleByBruteForceDijkstra(RecoveryNetwork network, long threshold, long k)
    {
        var n = network.Nodes.Length;
        var adjacency = BuildEligibleAdjacency(network, threshold);

        var distances = new Dictionary<int, long> { [0] = 0L };
        var settled = new HashSet<int>();
        var queue = new PriorityQueue<int, long>();
        queue.Enqueue(0, 0L);

        while (queue.TryDequeue(out var node, out _))
        {
            if (settled.Add(node) && node != n - 1)
            {
                RelaxNeighbors(adjacency, distances, queue, node);
            }
        }

        return distances.TryGetValue(n - 1, out var best) && best <= k;
    }

    // Only the edges a path of this score is allowed to use, as a plain BCL
    // adjacency list.
    private static Dictionary<int, List<(int To, long Weight)>> BuildEligibleAdjacency(
        RecoveryNetwork network,
        long threshold)
    {
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

        return adjacency;
    }

    // One relaxation step of the Dijkstra above: offer every edge out of `node`
    // to its neighbour when that shortens the best known distance.
    private static void RelaxNeighbors(
        Dictionary<int, List<(int To, long Weight)>> adjacency,
        Dictionary<int, long> distances,
        PriorityQueue<int, long> queue,
        int node)
    {
        if (!adjacency.TryGetValue(node, out var neighbors))
        {
            return;
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

    // This repo's own Dijkstra (Algorithms.ShortestPaths.ShortestPath) over a
    // RecoveryNetwork rebuilt per probe via RecoveryNode/RecoveryTopology's
    // IEdgeTopology witness, in place of the baseline's hand-rolled BCL
    // priority queue - the same swap
    // MinimumTimeToTransportAllIndividualsSolution's two arms make around
    // TransportGraph/TransportTopology.
    public static int FindMaxPathScoreByReduceGraph(int[][] edges, bool[] online, long k)
    {
        var network = RecoveryNetwork.Build(online.Length, edges, online);
        return FindMaxPathScoreByReduceGraph(network, k);
    }

    public static int FindMaxPathScoreByReduceGraph(RecoveryNetwork network, long k) =>
        BinarySearchMaxScore(network.MaxCost, new FeasibilityByReduceGraph(network, k));

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
    private static int BinarySearchMaxScore(long maxCost, IFeasibilityProbe feasible)
    {
        var lo = 0L;
        var hi = maxCost;
        var answer = (long)LeetCodeAnswer.None;

        while (lo <= hi)
        {
            var mid = lo + (hi - lo) / 2;

            if (feasible.IsFeasible(mid))
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

    // The one question both arms answer, each in its own way: is there a source-to-
    // destination path, staying on online intermediate nodes, whose total cost stays
    // within the budget k and whose own score - its cheapest edge - reaches this
    // threshold. The threshold is named here for both arms to share, which a bare
    // callable had nowhere to put.
    private interface IFeasibilityProbe
    {
        bool IsFeasible(long threshold);
    }

    // The baseline probe: rebuild the BCL adjacency for this threshold and run the
    // hand-rolled Dijkstra over it.
    private sealed class FeasibilityByBruteForceDijkstra(RecoveryNetwork network, long k) : IFeasibilityProbe
    {
        public bool IsFeasible(long threshold) => IsFeasibleByBruteForceDijkstra(network, threshold, k);
    }

    // The composed probe: restrict the RecoveryNetwork to this threshold and let this
    // repo's own ShortestPath engine answer it.
    private sealed class FeasibilityByReduceGraph(RecoveryNetwork network, long k) : IFeasibilityProbe
    {
        public bool IsFeasible(long threshold) => IsFeasibleByReduceGraph(network, threshold, k);
    }
}
