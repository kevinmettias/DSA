using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.Connectivity;

// Bridges and articulation points both fall out of the same discoveryIndex/lowLink DFS
// StronglyConnectedComponents.cs uses, just over undirected (symmetric) adjacency instead of
// directed - IGraphTopology has no directed/undirected distinction, so "undirected" here means
// the same convention MinimumSpanningTree.cs documents for IEdgeTopology: the caller stores
// each undirected edge as two directed child-edges, one from each endpoint.
//
// One traversal for both, the same reasoning TreeMetrics.HeightAndSize gives for its own
// combined entry point: an edge (u, child) is a bridge when low[child] > disc[u] (nothing in
// child's subtree reaches back to u or earlier), and u is an articulation point when some
// child has low[child] >= disc[u] (u itself, non-root) or u is the DFS root with 2+ DFS-tree
// children - both conditions read the literal same disc/low values produced by the literal
// same recursion, so splitting this into two independently-recursing public methods would
// duplicate correctness-critical low-link math for no savings, since the DFS cost is identical
// either way.
//
// The "don't walk back along the edge just arrived on" parent skip is handled for real, not
// documented away as an unenforced precondition: a symmetric edge back to the immediate parent
// is control flow this DFS actively re-derives every step, so each call tracks whether its one
// legitimate parent-edge skip has already been used - a *second* occurrence of the same parent
// node (a parallel edge) is treated as a genuine back edge instead of being silently ignored,
// the standard multigraph-safe low-link technique.
//
// The four pieces of per-call mutable state (discoveryIndex, lowLink, bridges,
// articulationPoints) are bundled into one record rather than threaded as separate parameters,
// the same shape StronglyConnectedComponents.cs's own TarjanState uses and for the same
// reason - it keeps every recursive helper under this gate's check-parameter-count limit.
internal static class BridgesAndArticulationPoints
{
    private const int MinimumRootChildrenForArticulation = 2;

    public static (List<(TNode A, TNode B)> Bridges, List<TNode> ArticulationPoints) Find<
        TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        IEnumerable<TNode> nodes)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var state = new LowLinkState<TNode>(new(), new(), new(), new());

        foreach (var node in nodes)
        {
            if (!state.DiscoveryIndex.ContainsKey(node))
            {
                Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, null, state);
            }
        }

        return (state.Bridges, [.. state.ArticulationPoints]);
    }

    private static void Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, TNode? parent, LowLinkState<TNode> state)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var index = state.DiscoveryIndex.Count;
        state.DiscoveryIndex[node] = index;
        state.LowLink[node] = index;

        var dfsTreeChildCount = VisitChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
            node, parent, state);

        if (parent is null && dfsTreeChildCount >= MinimumRootChildrenForArticulation)
        {
            state.ArticulationPoints.Add(node);
        }
    }

    private static int VisitChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, TNode? parent, LowLinkState<TNode> state)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var dfsTreeChildCount = 0;
        var skippedParentEdge = false;

        var children = TOrder.Apply(TTopology.GetChildren(node));
        for (var i = 0; i < children.Count; i++)
        {
            var child = children.Get(i);

            if (ReferenceEquals(child, parent) && !skippedParentEdge)
            {
                skippedParentEdge = true;
                continue;
            }

            if (IsDfsTreeChild<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, parent, child, state))
            {
                dfsTreeChildCount++;
            }
        }

        return dfsTreeChildCount;
    }

    // Returns whether child was an undiscovered DFS-tree child (as opposed to a back edge to
    // an already-visited node), so the caller's dfsTreeChildCount stays accurate for the
    // root-articulation-point check.
    private static bool IsDfsTreeChild<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, TNode? parent, TNode child, LowLinkState<TNode> state)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        if (state.DiscoveryIndex.TryGetValue(child, out var childIndex))
        {
            state.LowLink[node] = Math.Min(state.LowLink[node], childIndex);
            return false;
        }

        var index = state.DiscoveryIndex[node];
        VisitTreeEdge<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, child, state);

        if (parent is not null && state.LowLink[child] >= index)
        {
            state.ArticulationPoints.Add(node);
        }

        return true;
    }

    private static void VisitTreeEdge<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, TNode child, LowLinkState<TNode> state)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        Visit<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(child, node, state);
        state.LowLink[node] = Math.Min(state.LowLink[node], state.LowLink[child]);

        if (state.LowLink[child] > state.DiscoveryIndex[node])
        {
            state.Bridges.Add((node, child));
        }
    }

    private sealed record LowLinkState<TNode>(
        Dictionary<TNode, int> DiscoveryIndex,
        Dictionary<TNode, int> LowLink,
        List<(TNode A, TNode B)> Bridges,
        HashSet<TNode> ArticulationPoints)
        where TNode : class;
}
