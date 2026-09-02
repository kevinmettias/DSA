using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

// LeetCode 3123. Find Edges in Shortest Paths: an undirected weighted graph of
// n nodes (0..n-1, possibly disconnected). For every edge, report whether it
// lies on at least one shortest path from node 0 to node n-1.
//
// An edge (u, v, w) lies on some shortest 0->(n-1) path exactly when
// dist(0, u) + w + dist(v, n-1) equals the overall shortest distance, or the
// same check with u and v swapped (a shortest path may cross the edge in
// either direction). Running one Dijkstra from node 0 and one from node n-1
// gives every distance either check needs, instead of a fresh search per
// edge. When 0 and n-1 are not connected at all, no edge belongs to any
// shortest path, so the answer is all false.
internal static class FindEdgesInShortestPathsSolution
{
    // Textbook baseline: BCL adjacency lists plus PriorityQueue<int,long>
    // Dijkstra, run once from each end - deliberately without this repo's own
    // graph engine, the arm the composed strategy below has to justify itself
    // against.
    public static bool[] AnswerByBruteForceDijkstra(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        var distFromStart = Dijkstra(adjacency, 0);
        var distFromEnd = Dijkstra(adjacency, n - 1);

        return BuildAnswer(edges, distFromStart, distFromEnd);
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
            var (a, b, w) = (edge[0], edge[1], (long)edge[2]);
            adjacency[a].Add((b, w));
            adjacency[b].Add((a, w));
        }

        return adjacency;
    }

    private static long[] Dijkstra(List<(int Neighbor, long Weight)>[] adjacency, int source)
    {
        var dist = new long[adjacency.Length];
        Array.Fill(dist, long.MaxValue);
        dist[source] = 0;

        var settled = new bool[adjacency.Length];
        var queue = new PriorityQueue<int, long>();
        queue.Enqueue(source, 0);

        while (queue.TryDequeue(out var node, out _))
        {
            if (!settled[node])
            {
                settled[node] = true;
                RelaxFrom(adjacency, dist, queue, node);
            }
        }

        return dist;
    }

    private static void RelaxFrom(
        List<(int Neighbor, long Weight)>[] adjacency, long[] dist, PriorityQueue<int, long> queue, int node)
    {
        foreach (var (neighbor, weight) in adjacency[node])
        {
            var candidate = dist[node] + weight;

            if (candidate < dist[neighbor])
            {
                dist[neighbor] = candidate;
                queue.Enqueue(neighbor, candidate);
            }
        }
    }

    private static bool[] BuildAnswer(int[][] edges, long[] distFromStart, long[] distFromEnd)
    {
        var answer = new bool[edges.Length];
        var shortest = distFromStart[^1];

        if (shortest == long.MaxValue)
        {
            return answer;
        }

        for (var i = 0; i < edges.Length; i++)
        {
            var edge = edges[i];
            var (a, b, w) = (edge[0], edge[1], (long)edge[2]);

            answer[i] = OnShortestPath(distFromStart, distFromEnd, a, b, w, shortest)
                || OnShortestPath(distFromStart, distFromEnd, b, a, w, shortest);
        }

        return answer;
    }

    private static bool OnShortestPath(long[] distFromStart, long[] distFromEnd, int u, int v, long w, long shortest)
        => distFromStart[u] != long.MaxValue && distFromEnd[v] != long.MaxValue
            && distFromStart[u] + w + distFromEnd[v] == shortest;

    // This repo's own Dijkstra: ShortestPath.Dijkstra over EdgeGraphNode via
    // EdgeGraphTopology gives every reachable node's distance from a source in
    // one call, so the puzzle reduces to two calls (from node 0, from node
    // n-1) plus a per-edge lookup - the same "search once, answer many
    // queries" composition OpenTheLockSolution's MinTurnsByReduceGraph uses
    // for its own graph-reduce arm.
    public static bool[] AnswerByShortestPathDijkstra(int n, int[][] edges) =>
        AnswerByShortestPathDijkstra(EdgeGraph.Build(n, edges));

    public static bool[] AnswerByShortestPathDijkstra(EdgeGraph graph)
    {
        var nodes = graph.Nodes;
        var distFromStart = ShortestPath
            .Dijkstra<EdgeGraphNode, EdgeGraphTopology, ListEdges<EdgeGraphNode, long>, long>(nodes[0]);
        var distFromEnd = ShortestPath
            .Dijkstra<EdgeGraphNode, EdgeGraphTopology, ListEdges<EdgeGraphNode, long>, long>(nodes[^1]);

        var answer = new bool[graph.Edges.Length];

        if (!distFromStart.TryGetValue(nodes[^1], out var shortest))
        {
            return answer;
        }

        for (var i = 0; i < graph.Edges.Length; i++)
        {
            var edge = graph.Edges[i];
            var (a, b, w) = (edge[0], edge[1], (long)edge[2]);

            answer[i] = OnShortestPath(nodes, distFromStart, distFromEnd, a, b, w, shortest)
                || OnShortestPath(nodes, distFromStart, distFromEnd, b, a, w, shortest);
        }

        return answer;
    }

    private static bool OnShortestPath(
        EdgeGraphNode[] nodes,
        Dictionary<EdgeGraphNode, long> distFromStart,
        Dictionary<EdgeGraphNode, long> distFromEnd,
        int u, int v, long w, long shortest)
        => distFromStart.TryGetValue(nodes[u], out var du)
            && distFromEnd.TryGetValue(nodes[v], out var dv)
            && du + w + dv == shortest;
}
