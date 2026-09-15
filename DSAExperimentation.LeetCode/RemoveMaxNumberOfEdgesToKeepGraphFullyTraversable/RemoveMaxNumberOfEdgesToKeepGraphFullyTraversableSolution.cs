using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.RemoveMaxNumberOfEdgesToKeepGraphFullyTraversable;

// LeetCode 1579. Remove Max Number of Edges to Keep Graph Fully Traversable: given
// edges typed 1 (Alice only), 2 (Bob only) and 3 (both), remove as many as possible
// while both traversers can still reach every node; -1 when that is impossible.
//
// Both strategies run the same greedy: offer every type-3 edge to both traversers
// first, then the single-owner edges to their own traverser, and keep an edge only
// when its endpoints were not already connected. Preferring shared edges is what
// makes the removal count maximal - a shared edge can never cost more than the two
// single-owner edges it would otherwise be replaced by - and the answer is
// edges.Length minus the kept count, or -1 if either traverser ends up with more
// than one component.
//
// They differ only in how "are these two already connected?" is answered: the
// baseline floods the incrementally-built adjacency list breadth-first from scratch
// on every edge, while the composed strategy asks two of this repo's own DisjointSet
// instances, one per traverser - the same flood-fill-vs-union-find contrast
// NumberOfOperationsToMakeNetworkConnected draws, here needing a per-edge
// reachability query rather than one final component count.
internal static class RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableSolution
{
    private const int EdgeTypeIndex = 0;
    private const int EdgeSourceIndex = 1;
    private const int EdgeTargetIndex = 2;

    private const int AliceOnlyEdgeType = 1;
    private const int BobOnlyEdgeType = 2;
    private const int BothOwnersEdgeType = 3;

    // LeetCode labels nodes 1..n; both strategies below index from 0.
    private const int FirstNodeLabel = 1;

    // The textbook answer: adjacency lists plus a BFS flood fill per connectivity
    // question - what you would write without a union-find. BCL containers only.
    public static int MaxNumEdgesToRemoveByFloodFill(int n, int[][] edges)
    {
        var alice = BuildEmptyAdjacency(n);
        var bob = BuildEmptyAdjacency(n);

        var usedEdges = ConnectSharedEdges(alice, bob, edges);
        usedEdges += ConnectOwnEdgesForBoth(alice, bob, edges);

        return IsFullyConnected(alice) && IsFullyConnected(bob)
            ? RemovableEdgeCount(edges, usedEdges)
            : LeetCodeAnswer.None;
    }

    private static List<int>[] BuildEmptyAdjacency(int n)
    {
        var adjacency = new List<int>[n];

        for (var node = 0; node < n; node++)
        {
            adjacency[node] = [];
        }

        return adjacency;
    }

    private static int ConnectSharedEdges(List<int>[] alice, List<int>[] bob, int[][] edges)
    {
        var used = 0;

        foreach (var edge in edges)
        {
            if (edge[EdgeTypeIndex] != BothOwnersEdgeType)
            {
                continue;
            }

            var (u, v) = Endpoints(edge);
            if (CanReach(alice, u, v))
            {
                continue;
            }

            Connect(alice, u, v);
            Connect(bob, u, v);
            used++;
        }

        return used;
    }

    private static int ConnectOwnEdges(List<int>[] adjacency, int[][] edges, int ownerType)
    {
        var used = 0;

        foreach (var edge in edges)
        {
            if (edge[EdgeTypeIndex] != ownerType)
            {
                continue;
            }

            var (u, v) = Endpoints(edge);
            if (CanReach(adjacency, u, v))
            {
                continue;
            }

            Connect(adjacency, u, v);
            used++;
        }

        return used;
    }

    // Each traverser then takes the single-owner edges only it can use.
    private static int ConnectOwnEdgesForBoth(List<int>[] alice, List<int>[] bob, int[][] edges) =>
        ConnectOwnEdges(alice, edges, AliceOnlyEdgeType) + ConnectOwnEdges(bob, edges, BobOnlyEdgeType);

