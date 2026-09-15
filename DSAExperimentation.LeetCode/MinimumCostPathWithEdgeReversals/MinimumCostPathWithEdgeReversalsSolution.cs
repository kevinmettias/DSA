using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumCostPathWithEdgeReversals;

// LeetCode 3650. Minimum Cost Path with Edge Reversals: a directed weighted graph of
// n nodes. Each node's switch lets you reverse one of its incoming edges (cost 2w)
// the instant you arrive, and immediately cross it back the other way. Find the
// cheapest node 0 -> node n-1 path, or -1 if none exists.
//
// A shortest *simple* path visits every node at most once, so it can only ever use
// one node's switch on one of that node's incoming edges - exactly the capacity the
// puzzle grants. That means the per-node "used at most once" bookkeeping never has to
// enter the search state at all: building a graph where every input edge (u, v, w)
// contributes both u -> v at cost w and v -> u at cost 2w turns the puzzle into one
// plain shortest-path query on the augmented graph.
internal static class MinimumCostPathWithEdgeReversalsSolution
{
    // Textbook baseline: BCL adjacency lists plus PriorityQueue<int,long> Dijkstra
    // over the same forward/reversed edge set, built inline - deliberately without
    // this repo's graph engine, the arm the composed strategy below has to justify
    // itself against.
    public static int MinCostByBruteForceDijkstra(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        var dist = SeedDistances(n);

        RunDijkstra(adjacency, dist);

        return DestinationDistance(dist, n);
    }

    // The distance array every arm starts from: node 0 at distance 0, every other node
    // still unreached.
    private static long[] SeedDistances(int n)
    {
        var dist = new long[n];
        Array.Fill(dist, long.MaxValue);
        dist[0] = 0;

        return dist;
    }

    // PriorityQueue<int, long> Dijkstra from node 0: settle each node once, then offer
    // every one of its outgoing edges' candidate distances to the queue.
    private static void RunDijkstra(List<(int Neighbor, long Weight)>[] adjacency, long[] dist)
    {
        var settled = new bool[dist.Length];
        var queue = new PriorityQueue<int, long>();
        queue.Enqueue(0, 0);

        while (queue.TryDequeue(out var node, out _))
        {
            if (settled[node])
            {
                continue;
            }

            settled[node] = true;
            RelaxOutgoing(adjacency[node], dist, queue, node);
        }
    }

    // One settled node's edge scan: a shorter route through it replaces the neighbor's
    // distance and re-offers that neighbor to the queue.
    private static void RelaxOutgoing(
        List<(int Neighbor, long Weight)> outgoing, long[] dist, PriorityQueue<int, long> queue, int node)
    {
        foreach (var (neighbor, weight) in outgoing)
        {
            var candidate = dist[node] + weight;

            if (candidate < dist[neighbor])
            {
                dist[neighbor] = candidate;
                queue.Enqueue(neighbor, candidate);
            }
        }
    }

    // The settled distance to the destination, narrowed to the problem's int answer, or
    // the problem's "no path" sentinel when node n - 1 was never reached.
    private static int DestinationDistance(long[] dist, int n)
    {
        var destinationDistance = dist[n - 1];

        return destinationDistance == long.MaxValue ? LeetCodeAnswer.None : (int)destinationDistance;
    }

    private static List<(int Neighbor, long Weight)>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<(int Neighbor, long Weight)>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            var (u, v, w) = (edge[0], edge[1], (long)edge[2]);
            adjacency[u].Add((v, w));
            adjacency[v].Add((u, 2 * w));
        }

        return adjacency;
    }

    // This repo's own Dijkstra: ShortestPath.Dijkstra over ReversalGraphNode via
    // ReversalGraphTopology gives every reachable node's distance from node 0 in one
    // call, so the puzzle reduces to building the augmented graph once and reading
    // off node n-1's distance - the same composition FindEdgesInShortestPathsSolution
    // uses for LC 3123's own Dijkstra arm.
    public static int MinCostByShortestPathDijkstra(int n, int[][] edges)
    {
        var graph = ReversalGraph.Build(n, edges);
        return MinCostByShortestPathDijkstra(graph);
    }

    public static int MinCostByShortestPathDijkstra(ReversalGraph graph)
    {
        var nodes = graph.Nodes;
        var distances = ShortestPath
            .Dijkstra<ReversalGraphNode, ReversalGraphTopology, ListEdges<ReversalGraphNode, long>, long>(nodes[0]);

        return distances.TryGetValue(nodes[^1], out var distance) ? (int)distance : LeetCodeAnswer.None;
    }
}
