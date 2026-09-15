using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.IncrementalEvenWeightedCycleQueries;

// LeetCode 3887. Incremental Even-Weighted Cycle Queries: process (u, v, w) edges
// one at a time into an initially empty n-node graph, keeping an edge only if
// every cycle in the resulting graph still sums to an even total weight. Return
// how many edges survive.
//
// The textbook parity union-find for this problem needs a disjoint-set that
// tracks an XOR-to-root value updated during path compression itself - a
// capability DataStructures.DisjointSet deliberately does not expose (Find/Union
// have no hook for auxiliary per-node state, and KeyedDisjointSet's own doc
// comment explains why composing DisjointSetForest directly to fake one would
// re-derive path compression by hand instead of reusing it - the exact "compose,
// don't redesign" line this problem would otherwise cross). Both strategies here
// instead track the graph's actual accepted edges and answer "what is the parity
// of some u-v path" with a real BFS over them - always well-defined once
// same-component, because every cycle already accepted has even weight, so any
// two u-v paths through the accepted graph agree on parity. What distinguishes
// the strategies is only whether a different-component pair - which can never
// already hold a cycle, so is always safe to add - gets to skip that BFS.
internal static class IncrementalEvenWeightedCycleQueriesSolution
{
    // Textbook baseline: BFS the accepted-edges adjacency from scratch for every
    // incoming edge, whether or not the endpoints could possibly be connected yet
    // - deliberately without this repo's DisjointSet, the arm the pruned strategy
    // below has to justify itself against.
    public static int CountAddedEdgesByBruteForceBfs(int n, int[][] edges)
    {
        var adjacency = BuildEmptyAdjacency(n);
        var addedCount = 0;

        foreach (var edge in edges)
        {
            var (u, v, w) = (edge[0], edge[1], edge[2]);

            if (TryFindParity(adjacency, u, v, out var parity) && parity != w)
            {
                continue;
            }

            AddEdge(adjacency, u, v, w);
            addedCount++;
        }

        return addedCount;
    }

    // This repo's own DisjointSet answers "could u and v possibly already share a
    // cycle" in O(a(n)): two different components never do, so the edge is always
    // safe to add without ever walking the graph. Only a same-component pair -
    // the sole case that can close a cycle - falls back to the same BFS parity
    // walk the baseline always runs.
    public static int CountAddedEdgesByDisjointSetPrunedBfs(int n, int[][] edges)
    {
        var adjacency = BuildEmptyAdjacency(n);
        var components = new DisjointSet(n);
        var addedCount = 0;

        foreach (var edge in edges)
        {
            if (TryAcceptPrunedEdge(adjacency, components, edge))
            {
                addedCount++;
            }
        }

        return addedCount;
    }

    // Accept one incoming edge unless it would close an odd-weight cycle. Only a pair
    // the disjoint-set already joins can close one, so only that pair pays for the
    // BFS parity walk.
    private static bool TryAcceptPrunedEdge(
        List<(int Neighbor, int Weight)>[] adjacency, DisjointSet components, int[] edge)
    {
        var (u, v, w) = (edge[0], edge[1], edge[2]);
        var sameComponent = components.IsConnected(u, v);

        if (sameComponent && HasConflictingParity(adjacency, u, v, w))
        {
            return false;
        }

        AddEdge(adjacency, u, v, w);

        if (!sameComponent)
        {
            components.Union(u, v);
        }

        return true;
    }

    // The accepted graph already reaches target from source at a parity that
    // disagrees with weight, so adding this edge would close an odd-weight cycle.
    // Unreached target means no parity at all, which the walk reports as false and
    // which never conflicts.
    private static bool HasConflictingParity(
        List<(int Neighbor, int Weight)>[] adjacency, int source, int target, int weight) =>
        TryFindParity(adjacency, source, target, out var parity) && parity != weight;

    // BFS from source over the accepted-edges adjacency; parity is the running
    // XOR of edge weights from source to each reached node ("sum is even" and
    // "XOR is 0" agree exactly because every weight is 0 or 1). False (parity
    // meaningless) when target is not reached at all - the edge cannot close any
    // cycle in that case, so it is always safe to add.
    private static bool TryFindParity(
        List<(int Neighbor, int Weight)>[] adjacency, int source, int target, out int parity)
    {
        var parityToSource = new Dictionary<int, int> { [source] = 0 };
        var frontier = new Queue<int>();
        frontier.Enqueue(source);

        while (frontier.Count > 0)
        {
            var node = frontier.Dequeue();

            if (node == target)
            {
                parity = parityToSource[node];
                return true;
            }

            ExpandParityNeighbors(adjacency, node, parityToSource, frontier);
        }

        parity = default;
        return false;
    }

    // Give every unreached neighbour of node its parity one edge further out, and
    // queue it: BFS discovers each node at the closest source parity, which is the
    // only parity it can ever report.
    private static void ExpandParityNeighbors(
        List<(int Neighbor, int Weight)>[] adjacency,
        int node,
        Dictionary<int, int> parityToSource,
        Queue<int> frontier)
    {
        foreach (var (neighbor, weight) in adjacency[node])
        {
            if (parityToSource.ContainsKey(neighbor))
            {
                continue;
            }

            parityToSource[neighbor] = parityToSource[node] ^ weight;
            frontier.Enqueue(neighbor);
        }
    }

    private static List<(int Neighbor, int Weight)>[] BuildEmptyAdjacency(int n)
    {
        var adjacency = new List<(int Neighbor, int Weight)>[n];

        for (var node = 0; node < n; node++)
        {
            adjacency[node] = [];
        }

        return adjacency;
    }

    private static void AddEdge(List<(int Neighbor, int Weight)>[] adjacency, int u, int v, int w)
    {
        adjacency[u].Add((v, w));
        adjacency[v].Add((u, w));
    }
}
