using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.RedundantConnection;

// LeetCode 684. Redundant Connection: given a tree with one extra edge added, find
// the extra edge - the first edge (in input order) that connects two nodes already
// in the same DisjointSet component.
internal static class RedundantConnectionSolution
{
    // The textbook arm the disjoint set is measured against: keep the forest built so
    // far as an adjacency list and, for each edge in turn, search for a path between its
    // endpoints before linking them. Same verdict - the first edge whose endpoints are
    // already connected - but the connectivity test is a fresh walk per edge rather than
    // a near-constant-time find.
    public static int[] FindRedundantEdgeByPathSearch(int[][] edges)
    {
        var forest = new Dictionary<int, List<int>>();

        foreach (var edge in edges)
        {
            var (first, second) = (edge[0], edge[1]);

            if (IsConnected(forest, first, second))
            {
                return edge;
            }

            Link(forest, first, second);
            Link(forest, second, first);
        }

        return [];
    }

    private static bool IsConnected(Dictionary<int, List<int>> forest, int from, int to)
    {
        if (from == to)
        {
            return true;
        }

        var seen = new HashSet<int> { from };
        var frontier = new Stack<int>();
        frontier.Push(from);

        while (frontier.Count > 0)
        {
            if (!forest.TryGetValue(frontier.Pop(), out var neighbours))
            {
                continue;
            }

            foreach (var neighbour in neighbours)
            {
                if (neighbour == to)
                {
                    return true;
                }

                if (seen.Add(neighbour))
                {
                    frontier.Push(neighbour);
                }
            }
        }

        return false;
    }

    private static void Link(Dictionary<int, List<int>> forest, int from, int to)
    {
        if (!forest.TryGetValue(from, out var neighbours))
        {
            neighbours = [];
            forest[from] = neighbours;
        }

        neighbours.Add(to);
    }

    // Precondition (guaranteed by LC 684's own constraints): edges describes a tree
    // plus exactly one extra edge, so the loop below always returns before falling
    // through.
    public static int[] FindRedundantEdgeByDisjointSet(int[][] edges)
    {
        var components = new DisjointSet(edges.Length + 1);

        foreach (var edge in edges)
        {
            var (first, second) = (edge[0], edge[1]);

            if (components.IsConnected(first, second))
            {
                return edge;
            }

            components.Union(first, second);
        }

        return [];
    }
}
