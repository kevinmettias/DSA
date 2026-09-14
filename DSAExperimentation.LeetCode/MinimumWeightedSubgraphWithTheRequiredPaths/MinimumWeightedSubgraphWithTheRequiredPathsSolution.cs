using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumWeightedSubgraphWithTheRequiredPaths;

// LeetCode 2203. Minimum Weighted Subgraph With the Required Paths: pick the
// cheapest set of edges of a directed weighted graph that still contains a path
// src1 -> dest and a path src2 -> dest, or report -1 when no such set exists.
//
// The optimal subgraph is always two shortest paths (src1 -> meet, src2 -> meet)
// joined to one shared shortest suffix (meet -> dest): any edge counted twice
// would be paid for twice, so the two routes may as well merge as early as they
// can and share everything after. So the answer is
// min over every candidate meeting vertex v of
// dist(src1, v) + dist(src2, v) + dist(v, dest).
//
// The two strategies differ only in how those distances are obtained: one
// point-to-point search per candidate vertex, or three whole-graph searches whose
// results every candidate then reads off - the third run on the edge-reversed
// graph, since "distance to dest" is exactly "distance from dest on the reverse
// graph".
internal static class MinimumWeightedSubgraphWithTheRequiredPathsSolution
{
    // Textbook baseline: BCL adjacency lists plus a PriorityQueue<int, long>
    // Dijkstra that stops as soon as its target settles, run three times for every
    // candidate meeting vertex - deliberately without this repo's graph engine,
    // the arm the composed strategy below has to justify itself against.
    public static long MinimumWeightByPerNodeSearch(int n, int[][] edges, int src1, int src2, int dest)
    {
        var adjacency = BuildAdjacency(n, edges);
        var endpoints = new PathEndpoints(src1, src2, dest);
        var best = long.MaxValue;

        for (var node = 0; node < n; node++)
        {
            var candidate = MeetingCostAt(adjacency, endpoints, node);

            if (candidate is not null && candidate.Value < best)
            {
                best = candidate.Value;
            }
        }

        return best == long.MaxValue ? LeetCodeAnswer.None : best;
    }

    private static long? MeetingCostAt(List<(int Neighbor, long Weight)>[] adjacency, PathEndpoints endpoints, int node)
    {
        var fromSrc1 = ShortestDistance(adjacency, endpoints.Src1, node);
        var fromSrc2 = ShortestDistance(adjacency, endpoints.Src2, node);
        var toDest = ShortestDistance(adjacency, node, endpoints.Dest);

        if (fromSrc1 is null || fromSrc2 is null || toDest is null)
        {
            return null;
        }

        return fromSrc1.Value + fromSrc2.Value + toDest.Value;
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
            adjacency[edge[0]].Add((edge[1], edge[2]));
        }

        return adjacency;
    }

    private static long? ShortestDistance(List<(int Neighbor, long Weight)>[] adjacency, int source, int target)
    {
        var distances = new long[adjacency.Length];
        Array.Fill(distances, long.MaxValue);
        distances[source] = 0;

        var settled = new bool[adjacency.Length];
        var queue = new PriorityQueue<int, long>();
        queue.Enqueue(source, 0);

        while (queue.TryDequeue(out var node, out _))
        {
            if (!settled[node])
            {
                settled[node] = true;

                if (node == target)
                {
                    return distances[node];
                }

                RelaxFrom(adjacency, distances, queue, node);
            }
        }

        return null;
    }

    private static void RelaxFrom(
        List<(int Neighbor, long Weight)>[] adjacency, long[] distances, PriorityQueue<int, long> queue, int node)
    {
        foreach (var (neighbor, weight) in adjacency[node])
        {
            var candidate = distances[node] + weight;

            if (candidate < distances[neighbor])
            {
                distances[neighbor] = candidate;
                queue.Enqueue(neighbor, candidate);
            }
        }
    }

    // This repo's own Dijkstra: ShortestPath.Dijkstra over RequiredPathsNode via
    // RequiredPathsTopology gives every reachable node's distance from a source in
    // one call, so three calls - forward from src1, forward from src2, and one on
    // the edge-reversed graph from dest - already hold every distance any candidate
    // meeting vertex needs. The reverse-graph trick is what turns O(V) independent
    // searches into O(1) dictionary lookups, the same "search once, answer many
    // queries" composition FindEdgesInShortestPathsSolution uses for LC 3123.
    public static long MinimumWeightByReverseGraphDijkstra(int n, int[][] edges, int src1, int src2, int dest)
    {
        var graph = RequiredPathsGraph.Build(n, edges);

        return MinimumWeightByReverseGraphDijkstra(graph, src1, src2, dest);
    }

    public static long MinimumWeightByReverseGraphDijkstra(RequiredPathsGraph graph, int src1, int src2, int dest)
    {
        var fromSrc1 = DistancesFrom(graph.Forward[src1]);
        var fromSrc2 = DistancesFrom(graph.Forward[src2]);
        var toDest = DistancesFrom(graph.Reverse[dest]);

        var best = long.MaxValue;

        for (var id = 0; id < graph.Forward.Length; id++)
        {
            if (fromSrc1.TryGetValue(graph.Forward[id], out var d1)
                && fromSrc2.TryGetValue(graph.Forward[id], out var d2)
                && toDest.TryGetValue(graph.Reverse[id], out var d3))
            {
                best = Math.Min(best, d1 + d2 + d3);
            }
        }

        return best == long.MaxValue ? LeetCodeAnswer.None : best;
    }

    private static Dictionary<RequiredPathsNode, long> DistancesFrom(RequiredPathsNode source)
        => ShortestPath
            .Dijkstra<RequiredPathsNode, RequiredPathsTopology, ListEdges<RequiredPathsNode, long>, long>(source);

    // The three vertices LeetCode's own signature passes as loose ints, bundled so
    // the per-candidate helper does not carry four positional ints of its own.
    private readonly record struct PathEndpoints(int Src1, int Src2, int Dest);
}
