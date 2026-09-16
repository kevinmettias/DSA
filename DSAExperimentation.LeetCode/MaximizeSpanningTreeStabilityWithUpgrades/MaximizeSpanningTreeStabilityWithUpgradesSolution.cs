using DisjointSetOperations = DSAExperimentation.DataStructures.DisjointSet.DisjointSet;

namespace DSAExperimentation.LeetCode.MaximizeSpanningTreeStabilityWithUpgrades;

// LeetCode 3600. Maximize Spanning Tree Stability with Upgrades: every must-edge
// has to appear in the spanning tree at its original strength (it can never be
// upgraded); every optional edge may be left out, used at its original strength,
// or upgraded once (doubling it) at a cost of one of the upgrades. The
// stability of a tree is the minimum strength among its own edges, and the answer
// is the maximum stability any valid spanning tree can reach.
//
// This is "maximize the minimum, under a budget" - the classic binary-search-on-
// the-answer shape. Feasible(X) asks: using every must-edge (whose own strength
// therefore has to already be >= X, since it can neither be dropped nor upgraded)
// plus as many optional edges as needed at >= X (free if already >= X, else
// upgraded for one of the upgrades if doubling clears X), can every node be
// connected? Free edges are always tried before upgrade-eligible ones - the same
// matroid property Kruskal itself relies on (MinimumSpanningTree.Kruskal's own doc
// comment) means the number of upgrade-eligible edges that end up actually
// connecting something is the same no matter which order they're tried in, so
// this greedy computes the true minimum upgrade count for X, not just *a*
// feasible count.
//
// A cycle confined entirely to must-edges is a separate, threshold-independent
// impossibility: a spanning TREE cannot contain a cycle, no matter how large the
// upgrade budget is or how low X is allowed to go, so it is checked once up front
// rather than folded into Feasible.
internal static class MaximizeSpanningTreeStabilityWithUpgradesSolution
{
    // The two questions the binary search probes its answer with, held once per arm
    // rather than re-created on every search: each is asked O(log(stability)) times
    // per call, so building its implementation inside the search would charge the
    // measurement for allocations the search itself never makes.
    private static readonly IMustEdgeCycleCheck ArrayCycleCheck = new ArrayMustEdgeCycleCheck();
    private static readonly IFeasibilityCheck ArrayFeasibility = new ArrayFeasibilityCheck();
    private static readonly IMustEdgeCycleCheck DisjointSetCycleCheck = new DisjointSetMustEdgeCycleCheck();
    private static readonly IFeasibilityCheck DisjointSetFeasibility = new DisjointSetFeasibilityCheck();

    // Baseline: a hand-rolled int[] union-find (path compression, no union-by-rank)
    // fronting the same binary search - "what you'd write without this repo"
    // (ARCHITECTURE.md 17.5).
    public static int MaxStabilityByArrayUnionFind(int nodeCount, int[][] edges, int upgrades)
    {
        var graph = StabilityGraph.Build(nodeCount, edges);
        return MaxStabilityByArrayUnionFind(graph, upgrades);
    }

    public static int MaxStabilityByArrayUnionFind(StabilityGraph graph, int upgrades) =>
        MaxStabilityByBinarySearch(graph, upgrades, ArrayCycleCheck, ArrayFeasibility);

