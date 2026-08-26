using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Algorithms.TopologicalSort;

// Kahn's algorithm: repeatedly peel off nodes with no remaining incoming edges. Only
// IGraphTopology is required, not IDagTopology's acyclicity promise, because the
// leftover-in-degree check below *is* the cycle check - the same law CheckedFold already
// resolved this way (see its own doc comment): the caller reached for the untrusted tier
// specifically because it can't vouch for acyclicity, so this has to find out at runtime
// rather than trust a promise. Unlike CheckedFold's recursion-path HashSet, no separate
// cycle-tracking structure is needed here - a cycle announces itself for free as nodes
// whose in-degree never reaches zero.
//
// nodes must enumerate every vertex in the graph, not merely its sources - unlike a
// root-seeded walk (Reduce.Graph, ConnectedComponents.Count) that discovers descendants
// transitively and only needs one representative per component, Kahn's needs each
// vertex's true in-degree before the first node is ever dequeued. There is no
// reverse-adjacency contract anywhere in Graph/Contracts, so nothing here could discover a
// missing ancestor even if it wanted to - this precondition is unenforced, the same shape
// as BinarySearch's sortedness. The two violation directions fail differently, though:
// omitting a descendant self-detects (it's still discovered via its parent's edge,
// inflating sorted past vertices.Count, so TrySort correctly returns false); omitting an
// ancestor does not (nothing forward-reachable from the supplied set points back to it),
// so TrySort returns true with an ordering that silently omits it. A false result
// therefore also doesn't distinguish a true cycle from a caller-omitted descendant - both
// present identically as leftover in-degree.
internal static class TopologicalSort
{
    public static bool TrySort<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        IEnumerable<TNode> nodes, out List<TNode> ordering)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        // Distinct() guards more than a frontier double-enqueue: without it, a repeated
        // input node inflates every child's in-degree once per occurrence in the pass
        // below, and the same repeated node then drains that inflation back out via one
        // decrement per occurrence during the walk - producing a silently duplicated
        // ordering, not a caught failure.
        var vertices = nodes.Distinct().ToList();
        var inDegree = BuildInDegree<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(vertices);

        var frontier = new Queue<TNode>(vertices.Where(vertex => inDegree[vertex] == 0));
        var sorted = new List<TNode>(vertices.Count);

        while (frontier.Count > 0)
        {
            var node = frontier.Dequeue();
            sorted.Add(node);

            ReleaseChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(node, inDegree, frontier);
        }

        ordering = sorted;
        return sorted.Count == vertices.Count;
    }

    private static Dictionary<TNode, int> BuildInDegree<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        List<TNode> vertices)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var inDegree = new Dictionary<TNode, int>();

        foreach (var vertex in vertices)
        {
            inDegree.TryAdd(vertex, 0);

            var children = TOrder.Apply(TTopology.GetChildren(vertex));
            for (var i = 0; i < children.Count; i++)
            {
                var child = children.Get(i);
                inDegree[child] = inDegree.GetValueOrDefault(child) + 1;
            }
        }

        return inDegree;
    }

    // GetValueOrDefault, not the raw indexer: a child reachable only through a vertex
    // the caller omitted from `nodes` never got a dictionary entry from BuildInDegree,
    // and this still has to degrade to an incomplete result rather than throw - the
    // same "wrong answer, never an exception" shape every other unenforced precondition
    // in this repo has.
    private static void ReleaseChildren<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(
        TNode node, Dictionary<TNode, int> inDegree, Queue<TNode> frontier)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var children = TOrder.Apply(TTopology.GetChildren(node));
        for (var i = 0; i < children.Count; i++)
        {
            var child = children.Get(i);
            var remaining = inDegree.GetValueOrDefault(child) - 1;
            inDegree[child] = remaining;

            if (remaining == 0)
            {
                frontier.Enqueue(child);
            }
        }
    }
}
