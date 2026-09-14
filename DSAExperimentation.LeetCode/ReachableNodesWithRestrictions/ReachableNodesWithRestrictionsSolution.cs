using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.ReachableNodesWithRestrictions;

// LeetCode 2368. Reachable Nodes With Restrictions: how many nodes of an undirected
// tree can still be reached from node 0 once the restricted nodes may not be entered?
//
// Deleting nodes from a tree only ever prunes whole subtrees - it can never reconnect
// two pieces that were not already joined - so the survivors still form a forest, and
// the question can be answered either by walking out from node 0 or by partitioning
// the surviving edges and asking which nodes share node 0's component. That is the
// same walk-versus-partition pair FindIfPathExistsInGraph draws for LC 1971, counting
// a component here instead of testing one pair.
internal static class ReachableNodesWithRestrictionsSolution
{
    // The endpoints' slots in LeetCode's own two-element edge array.
    private const int From = 0;
    private const int To = 1;

    // Every walk starts at node 0, which LeetCode guarantees is never restricted.
    private const int Root = 0;

    // The textbook answer: materialize an adjacency list and flood-fill out from node
    // 0 with an iterative depth-first search that simply refuses to step onto a
    // restricted node, counting what it visits. Entirely BCL collections - iterative
    // rather than recursive to stay overflow-safe on a path-shaped tree. It is the arm
    // the union-find strategy below has to justify itself against.
    public static int ReachableNodesByDepthFirstFloodFill(int nodeCount, int[][] edges, int[] restricted)
    {
        var restrictedSet = new Set<int>(restricted);

        return ReachableNodesByDepthFirstFloodFill(nodeCount, edges, restrictedSet);
    }

    public static int ReachableNodesByDepthFirstFloodFill(int nodeCount, int[][] edges, Set<int> restricted)
    {
        var adjacency = BuildAdjacency(nodeCount, edges);
        var visited = new bool[nodeCount];
        var stack = new Stack<int>();

        stack.Push(Root);
        visited[Root] = true;
        var reachable = 0;

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            reachable++;
            PushOpenNeighbors(adjacency[node], visited, restricted, stack);
        }

        return reachable;
    }

    private static List<int>[] BuildAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<int>[nodeCount];

        for (var node = 0; node < nodeCount; node++)
        {
            adjacency[node] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[From]].Add(edge[To]);
            adjacency[edge[To]].Add(edge[From]);
        }

        return adjacency;
    }

    private static void PushOpenNeighbors(
        List<int> neighbors, bool[] visited, Set<int> restricted, Stack<int> stack)
    {
        foreach (var next in neighbors)
        {
            if (!visited[next] && !restricted.Has(next))
            {
                visited[next] = true;
                stack.Push(next);
            }
        }
    }

    // This repo's own DisjointSet: union every edge whose endpoints are BOTH
    // unrestricted - the edges that survive the deletion - and node 0's component is
    // exactly its reachable set, so the answer is a count of unrestricted nodes
    // sharing its root, with no traversal and no adjacency copy at all (the same
    // union-per-edge shape FindIfPathExistsInGraph and NumberOfProvinces use).
    public static int ReachableNodesByDisjointSet(int nodeCount, int[][] edges, int[] restricted)
    {
        var restrictedSet = new Set<int>(restricted);

        return ReachableNodesByDisjointSet(nodeCount, edges, restrictedSet);
    }

    public static int ReachableNodesByDisjointSet(int nodeCount, int[][] edges, Set<int> restricted)
    {
        var components = new DisjointSet(nodeCount);

        foreach (var edge in edges)
        {
            if (!restricted.Has(edge[From]) && !restricted.Has(edge[To]))
            {
                components.Union(edge[From], edge[To]);
            }
        }

        var reachable = 0;

        for (var node = 0; node < nodeCount; node++)
        {
            if (!restricted.Has(node) && components.IsConnected(node, Root))
            {
                reachable++;
            }
        }

        return reachable;
    }
}
