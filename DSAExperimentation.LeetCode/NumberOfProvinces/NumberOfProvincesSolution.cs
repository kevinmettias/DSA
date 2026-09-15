using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.NumberOfProvinces;

// LeetCode 547. Number of Provinces: count the connected components of the
// adjacency matrix isConnected describes.
//
// Both strategies touch every cell of the n x n matrix, so this is not a different
// asymptotic class - Union-Find's edge is near-constant-time merging via path
// compression/union-by-rank instead of DFS's own recursion and visited-array
// bookkeeping.
internal static class NumberOfProvincesSolution
{
    // The textbook answer: a DFS flood-fill over the adjacency matrix with an
    // explicit visited array. Deliberately written without this repo's primitives;
    // it is the arm the composed solution below has to justify itself against.
    public static int CountProvincesByDepthFirstFloodFill(int[][] isConnected)
    {
        var cityCount = isConnected.Length;
        var visited = new bool[cityCount];
        var provinces = 0;

        for (var i = 0; i < cityCount; i++)
        {
            if (visited[i])
            {
                continue;
            }

            Visit(i, isConnected, visited);
            provinces++;
        }

        return provinces;
    }

    // Union every isConnected[i][j] pair into this repo's own DisjointSet, then
    // count distinct roots with this repo's own Set<int> - the same DisjointSet
    // RedundantConnectionTests already uses to detect a cycle-closing edge, just
    // counting components at the end instead of stopping at the first edge that
    // finds two nodes already joined.
    public static int CountProvincesByDisjointSetUnionFind(int[][] isConnected)
    {
        var cityCount = isConnected.Length;
        var components = new DisjointSet(cityCount);

        for (var i = 0; i < cityCount; i++)
        {
            for (var j = i + 1; j < cityCount; j++)
            {
                if (isConnected[i][j] == 1)
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new Set<int>();
        for (var i = 0; i < cityCount; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count;
    }

    private static void Visit(int city, int[][] isConnected, bool[] visited)
    {
        visited[city] = true;

        for (var next = 0; next < isConnected.Length; next++)
        {
            if (isConnected[city][next] == 1 && !visited[next])
            {
                Visit(next, isConnected, visited);
            }
        }
    }
}