    private static void Connect(List<int>[] adjacency, int u, int v)
    {
        adjacency[u].Add(v);
        adjacency[v].Add(u);
    }

    private static bool CanReach(List<int>[] adjacency, int source, int target)
    {
        var visited = new bool[adjacency.Length];
        var queue = new Queue<int>();
        queue.Enqueue(source);
        visited[source] = true;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == target)
            {
                return true;
            }

            EnqueueUnvisitedNeighbors(adjacency, current, visited, queue);
        }

        return false;
    }

    private static bool IsFullyConnected(List<int>[] adjacency)
    {
        var visited = new bool[adjacency.Length];
        var queue = new Queue<int>();
        queue.Enqueue(0);
        visited[0] = true;
        var visitedCount = 1;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            visitedCount += EnqueueUnvisitedNeighbors(adjacency, current, visited, queue);
        }

        return visitedCount == adjacency.Length;
    }

    private static int EnqueueUnvisitedNeighbors(
        List<int>[] adjacency,
        int node,
        bool[] visited,
        Queue<int> queue)
    {
        var enqueued = 0;

        foreach (var next in adjacency[node])
        {
            if (!visited[next])
            {
                visited[next] = true;
                queue.Enqueue(next);
                enqueued++;
            }
        }

        return enqueued;
    }

    // This repo's own DisjointSet, one instance per traverser: the same greedy, with
    // every "already connected?" question answered in near-constant amortized time by
    // Find/IsConnected instead of a fresh graph walk.
    public static int MaxNumEdgesToRemoveByDisjointSet(int n, int[][] edges)
    {
        var alice = new DisjointSet(n);
        var bob = new DisjointSet(n);

        var usedEdges = UnionSharedEdges(alice, bob, edges);
        usedEdges += UnionOwnEdgesForBoth(alice, bob, edges);

        return IsFullyConnected(alice) && IsFullyConnected(bob)
            ? RemovableEdgeCount(edges, usedEdges)
            : LeetCodeAnswer.None;
    }

    private static int UnionSharedEdges(DisjointSet alice, DisjointSet bob, int[][] edges)
    {
        var used = 0;

        foreach (var edge in edges)
        {
            if (edge[EdgeTypeIndex] != BothOwnersEdgeType)
            {
                continue;
            }

            var (u, v) = Endpoints(edge);
            if (alice.IsConnected(u, v))
            {
                continue;
            }

            alice.Union(u, v);
            bob.Union(u, v);
            used++;
        }

        return used;
    }

    private static int UnionOwnEdges(DisjointSet components, int[][] edges, int ownerType)
    {
        var used = 0;

        foreach (var edge in edges)
        {
            if (edge[EdgeTypeIndex] != ownerType)
            {
                continue;
            }

            var (u, v) = Endpoints(edge);
            if (components.IsConnected(u, v))
            {
                continue;
            }

            components.Union(u, v);
            used++;
        }

        return used;
    }

    // Each traverser then takes the single-owner edges only it can use.
    private static int UnionOwnEdgesForBoth(DisjointSet alice, DisjointSet bob, int[][] edges) =>
        UnionOwnEdges(alice, edges, AliceOnlyEdgeType) + UnionOwnEdges(bob, edges, BobOnlyEdgeType);

    private static bool IsFullyConnected(DisjointSet components)
    {
        var root = components.Find(0);

        for (var node = 1; node < components.Count; node++)
        {
            if (components.Find(node) != root)
            {
                return false;
            }
        }

        return true;
    }

    private static (int U, int V) Endpoints(int[] edge) =>
        (edge[EdgeSourceIndex] - FirstNodeLabel, edge[EdgeTargetIndex] - FirstNodeLabel);

    // The edges no traversal needed, so the answer is however many of them exist.
    private static int RemovableEdgeCount(int[][] edges, int usedEdges) => edges.Length - usedEdges;
}
