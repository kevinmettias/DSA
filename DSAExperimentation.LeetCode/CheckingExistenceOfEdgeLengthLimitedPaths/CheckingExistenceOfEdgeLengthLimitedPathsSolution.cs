using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.CheckingExistenceOfEdgeLengthLimitedPaths;

// LeetCode 1697. Checking Existence of Edge Length Limited Paths: for each query
// (p, q, limit), report whether p and q are joined by a path whose every edge is
// strictly lighter than limit.
//
// Both strategies answer the same bool[] in the queries' original order; they
// differ in whether each query is answered from scratch or all of them are
// answered by one sweep. The edge list is undirected and may hold parallel edges
// of different weights, so a heavier duplicate never invalidates a lighter one.
internal static class CheckingExistenceOfEdgeLengthLimitedPathsSolution
{
    // The textbook baseline this composition has to justify itself against:
    // rebuild nothing, but walk the graph once per query with a depth-first
    // search that simply refuses to cross an edge at or above that query's
    // limit. Deliberately plain BCL - adjacency lists, a Stack<int> and a
    // visited flag array - so the (q * (n + e)) cost is exactly what you would
    // pay writing this without the repo.
    public static bool[] DistanceLimitedPathsExistByPerQueryDfs(int n, int[][] edgeList, int[][] queries)
    {
        var adjacency = BuildAdjacency(n, edgeList);
        var results = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var query = queries[i];
            results[i] = HasLimitedPath(adjacency, query[0], new PathQuery(query[1], query[2]));
        }

        return results;
    }

    private static List<(int To, int Weight)>[] BuildAdjacency(int n, int[][] edgeList)
    {
        var adjacency = new List<(int To, int Weight)>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edgeList)
        {
            var (from, to, weight) = (edge[0], edge[1], edge[2]);
            adjacency[from].Add((to, weight));
            adjacency[to].Add((from, weight));
        }

        return adjacency;
    }

    // A node always reaches itself, however tight the limit - there is no edge to
    // cross - so the empty path is answered before any search starts.
    private static bool HasLimitedPath(List<(int To, int Weight)>[] adjacency, int start, PathQuery query)
    {
        if (start == query.Target)
        {
            return true;
        }

        return TraverseForTarget(adjacency, start, query);
    }

    private static bool TraverseForTarget(List<(int To, int Weight)>[] adjacency, int start, PathQuery query)
    {
        var visited = new bool[adjacency.Length];
        var stack = new Stack<int>();
        stack.Push(start);
        visited[start] = true;

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (VisitNeighbors(adjacency[node], query, visited, stack))
            {
                return true;
            }
        }

        return false;
    }

    private static bool VisitNeighbors(List<(int To, int Weight)> neighbors, PathQuery query, bool[] visited, Stack<int> stack)
    {
        foreach (var (to, weight) in neighbors)
        {
            if (weight >= query.Limit || visited[to])
            {
                continue;
            }

            if (to == query.Target)
            {
                return true;
            }

            visited[to] = true;
            stack.Push(to);
        }

        return false;
    }

    private readonly record struct PathQuery(int Target, int Limit);

    // The composed answer: the standard offline Kruskal-style sweep over this
    // repo's own DisjointSet - the same primitive
    // FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree and
    // GraphConnectivityWithThreshold compose. Sort edges by weight and queries by
    // limit, both ascending, then walk queries in limit order: before answering
    // one, union every edge still strictly below its limit. The edge cursor only
    // ever advances, so the whole edge list is swept once across every query
    // rather than once per query, and each answer is a single IsConnected lookup.
    public static bool[] DistanceLimitedPathsExistByOfflineDisjointSet(int n, int[][] edgeList, int[][] queries)
    {
        var edgesByWeight = edgeList.OrderBy(edge => edge[2]).ToArray();
        var queryOrder = Enumerable.Range(0, queries.Length).OrderBy(i => queries[i][2]).ToArray();

        var components = new DisjointSet(n);
        var results = new bool[queries.Length];
        var edgeIndex = 0;

        foreach (var queryIndex in queryOrder)
        {
            var query = queries[queryIndex];

            edgeIndex = UnionEdgesBelow(components, edgesByWeight, edgeIndex, query[2]);
            results[queryIndex] = components.IsConnected(query[0], query[1]);
        }

        return results;
    }

    // Consumes every remaining edge strictly lighter than limit and reports where
    // the cursor stopped; the next query's limit is no smaller, so it resumes from
    // there instead of restarting.
    private static int UnionEdgesBelow(DisjointSet components, int[][] edgesByWeight, int edgeIndex, int limit)
    {
        while (edgeIndex < edgesByWeight.Length)
        {
            var edge = edgesByWeight[edgeIndex];

            if (edge[2] >= limit)
            {
                break;
            }

            components.Union(edge[0], edge[1]);
            edgeIndex++;
        }

        return edgeIndex;
    }
}
