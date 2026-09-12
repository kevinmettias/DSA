using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.RedundantConnection;

// LeetCode 684. Redundant Connection: given a tree with one extra edge added, find
// the extra edge - the first edge (in input order) that connects two nodes already
// in the same DisjointSet component.
internal static class RedundantConnectionSolution
{
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