    private static bool HasMustEdgeCycleByArray(StabilityGraph graph)
    {
        var parent = NewParents(graph.NodeCount);

        foreach (var (u, v, _) in graph.MustEdges)
        {
            if (!TryUnionByArray(parent, u, v))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsFeasibleByArray(StabilityGraph graph, int threshold, int upgrades)
    {
        if (HasEdgeBelowThreshold(graph, threshold))
        {
            return false;
        }

        var parent = NewParents(graph.NodeCount);
        UnionThresholdEdgesByArray(parent, graph, threshold);

        if (UpgradesNeededByArray(parent, graph, threshold) > upgrades)
        {
            return false;
        }

        return IsFullyConnectedByArray(parent, graph.NodeCount);
    }

    // Unions every edge the threshold already admits for free: the must-edges (each of
    // which is at least the threshold, or the check above would already have failed)
    // and the optional edges strong enough to need no upgrade.
    private static void UnionThresholdEdgesByArray(int[] parent, StabilityGraph graph, int threshold)
    {
        foreach (var (u, v, _) in graph.MustEdges)
        {
            TryUnionByArray(parent, u, v);
        }

        foreach (var (u, v, strength) in graph.OptionalEdges)
        {
            if (strength >= threshold)
            {
                TryUnionByArray(parent, u, v);
            }
        }
    }

    // Spends one of the upgrades on every eligible optional edge that still joins
    // two components, and reports how many that took.
    private static int UpgradesNeededByArray(int[] parent, StabilityGraph graph, int threshold)
    {
        var upgradesUsed = 0;

        foreach (var (u, v, strength) in graph.OptionalEdges)
        {
            if (IsUpgradeEligible(strength, threshold) && TryUnionByArray(parent, u, v))
            {
                upgradesUsed++;
            }
        }

        return upgradesUsed;
    }

    // One component spanning every node is exactly the spanning tree the threshold
    // needs, and every node sharing the first node's root says the same thing.
    private static bool IsFullyConnectedByArray(int[] parent, int nodeCount)
    {
        var root = FindByArray(parent, 0);

        for (var node = 1; node < nodeCount; node++)
        {
            if (FindByArray(parent, node) != root)
            {
                return false;
            }
        }

        return true;
    }

    private static int[] NewParents(int nodeCount)
    {
        var parent = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
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

    private static bool TryUnionByArray(int[] parent, int firstNode, int secondNode)
    {
        var (rootA, rootB) = (FindByArray(parent, firstNode), FindByArray(parent, secondNode));

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
    public static int MaxStabilityByDisjointSet(int nodeCount, int[][] edges, int upgrades)
    {
        var graph = StabilityGraph.Build(nodeCount, edges);
        return MaxStabilityByDisjointSet(graph, upgrades);
    }

    public static int MaxStabilityByDisjointSet(StabilityGraph graph, int upgrades) =>
        MaxStabilityByBinarySearch(graph, upgrades, DisjointSetCycleCheck, DisjointSetFeasibility);

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

    private static bool IsFeasibleByDisjointSet(StabilityGraph graph, int threshold, int upgrades)
    {
        if (HasEdgeBelowThreshold(graph, threshold))
        {
            return false;
        }

        var components = new DisjointSetOperations(graph.NodeCount);
        UnionThresholdEdgesByDisjointSet(components, graph, threshold);

        if (UpgradesNeededByDisjointSet(components, graph, threshold) > upgrades)
        {
            return false;
        }

        return IsFullyConnectedByDisjointSet(components, graph.NodeCount);
    }

    // The same free-edge union as the array arm, over this repo's own DisjointSet.
    private static void UnionThresholdEdgesByDisjointSet(
        DisjointSetOperations components, StabilityGraph graph, int threshold)
    {
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
    }

    private static int UpgradesNeededByDisjointSet(
        DisjointSetOperations components, StabilityGraph graph, int threshold)
    {
        var upgradesUsed = 0;

        foreach (var (u, v, strength) in graph.OptionalEdges)
        {
            if (IsUpgradeEligible(strength, threshold) && !components.IsConnected(u, v))
            {
                components.Union(u, v);
                upgradesUsed++;
            }
        }

        return upgradesUsed;
    }

    private static bool IsFullyConnectedByDisjointSet(DisjointSetOperations components, int nodeCount)
    {
        for (var node = 1; node < nodeCount; node++)
        {
            if (!components.IsConnected(0, node))
            {
                return false;
            }
        }

        return true;
    }

    // An optional edge is worth spending one of the upgrades on exactly when it is
    // too weak as-is but doubling it would clear the threshold.
    private static bool IsUpgradeEligible(int strength, int threshold) =>
        strength < threshold && 2 * strength >= threshold;

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

    // A must-edge below the threshold can neither be dropped nor upgraded, so its
    // mere presence rules the threshold out for both arms.
    private static bool HasEdgeBelowThreshold(StabilityGraph graph, int threshold) =>
        graph.MustEdges.Exists(edge => edge.Strength < threshold);

    // Both arms front the identical maximize-the-minimum binary search - same
    // must-edge-cycle guard, same ceiling, same midpoint rounding, same early
    // infeasibility answer - and differ only in the union-find the feasibility
    // questions are asked of. Those two questions are therefore what arrives as
    // parameters, and the search itself is written once rather than per arm.
    private static int MaxStabilityByBinarySearch(
        StabilityGraph graph,
        int upgrades,
        IMustEdgeCycleCheck cycleCheck,
        IFeasibilityCheck feasibility)
    {
        if (cycleCheck.Exists(graph) || !feasibility.Holds(graph, 1, upgrades))
        {
            return LeetCodeAnswer.None;
        }

        var (low, high) = (1, UpperBound(graph));

        while (low < high)
        {
            var mid = low + ((high - low + 1) / 2);

            if (feasibility.Holds(graph, mid, upgrades))
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

    // Whether the must-edges alone already contain a cycle, which no spanning tree
    // can contain however many upgrades are available - the threshold-independent
    // half of the search's guard.
    private interface IMustEdgeCycleCheck
    {
        bool Exists(StabilityGraph graph);
    }

    // Whether every node can be connected using all the must-edges plus optional
    // edges that are either already at `threshold` or worth one of `upgrades`
    // doublings - the threshold-dependent half of the guard, and the probe the search
    // binary-searches with.
    private interface IFeasibilityCheck
    {
        bool Holds(StabilityGraph graph, int threshold, int upgrades);
    }

    // The baseline arm's answers: both front the same hand-rolled int[] union-find.
    private sealed class ArrayMustEdgeCycleCheck : IMustEdgeCycleCheck
    {
        public bool Exists(StabilityGraph graph) => HasMustEdgeCycleByArray(graph);
    }

    private sealed class ArrayFeasibilityCheck : IFeasibilityCheck
    {
        public bool Holds(StabilityGraph graph, int threshold, int upgrades) =>
            IsFeasibleByArray(graph, threshold, upgrades);
    }

    // The composed arm's answers: both front this repo's own DisjointSet.
    private sealed class DisjointSetMustEdgeCycleCheck : IMustEdgeCycleCheck
    {
        public bool Exists(StabilityGraph graph) => HasMustEdgeCycleByDisjointSet(graph);
    }

    private sealed class DisjointSetFeasibilityCheck : IFeasibilityCheck
    {
        public bool Holds(StabilityGraph graph, int threshold, int upgrades) =>
            IsFeasibleByDisjointSet(graph, threshold, upgrades);
    }
}
