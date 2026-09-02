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
    public static long[] MinCostsByBruteForceDijkstra(int n, int[] prices, int[][] roads) =>
        MinCostsByBruteForceDijkstra(AppleNetwork.Build(n, prices, roads));

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
        var forward = DijkstraByBruteForce(network, source, useReturnEdges: false);
        var backward = DijkstraByBruteForce(network, source, useReturnEdges: true);
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

    private static long[] DijkstraByBruteForce(AppleNetwork network, int source, bool useReturnEdges)
    {
        var n = network.Nodes.Length;
        var distances = new long[n];
        Array.Fill(distances, long.MaxValue);
        distances[source] = 0L;

        var settled = new bool[n];
        var queue = new PriorityQueue<int, long>();
        queue.Enqueue(source, 0L);

        while (queue.TryDequeue(out var nodeId, out _))
        {
            if (settled[nodeId])
            {
                continue;
            }

            settled[nodeId] = true;
            var node = network.Nodes[nodeId];
            var edges = useReturnEdges ? node.ReturnEdges : node.ForwardEdges;

            foreach (var (weight, target) in edges)
            {
                var candidate = distances[nodeId] + weight;

                if (candidate < distances[target.Id])
                {
                    distances[target.Id] = candidate;
                    queue.Enqueue(target.Id, candidate);
                }
            }
        }

        return distances;
    }

    // This repo's own Dijkstra (Algorithms.ShortestPaths.ShortestPath), run twice
    // per source over AppleNetwork's two IEdgeTopology witnesses in place of the
    // baseline's hand-rolled BCL priority queue - the same swap
    // NetworkRecoveryPathwaysSolution's two arms make around RecoveryNode/
    // RecoveryTopology.
    public static long[] MinCostsByReduceGraph(int n, int[] prices, int[][] roads) =>
        MinCostsByReduceGraph(AppleNetwork.Build(n, prices, roads));

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
}
