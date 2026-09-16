using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

// LeetCode 3123. Find Edges in Shortest Paths: an undirected weighted graph of
// nodeCount nodes, 0-indexed and possibly disconnected. For every edge, report
// whether it lies on at least one shortest path from node 0 to the last node.
//
// An edge (u, v, w) lies on some shortest 0->(n-1) path exactly when
// dist(0, u) + w + dist(v, n-1) equals the overall shortest distance, or the
// same check with u and v swapped (a shortest path may cross the edge in
// either direction). Running one Dijkstra from node 0 and one from the last
// node gives every distance either check needs, instead of a fresh search per
// edge. When nodes 0 and n-1 are not connected at all, no edge belongs to any
// shortest path, so the answer is all false.
internal static class FindEdgesInShortestPathsSolution
{
    // Textbook baseline: BCL adjacency lists plus PriorityQueue<int,long>
    // Dijkstra, run once from each end - deliberately without this repo's own
    // graph engine, the arm the composed strategy below has to justify itself
    // against.
    public static bool[] AnswerByBruteForceDijkstra(int nodeCount, int[][] edges)
    {
        var adjacency = BuildAdjacency(nodeCount, edges);
        var distFromStart = Dijkstra(adjacency, 0);
        var distFromEnd = Dijkstra(adjacency, nodeCount - 1);

        return BuildAnswer(edges, distFromStart, distFromEnd);
    }

    private static List<(int Neighbor, long Weight)>[] BuildAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<(int Neighbor, long Weight)>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
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

            answer[i] = IsOnShortestPath(distFromStart, distFromEnd, (a, b, w), shortest)
                || IsOnShortestPath(distFromStart, distFromEnd, (b, a, w), shortest);
        }

        return answer;
    }

    private static bool IsOnShortestPath(
        long[] distFromStart, long[] distFromEnd, (int U, int V, long W) edge, long shortest)
        => distFromStart[edge.U] != long.MaxValue && distFromEnd[edge.V] != long.MaxValue
            && distFromStart[edge.U] + edge.W + distFromEnd[edge.V] == shortest;

    // This repo's own Dijkstra: ShortestPath.Dijkstra over EdgeGraphNode via
    // EdgeGraphTopology gives every reachable node's distance from a source in
    // one call, so the puzzle reduces to two calls (from node 0, from node
    // n-1) plus a per-edge lookup - the same "search once, answer many
    // queries" composition OpenTheLockSolution's MinTurnsByReduceGraph uses
    // for its own graph-reduce arm.
    public static bool[] AnswerByShortestPathDijkstra(int nodeCount, int[][] edges)
    {
        var graph = EdgeGraph.Build(nodeCount, edges);

        return AnswerByShortestPathDijkstra(graph);
    }

    public static bool[] AnswerByShortestPathDijkstra(EdgeGraph graph)
    {
        var nodes = graph.Nodes;
        var distFromStart = ShortestPath
            .Dijkstra<EdgeGraphNode, EdgeGraphTopology, ListEdges<EdgeGraphNode, long>, long>(nodes[0]);
        var distFromEnd = ShortestPath
            .Dijkstra<EdgeGraphNode, EdgeGraphTopology, ListEdges<EdgeGraphNode, long>, long>(nodes[^1]);
        var distances = (distFromStart, distFromEnd);

        if (!distFromStart.TryGetValue(nodes[^1], out var shortest))
        {
            return new bool[graph.Edges.Length];
        }

        return MarkShortestPathEdges(graph, nodes, distances, shortest);
    }

    // The per-edge test run over the whole graph: an edge lies on some shortest
    // 0 -> (n-1) path when either of its two orientations has
    // dist(0, u) + w + dist(v, n-1) equal to the overall shortest distance.
    private static bool[] MarkShortestPathEdges(
        EdgeGraph graph,
        EdgeGraphNode[] nodes,
        (Dictionary<EdgeGraphNode, long> FromStart, Dictionary<EdgeGraphNode, long> FromEnd) distances,
        long shortest)
    {
        var answer = new bool[graph.Edges.Length];

        for (var i = 0; i < graph.Edges.Length; i++)
        {
            var edge = graph.Edges[i];
            var (a, b, w) = (edge[0], edge[1], (long)edge[2]);

            answer[i] = IsOnShortestPath(nodes, distances, (a, b, w), shortest)
                || IsOnShortestPath(nodes, distances, (b, a, w), shortest);
        }

        return answer;
    }

    private static bool IsOnShortestPath(
        EdgeGraphNode[] nodes,
        (Dictionary<EdgeGraphNode, long> FromStart, Dictionary<EdgeGraphNode, long> FromEnd) distances,
        (int U, int V, long W) edge,
        long shortest)
        => distances.FromStart.TryGetValue(nodes[edge.U], out var du)
            && distances.FromEnd.TryGetValue(nodes[edge.V], out var dv)
            && du + edge.W + dv == shortest;
}
