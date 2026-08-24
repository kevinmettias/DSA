using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;
using DSAExperimentation.Graph.Engines.Reducing;
using DSAExperimentation.Graph.Engines.Walking;

namespace DSAExperimentation.Graph.Algorithms.Connectivity;

// Proof of the multi-root Reduce.Graph overload: counting components needs one
// visited-set carried across several separate top-level walks - a node reached from
// an earlier root has to be skipped when it later turns up as its own root, or every
// node in a component gets double-counted as a component of its own. The single-root
// overloads can't express this, since each constructs its own guard fresh, internally,
// per call.
internal static class ConnectedComponents
{
    // TOrderStrategy generic rather than hardcoded to depth-first: which order
    // Reduce.Graph explores a component in never changes the count, so the choice
    // costs nothing either way - proof that the multi-root overload underneath is
    // already just as BFS-compatible as any other Reduce.Graph call.
    public static int Count<TNode, TTopology, TChildren, TOrder, TOrderedChildren, TOrderStrategy>(
        IEnumerable<TNode> nodes)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
    {
        var visited = new HashSet<TNode>();
        var count = 0;

        foreach (var node in nodes)
        {
            if (visited.Contains(node))
            {
                continue;
            }

            Reduce.Graph<
                TNode, TTopology, TChildren, TOrder, TOrderedChildren,
                TOrderStrategy, NoOpReduceAlgebra<TNode>, Unit>(node, visited);

            count++;
        }

        return count;
    }

    // Marks a component's nodes as visited via the walk itself; the count comes
    // from how many times a fresh root was found above, not from anything this
    // algebra computes.
    private readonly struct NoOpReduceAlgebra<TNode> : IReduceAlgebra<TNode, Unit>
    {
        public static Unit Seed => default;
    }
}
