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
    public static long MinimumWeightByPerNodeSearch(int n, int[][] edges, PathEndpoints endpoints)
    {
        var adjacency = BuildAdjacency(n, edges);
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

        if (HasAnUnreachableLeg(fromSrc1, fromSrc2, toDest))
        {
            return null;
        }

        return fromSrc1.Value + fromSrc2.Value + toDest.Value;
    }

    // A meeting vertex only counts as a candidate when all three legs of the route -
    // both sources in, the destination out - have a distance at all.
    private static bool HasAnUnreachableLeg(long? fromSrc1, long? fromSrc2, long? toDest) =>
        fromSrc1 is null || fromSrc2 is null || toDest is null;

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
        var (distances, settled) = StartSearch(adjacency.Length, source);
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

    // Every node starts unreached and unsettled; the caller seeds the frontier with
    // the single source node at distance zero.
    private static (long[] Distances, bool[] Settled) StartSearch(int nodeCount, int source)
    {
        var distances = new long[nodeCount];
        Array.Fill(distances, long.MaxValue);
        distances[source] = 0;

        var settled = new bool[nodeCount];

        return (distances, settled);
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
    public static long MinimumWeightByReverseGraphDijkstra(int n, int[][] edges, PathEndpoints endpoints)
    {
        var graph = RequiredPathsGraph.Build(n, edges);

        return MinimumWeightByReverseGraphDijkstra(graph, endpoints);
    }

    public static long MinimumWeightByReverseGraphDijkstra(RequiredPathsGraph graph, PathEndpoints endpoints)
    {
        var fromSrc1 = DistancesFrom(graph.Forward[endpoints.Src1]);
        var fromSrc2 = DistancesFrom(graph.Forward[endpoints.Src2]);
        var toDest = DistancesFrom(graph.Reverse[endpoints.Dest]);

        var best = long.MaxValue;

        for (var id = 0; id < graph.Forward.Length; id++)
        {
            if (TryGetSourceDistanceTotal(fromSrc1, fromSrc2, graph.Forward[id], out var sourceTotal)
                && toDest.TryGetValue(graph.Reverse[id], out var distanceToDest))
            {
                best = Math.Min(best, sourceTotal + distanceToDest);
            }
        }

        return best == long.MaxValue ? LeetCodeAnswer.None : best;
    }

    // Both sources have to reach the meeting vertex for it to be a candidate. They are
    // looked up on the same forward vertex, and only their combined distance matters.
    private static bool TryGetSourceDistanceTotal(
        Dictionary<RequiredPathsNode, long> fromSrc1,
        Dictionary<RequiredPathsNode, long> fromSrc2,
        RequiredPathsNode vertex,
        out long total)
    {
        if (!fromSrc1.TryGetValue(vertex, out var fromFirstSource)
            || !fromSrc2.TryGetValue(vertex, out var fromSecondSource))
        {
            total = 0;
            return false;
        }

        total = fromFirstSource + fromSecondSource;
        return true;
    }

    private static Dictionary<RequiredPathsNode, long> DistancesFrom(RequiredPathsNode source)
        => ShortestPath
            .Dijkstra<RequiredPathsNode, RequiredPathsTopology, ListEdges<RequiredPathsNode, long>, long>(source);

    // The three vertices LeetCode's own signature passes as loose ints, bundled once at
    // the public entry points and handed on unchanged to every helper that needs them -
    // internal rather than private because those entry points are public.
    internal readonly record struct PathEndpoints(int Src1, int Src2, int Dest);
}
