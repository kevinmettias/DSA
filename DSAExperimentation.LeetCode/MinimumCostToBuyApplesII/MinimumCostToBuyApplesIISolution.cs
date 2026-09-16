using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumCostToBuyApplesII;

// LeetCode 3928. Minimum Cost to Buy Apples II: from every shop i, buy locally
// for prices[i] or travel empty to some shop j, buy for prices[j], and travel
// back carrying apples (paying cost * tax per road on the return leg only).
// Forward and return legs may use different paths, so the answer per source is
// min over j of (cheapest empty trip i -> j) + prices[j] + (cheapest laden trip
// j -> i) - two independent shortest-path trees per source, one over plain-cost
// edges and one over cost*tax edges (LC 2473's own single-multiplier trick does
// not apply here, since tax varies per road rather than being one global k).
internal static class MinimumCostToBuyApplesIISolution
{
    // Textbook: BCL Dictionary adjacency plus BCL's own PriorityQueue for
    // Dijkstra, run twice per source - the arm the repo's own ShortestPath engine
    // below has to justify itself against.
    public static long[] MinCostsByBruteForceDijkstra(int shopCount, int[] prices, int[][] roads)
    {
        var network = AppleNetwork.Build(shopCount, prices, roads);
        return MinCostsByBruteForceDijkstra(network);
    }

    public static long[] MinCostsByBruteForceDijkstra(AppleNetwork network)
    {
        var n = network.Nodes.Length;
        var answer = new long[n];

        for (var source = 0; source < n; source++)
        {
            answer[source] = MinCostFromByBruteForceDijkstra(network, source);
        }

        return answer;
    }

    private static long MinCostFromByBruteForceDijkstra(AppleNetwork network, int source)
    {
        var forward = DijkstraByBruteForce(network, source, EdgeSet.Forward);
        var backward = DijkstraByBruteForce(network, source, EdgeSet.Return);
        var best = long.MaxValue;

        for (var j = 0; j < network.Nodes.Length; j++)
        {
            if (forward[j] == long.MaxValue || backward[j] == long.MaxValue)
            {
                continue;
            }

            best = Math.Min(best, forward[j] + network.Prices[j] + backward[j]);
        }

        return best;
    }

    private static long[] DijkstraByBruteForce(AppleNetwork network, int source, EdgeSet edgeSet)
    {
        var n = network.Nodes.Length;
        var distances = new long[n];
        Array.Fill(distances, long.MaxValue);
        distances[source] = 0L;

        var frontier = new DijkstraFrontier(edgeSet, distances, new bool[n], new PriorityQueue<int, long>());
        frontier.Queue.Enqueue(source, 0L);

        while (frontier.Queue.TryDequeue(out var nodeId, out _))
        {
            SettleAndRelax(network, nodeId, frontier);
        }

        return distances;
    }

    // Settles one dequeued node and relaxes every edge out of it, queueing each
    // neighbour the relaxation improved. A node that is already settled is a stale
    // queue entry left behind by an earlier improvement, so it is skipped.
    private static void SettleAndRelax(AppleNetwork network, int nodeId, DijkstraFrontier frontier)
    {
        if (frontier.Settled[nodeId])
        {
            return;
        }

        frontier.Settled[nodeId] = true;
        var node = network.Nodes[nodeId];
        var edges = frontier.Edges == EdgeSet.Return ? node.ReturnEdges : node.ForwardEdges;

        foreach (var (weight, target) in edges)
        {
            var candidate = frontier.Distances[nodeId] + weight;

            if (candidate < frontier.Distances[target.Id])
            {
                frontier.Distances[target.Id] = candidate;
                frontier.Queue.Enqueue(target.Id, candidate);
            }
        }
    }

    // This repo's own Dijkstra (Algorithms.ShortestPaths.ShortestPath), run twice
    // per source over AppleNetwork's two IEdgeTopology witnesses in place of the
    // baseline's hand-rolled BCL priority queue - the same swap
    // NetworkRecoveryPathwaysSolution's two arms make around RecoveryNode/
    // RecoveryTopology.
    public static long[] MinCostsByReduceGraph(int shopCount, int[] prices, int[][] roads)
    {
        var network = AppleNetwork.Build(shopCount, prices, roads);
        return MinCostsByReduceGraph(network);
    }

    public static long[] MinCostsByReduceGraph(AppleNetwork network)
    {
        var n = network.Nodes.Length;
        var answer = new long[n];

        for (var source = 0; source < n; source++)
        {
            var sourceNode = network.Nodes[source];
            var forward = ShortestPath
                .Dijkstra<AppleNode, AppleForwardTopology, ListEdges<AppleNode, long>, long>(sourceNode);
            var backward = ShortestPath
                .Dijkstra<AppleNode, AppleReturnTopology, ListEdges<AppleNode, long>, long>(sourceNode);

            answer[source] = BestCost(network, forward, backward);
        }

        return answer;
    }

    private static long BestCost(
        AppleNetwork network, Dictionary<AppleNode, long> forward, Dictionary<AppleNode, long> backward)
    {
        var best = long.MaxValue;

        for (var j = 0; j < network.Nodes.Length; j++)
        {
            var node = network.Nodes[j];

            if (!forward.TryGetValue(node, out var forwardCost) || !backward.TryGetValue(node, out var backwardCost))
            {
                continue;
            }

            best = Math.Min(best, forwardCost + network.Prices[j] + backwardCost);
        }

        return best;
    }

    // The baseline Dijkstra's own working state: which edge set the run relaxes (the
    // plain-cost forward edges or the cost*tax return edges), the best distance known
    // to each node so far, whether it has been settled, and the frontier of candidates.
    private readonly record struct DijkstraFrontier(
        EdgeSet Edges, long[] Distances, bool[] Settled, PriorityQueue<int, long> Queue);

    // Which of AppleNetwork's two edge families a run relaxes: the plain-cost edges
    // travelled empty to a shop, or the cost*tax edges travelled back carrying
    // apples. Named where a rewritten `true`/`false` at the call site said it only
    // by position.
    private enum EdgeSet
    {
        Forward,
        Return,
    }
}
