using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.ReachableNodesInSubdividedGraph;

// LeetCode 882. Reachable Nodes In Subdivided Graph: an undirected graph on n
// original nodes where edge i is replaced by a chain of cnt_i new subdivision nodes.
// Count how many nodes - original and subdivision alike - are within maxMoves unit
// moves of node 0.
//
// The composed strategy never materializes the subdivided graph. Weighting each
// original edge at cnt + 1 (the number of unit moves that edge's chain costs end to
// end) turns "distance in the subdivided graph" into "distance in the ORIGINAL
// graph", which is one ShortestPath.Dijkstra call. An original node is then reachable
// when its distance fits the budget; an edge's subdivision nodes are reachable from
// whichever endpoint has leftover budget to walk out to them, so each edge
// contributes min(cnt, fromU + fromV) - the cap being what stops the two sides'
// overlap from double-counting a node both ends can reach.
internal static class ReachableNodesInSubdividedGraphSolution
{
    // Sentinel for "no BFS distance assigned yet"; every real distance is >= 0.
    private const int Unvisited = -1;

    // Textbook baseline: build every subdivision node as a real vertex in a BCL
    // adjacency list and BFS the whole thing, deliberately without this repo's graph
    // engine. Its cost grows with the TOTAL subdivision count, which can dwarf the
    // original graph - that is the arm the strategy below has to justify itself
    // against.
    public static int CountReachableNodesByMaterializedBfs(int[][] edges, int maxMoves, int n)
    {
        var adjacency = BuildSubdividedAdjacency(edges, n);

        return CountReachableViaBfs(adjacency, maxMoves);
    }

    private static List<List<int>> BuildSubdividedAdjacency(int[][] edges, int n)
    {
        var adjacency = new List<List<int>>(n);

        for (var i = 0; i < n; i++)
        {
            adjacency.Add([]);
        }

        foreach (var edge in edges)
        {
            AppendSubdividedEdge(adjacency, edge);
        }

        return adjacency;
    }

    private static void AppendSubdividedEdge(List<List<int>> adjacency, int[] edge)
    {
        var (u, v, cnt) = (edge[0], edge[1], edge[2]);
        var previous = u;

        for (var k = 0; k < cnt; k++)
        {
            previous = AppendSubdivisionNode(adjacency, previous);
        }

        Connect(adjacency, previous, v);
    }

    // Appends one new subdivision node between `previous` and the rest of the chain,
    // returning the new node's id so the caller can keep threading the chain forward.
    private static int AppendSubdivisionNode(List<List<int>> adjacency, int previous)
    {
        var mid = adjacency.Count;
        adjacency.Add([]);
        Connect(adjacency, previous, mid);

        return mid;
    }

    private static void Connect(List<List<int>> adjacency, int a, int b)
    {
        adjacency[a].Add(b);
        adjacency[b].Add(a);
    }

    private static int CountReachableViaBfs(List<List<int>> adjacency, int maxMoves)
    {
        var distance = new int[adjacency.Count];
        Array.Fill(distance, Unvisited);
        distance[0] = 0;

        var queue = new Queue<int>();
        queue.Enqueue(0);
        var reachable = 0;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            reachable++;
            EnqueueNeighborsWithinBudget(adjacency, (distance, queue), node, maxMoves);
        }

        return reachable;
    }

    // The BFS's own bookkeeping - how far each node is, and which nodes are still
    // pending - is one piece of search state, created together and threaded together.
    private static void EnqueueNeighborsWithinBudget(
        List<List<int>> adjacency, (int[] Distance, Queue<int> Pending) search, int node, int maxMoves)
    {
        var (distance, queue) = search;

        foreach (var next in adjacency[node])
        {
            if (distance[next] != Unvisited)
            {
                continue;
            }

            distance[next] = distance[node] + 1;

            if (distance[next] <= maxMoves)
            {
                queue.Enqueue(next);
            }
        }
    }

    // This repo's own Dijkstra over just the ORIGINAL graph (edge weight =
    // subdivision count + 1), plus one arithmetic pass per edge that counts that
    // edge's reachable subdivision nodes analytically - cost independent of how large
    // any single edge's subdivision count is. The same "search once, answer many
    // queries" composition FindEdgesInShortestPathsSolution uses for LC 3123.
    public static int CountReachableNodesByDijkstra(int[][] edges, int maxMoves, int n)
    {
        var graph = SubdividedGraph.Build(n, edges);

        return CountReachableNodesByDijkstra(graph, maxMoves);
    }

    public static int CountReachableNodesByDijkstra(SubdividedGraph graph, int maxMoves)
    {
        var distances = ShortestPath.Dijkstra<
            SubdividedGraphNode, SubdividedGraphTopology, ListEdges<SubdividedGraphNode, int>, int>(graph.Nodes[0]);

        var reachable = CountOriginalNodesWithinBudget(graph, distances, maxMoves);

        return reachable + CountSubdivisionNodesWithinBudget(graph, distances, maxMoves);
    }

    private static int CountOriginalNodesWithinBudget(
        SubdividedGraph graph, Dictionary<SubdividedGraphNode, int> distances, int maxMoves)
    {
        var reachable = 0;

        foreach (var node in graph.Nodes)
        {
            if (distances.TryGetValue(node, out var distance) && distance <= maxMoves)
            {
                reachable++;
            }
        }

        return reachable;
    }

    private static int CountSubdivisionNodesWithinBudget(
        SubdividedGraph graph, Dictionary<SubdividedGraphNode, int> distances, int maxMoves)
    {
        var reachable = 0;

        foreach (var edge in graph.Edges)
        {
            reachable += CountEdgeSubdivisionsWithinBudget(graph, distances, edge, maxMoves);
        }

        return reachable;
    }

    private static int CountEdgeSubdivisionsWithinBudget(
        SubdividedGraph graph, Dictionary<SubdividedGraphNode, int> distances, int[] edge, int maxMoves)
    {
        var (u, v, cnt) = (edge[0], edge[1], edge[2]);
        var fromU = RemainingSubdivisionCapacity(distances, graph.Nodes[u], cnt, maxMoves);
        var fromV = RemainingSubdivisionCapacity(distances, graph.Nodes[v], cnt, maxMoves);

        return Math.Min(cnt, fromU + fromV);
    }

    // How many of an edge's `cnt` subdivision nodes are still within the move budget
    // when walked in from `node`, given the moves already spent reaching it.
    private static int RemainingSubdivisionCapacity(
        Dictionary<SubdividedGraphNode, int> distances, SubdividedGraphNode node, int cnt, int maxMoves)
    {
        if (!distances.TryGetValue(node, out var distance))
        {
            return 0;
        }

        var movesLeftOnEdge = Math.Min(cnt, maxMoves - distance);

        return Math.Max(0, movesLeftOnEdge);
    }
}
