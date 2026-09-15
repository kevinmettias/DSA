namespace DSAExperimentation.LeetCode.MinimumCostWalkInWeightedGraph;

// LeetCode 3108. Minimum Cost Walk in Weighted Graph: for each query [s, t], the
// minimum cost of ANY walk (edges and vertices may repeat any number of times)
// from s to t, where a walk's cost is the bitwise AND of every edge it traverses.
//
// A walk can retrace its steps freely, so nothing stops it from visiting every
// edge of s's connected component before ending at t, which drives the cost down
// to the AND of every one of those edges and no further. s and t in different
// components have no walk between them at all.
internal static class MinimumCostWalkInWeightedGraphSolution
{
    // Baseline: re-derives each query's component from scratch with a BFS over a
    // BCL adjacency list, ANDing every edge crossed along the way - "what you'd
    // write without this repo," recomputed per query rather than precomputed once.
    public static int[] MinimumCostByBruteForceWalk(int n, int[][] edges, int[][] query)
    {
        var adjacency = BuildAdjacency(n, edges);
        var answers = new int[query.Length];

        for (var i = 0; i < query.Length; i++)
        {
            answers[i] = ComponentWalkCost(adjacency, query[i][0], query[i][1]);
        }

        return answers;
    }

    private static List<(int Neighbor, int Weight)>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<(int Neighbor, int Weight)>[n];

        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add((edge[1], edge[2]));
            adjacency[edge[1]].Add((edge[0], edge[2]));
        }

        return adjacency;
    }

    // Every edge incident to any node this BFS ever dequeues belongs to source's
    // component (its other endpoint is reachable by that very edge), so ANDing
    // every neighbor entry seen - not just the ones that extend the BFS tree -
    // covers the whole component's edge set, tree edges and cross edges alike.
    private static int ComponentWalkCost(List<(int Neighbor, int Weight)>[] adjacency, int source, int target)
    {
        var visited = new HashSet<int> { source };
        var frontier = new Queue<int>();
        frontier.Enqueue(source);

        var (reached, cost) = FloodComponent(adjacency, frontier, visited, target);

        return reached ? cost : LeetCodeAnswer.None;
    }

    private static (bool Reached, int Cost) FloodComponent(
        List<(int Neighbor, int Weight)>[] adjacency, Queue<int> frontier, HashSet<int> visited, int target)
    {
        var reached = false;
        var cost = -1;

        while (frontier.Count > 0)
        {
            var node = frontier.Dequeue();

            foreach (var (neighbor, weight) in adjacency[node])
            {
                cost &= weight;
                reached |= neighbor == target;

                if (visited.Add(neighbor))
                {
                    frontier.Enqueue(neighbor);
                }
            }
        }

        return (reached, cost);
    }

    // Composed: DataStructures.DisjointSet groups every vertex into a component
    // once; WalkCostComponents folds each component's AND alongside it, so every
    // query after that first pass is a single Find plus an array lookup.
    public static int[] MinimumCostByUnionFind(int n, int[][] edges, int[][] query)
    {
        var components = WalkCostComponents.Build(n, edges);
        return MinimumCostByUnionFind(components, query);
    }

    public static int[] MinimumCostByUnionFind(WalkCostComponents components, int[][] query)
    {
        var answers = new int[query.Length];

        for (var i = 0; i < query.Length; i++)
        {
            answers[i] = components.MinimumCost(query[i][0], query[i][1]);
        }

        return answers;
    }
}
