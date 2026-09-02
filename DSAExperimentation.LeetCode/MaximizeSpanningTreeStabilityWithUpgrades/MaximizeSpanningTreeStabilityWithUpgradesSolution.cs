using DisjointSetOperations = DSAExperimentation.DataStructures.DisjointSet.DisjointSet;

namespace DSAExperimentation.LeetCode.MaximizeSpanningTreeStabilityWithUpgrades;

// LeetCode 3600. Maximize Spanning Tree Stability with Upgrades: every must-edge
// has to appear in the spanning tree at its original strength (it can never be
// upgraded); every optional edge may be left out, used at its original strength,
// or upgraded once (doubling it) at a cost of one of the k upgrades. The
// stability of a tree is the minimum strength among its own edges, and the answer
// is the maximum stability any valid spanning tree can reach.
//
// This is "maximize the minimum, under a budget" - the classic binary-search-on-
// the-answer shape. Feasible(X) asks: using every must-edge (whose own strength
// therefore has to already be >= X, since it can neither be dropped nor upgraded)
// plus as many optional edges as needed at >= X (free if already >= X, else
// upgraded for one of the k upgrades if doubling clears X), can every node be
// connected? Free edges are always tried before upgrade-eligible ones - the same
// matroid property Kruskal itself relies on (MinimumSpanningTree.Kruskal's own doc
// comment) means the number of upgrade-eligible edges that end up actually
// connecting something is the same no matter which order they're tried in, so
// this greedy computes the true minimum upgrade count for X, not just *a*
// feasible count.
//
// A cycle confined entirely to must-edges is a separate, threshold-independent
// impossibility: a spanning TREE cannot contain a cycle, no matter how large k is
// or how low X is allowed to go, so it is checked once up front rather than
// folded into Feasible.
internal static class MaximizeSpanningTreeStabilityWithUpgradesSolution
{
    // Baseline: a hand-rolled int[] union-find (path compression, no union-by-rank)
    // fronting the same binary search - "what you'd write without this repo"
    // (ARCHITECTURE.md 17.5).
    public static int MaxStabilityByArrayUnionFind(int n, int[][] edges, int k) =>
        MaxStabilityByArrayUnionFind(StabilityGraph.Build(n, edges), k);

    public static int MaxStabilityByArrayUnionFind(StabilityGraph graph, int k)
    {
        if (HasMustEdgeCycleByArray(graph) || !IsFeasibleByArray(graph, threshold: 1, k))
        {
            return LeetCodeAnswer.None;
        }

        var (low, high) = (1, UpperBound(graph));

        while (low < high)
        {
            var mid = low + ((high - low + 1) / 2);

            if (IsFeasibleByArray(graph, mid, k))
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        return low;
    }

    private static bool HasMustEdgeCycleByArray(StabilityGraph graph)
    {
        var parent = NewParents(graph.NodeCount);

        foreach (var (u, v, _) in graph.MustEdges)
        {
            if (!UnionByArray(parent, u, v))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsFeasibleByArray(StabilityGraph graph, int threshold, int k)
    {
        if (graph.MustEdges.Exists(edge => edge.Strength < threshold))
        {
            return false;
        }

        var parent = NewParents(graph.NodeCount);

        foreach (var (u, v, _) in graph.MustEdges)
        {
            UnionByArray(parent, u, v);
        }

        foreach (var (u, v, strength) in graph.OptionalEdges)
        {
            if (strength >= threshold)
            {
                UnionByArray(parent, u, v);
            }
        }

        var upgradesUsed = 0;

        foreach (var (u, v, strength) in graph.OptionalEdges)
        {
            if (strength < threshold && 2 * strength >= threshold && UnionByArray(parent, u, v))
            {
                upgradesUsed++;
            }
        }

        if (upgradesUsed > k)
        {
            return false;
        }

        var root = FindByArray(parent, 0);

        for (var node = 1; node < graph.NodeCount; node++)
        {
            if (FindByArray(parent, node) != root)
            {
                return false;
            }
        }

        return true;
    }

    private static int[] NewParents(int n)
    {
        var parent = new int[n];

        for (var i = 0; i < n; i++)
        {
            parent[i] = i;
        }

        return parent;
    }

    private static int FindByArray(int[] parent, int node)
    {
        while (parent[node] != node)
        {
            parent[node] = parent[parent[node]];
            node = parent[node];
        }

        return node;
    }

    private static bool UnionByArray(int[] parent, int a, int b)
    {
        var (rootA, rootB) = (FindByArray(parent, a), FindByArray(parent, b));

        if (rootA == rootB)
        {
            return false;
        }

        parent[rootA] = rootB;
        return true;
    }

    // Composed: DataStructures.DisjointSet.DisjointSet (path compression AND
    // union-by-rank) fronting the identical binary search - LC's vertices are
    // already the dense [0, n) ids this repo's DisjointSet expects, the same
    // direct-use shape WalkCostComponents documents for LC 3108.
    public static int MaxStabilityByDisjointSet(int n, int[][] edges, int k) =>
        MaxStabilityByDisjointSet(StabilityGraph.Build(n, edges), k);

    public static int MaxStabilityByDisjointSet(StabilityGraph graph, int k)
    {
        if (HasMustEdgeCycleByDisjointSet(graph) || !IsFeasibleByDisjointSet(graph, threshold: 1, k))
        {
            return LeetCodeAnswer.None;
        }

        var (low, high) = (1, UpperBound(graph));

        while (low < high)
        {
            var mid = low + ((high - low + 1) / 2);

            if (IsFeasibleByDisjointSet(graph, mid, k))
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        return low;
    }

    private static bool HasMustEdgeCycleByDisjointSet(StabilityGraph graph)
    {
        var components = new DisjointSetOperations(graph.NodeCount);

        foreach (var (u, v, _) in graph.MustEdges)
        {
            if (components.IsConnected(u, v))
            {
                return true;
            }

            components.Union(u, v);
        }

        return false;
    }

    private static bool IsFeasibleByDisjointSet(StabilityGraph graph, int threshold, int k)
    {
        if (graph.MustEdges.Exists(edge => edge.Strength < threshold))
        {
            return false;
        }

        var components = new DisjointSetOperations(graph.NodeCount);

        foreach (var (u, v, _) in graph.MustEdges)
        {
            components.Union(u, v);
        }

        foreach (var (u, v, strength) in graph.OptionalEdges)
        {
            if (strength >= threshold)
            {
                components.Union(u, v);
            }
        }

        var upgradesUsed = 0;

        foreach (var (u, v, strength) in graph.OptionalEdges)
        {
            if (strength < threshold && 2 * strength >= threshold && !components.IsConnected(u, v))
            {
                components.Union(u, v);
                upgradesUsed++;
            }
        }

        if (upgradesUsed > k)
        {
            return false;
        }

        for (var node = 1; node < graph.NodeCount; node++)
        {
            if (!components.IsConnected(0, node))
            {
                return false;
            }
        }

        return true;
    }

    // Any value at least as large as the true answer works as a binary-search
    // ceiling: a must-edge's own strength caps it directly (it can never be
    // upgraded), an optional edge's doubled strength caps whatever it could
    // contribute upgraded.
    private static int UpperBound(StabilityGraph graph)
    {
        var upper = 1;

        foreach (var edge in graph.MustEdges)
        {
            upper = Math.Max(upper, edge.Strength);
        }

        foreach (var edge in graph.OptionalEdges)
        {
            upper = Math.Max(upper, 2 * edge.Strength);
        }

        return upper;
    }
}
