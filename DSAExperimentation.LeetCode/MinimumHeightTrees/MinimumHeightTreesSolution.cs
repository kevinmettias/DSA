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
    public static List<int> FindRootsByHeightFromEveryNode(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        return FindRootsByHeightFromEveryNode(adjacency);
    }

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
                ExpandNeighbors(adjacency, visited, frontier, node);
            }
        }

        return height;
    }

    // One step of the level-by-level walk: enqueue every unvisited neighbor of `node`.
    private static void ExpandNeighbors(List<int>[] adjacency, bool[] visited, Queue<int> frontier, int node)
    {
        foreach (var neighbor in adjacency[node])
        {
            if (!visited[neighbor])
            {
                visited[neighbor] = true;
                frontier.Enqueue(neighbor);
            }
        }
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
    public static List<int> FindRootsByLeafPeeling(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        return FindRootsByLeafPeeling(adjacency);
    }

    public static List<int> FindRootsByLeafPeeling(List<int>[] adjacency)
    {
        var n = adjacency.Length;

        if (n == 1)
        {
            return [0];
        }

        var degree = BuildDegrees(adjacency);
        var leaves = SeedLeaves(degree);

        PeelToCentroids(adjacency, degree, leaves);

        return DrainRoots(leaves);
    }

    // Undirected degree of every node: the adjacency list's own count, the same quantity
    // Kahn's algorithm tracks as in-degree for the directed case.
    private static int[] BuildDegrees(List<int>[] adjacency)
    {
        var degree = new int[adjacency.Length];
        for (var i = 0; i < degree.Length; i++)
        {
            degree[i] = adjacency[i].Count;
        }

        return degree;
    }

    // The frontier's first layer: every degree-1 node.
    private static RepoQueue SeedLeaves(int[] degree)
    {
        var leaves = new RepoQueue();
        for (var i = 0; i < degree.Length; i++)
        {
            if (degree[i] == 1)
            {
                leaves.Enqueue(i);
            }
        }

        return leaves;
    }

    // Peel one whole layer at a time until at most MaxCentroidCount nodes are left. Each
    // pass removes the currently queued leaves and, in doing so, releases the next layer.
    private static void PeelToCentroids(List<int>[] adjacency, int[] degree, RepoQueue leaves)
    {
        var remaining = adjacency.Length;
        while (remaining > MaxCentroidCount)
        {
            var leafCount = leaves.Count;
            remaining -= leafCount;

            for (var i = 0; i < leafCount; i++)
            {
                leaves.TryDequeue(out var leaf);
                ReleaseNeighbors(adjacency, degree, leaves, leaf);
            }
        }
    }

    // Dropping `leaf` from its neighbors' degrees promotes any neighbor left at degree 1
    // into the next layer.
    private static void ReleaseNeighbors(List<int>[] adjacency, int[] degree, RepoQueue leaves, int leaf)
    {
        foreach (var neighbor in adjacency[leaf])
        {
            if (--degree[neighbor] == 1)
            {
                leaves.Enqueue(neighbor);
            }
        }
    }

    // What is left in the frontier once peeling settles is the last layer: the centroids.
    private static List<int> DrainRoots(RepoQueue leaves)
    {
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
