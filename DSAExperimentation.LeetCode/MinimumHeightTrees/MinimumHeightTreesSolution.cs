using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.MinimumHeightTrees;

// LeetCode 310. Minimum Height Trees: of every possible root, find those that
// minimize the tree's height - equivalently, the tree's own centroid(s) (at most 2
// for any tree).
//
// Both strategies build the same undirected adjacency list from the edge list; they
// differ only in how they search it.
internal static class MinimumHeightTreesSolution
{
    // At most 2 centroid nodes remain once leaf-peeling settles.
    private const int MaxCentroidCount = 2;

    // The textbook O(n^2) approach: BFS from every single node to measure the tree's
    // height when rooted there, then keep the nodes achieving the minimum.
    // Deliberately written without this repo's search primitives - it is the arm the
    // leaf-peeling strategy below has to justify itself against.
    public static List<int> FindRootsByHeightFromEveryNode(int n, int[][] edges) =>
        FindRootsByHeightFromEveryNode(BuildAdjacency(n, edges));

    public static List<int> FindRootsByHeightFromEveryNode(List<int>[] adjacency)
    {
        var minHeight = int.MaxValue;
        var roots = new List<int>();

        for (var root = 0; root < adjacency.Length; root++)
        {
            var height = HeightFrom(adjacency, root);
            if (height < minHeight)
            {
                minHeight = height;
                roots.Clear();
                roots.Add(root);
            }
            else if (height == minHeight)
            {
                roots.Add(root);
            }
        }

        return roots;
    }

    private static int HeightFrom(List<int>[] adjacency, int root)
    {
        var visited = new bool[adjacency.Length];
        visited[root] = true;
        var frontier = new Queue<int>();
        frontier.Enqueue(root);
        var height = -1;

        while (frontier.Count > 0)
        {
            var levelSize = frontier.Count;
            height++;

            for (var i = 0; i < levelSize; i++)
            {
                var node = frontier.Dequeue();

                foreach (var neighbor in adjacency[node])
                {
                    if (!visited[neighbor])
                    {
                        visited[neighbor] = true;
                        frontier.Enqueue(neighbor);
                    }
                }
            }
        }

        return height;
    }

    // Repeatedly peel the current leaves (degree-1 nodes) layer by layer, the same
    // "peel the zero-remaining-degree frontier with a queue" shape TopologicalSort's
    // Kahn's algorithm already uses for in-degree (its own BuildInDegree/
    // ReleaseChildren pair) - just tracking plain undirected degree instead of
    // in-degree, and stopping once at most 2 nodes remain instead of running to
    // exhaustion. This repo's own Queue<int> is the frontier, the same
    // RemoveInvalidParenthesesTests.cs uses for its own level-by-level BFS peel. The
    // last layer standing are the roots whose eccentricity - and therefore tree
    // height - is minimal, since they are the tree's own centroid(s).
    public static List<int> FindRootsByLeafPeeling(int n, int[][] edges) =>
        FindRootsByLeafPeeling(BuildAdjacency(n, edges));

    public static List<int> FindRootsByLeafPeeling(List<int>[] adjacency)
    {
        var n = adjacency.Length;

        if (n == 1)
        {
            return [0];
        }

        var degree = new int[n];
        for (var i = 0; i < n; i++)
        {
            degree[i] = adjacency[i].Count;
        }

        var leaves = new RepoQueue();
        for (var i = 0; i < n; i++)
        {
            if (degree[i] == 1)
            {
                leaves.Enqueue(i);
            }
        }

        var remaining = n;
        while (remaining > MaxCentroidCount)
        {
            var leafCount = leaves.Count;
            remaining -= leafCount;

            for (var i = 0; i < leafCount; i++)
            {
                leaves.TryDequeue(out var leaf);

                foreach (var neighbor in adjacency[leaf])
                {
                    if (--degree[neighbor] == 1)
                    {
                        leaves.Enqueue(neighbor);
                    }
                }
            }
        }

        var roots = new List<int>();
        while (leaves.TryDequeue(out var rootNode))
        {
            roots.Add(rootNode);
        }

        return roots;
    }

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];
        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }
}
