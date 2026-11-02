using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.Walking;

namespace DSAExperimentation.Algorithms.Connectivity;

// Proof of the multi-root Reduce.Graph overload: counting components needs one
// visited-set carried across several separate top-level walks - a node reached from
// an earlier root has to be skipped when it later turns up as its own root, or every
// node in a component gets double-counted as a component of its own. The single-root
// overloads can't express this, since each constructs its own guard fresh, internally,
// per call.
/// <summary>Counts, or reduces one by one, the connected components of an undirected graph stored as symmetric adjacency.</summary>
public static class ConnectedComponents
{
    // TOrderStrategy generic rather than hardcoded to depth-first: which order
    // Reduce.Graph explores a component in never changes the count, so the choice
    // costs nothing either way - proof that the multi-root overload underneath is
    // already just as BFS-compatible as any other Reduce.Graph call.
    /// <summary>Counts the components holding the nodes, in <c>TOrder</c>'s child order.</summary>
    /// <param name="nodes">
    /// The nodes whose components are counted, each component once however many of them it holds.
    /// </param>
    /// <returns>The number of distinct components, 0 when there are no nodes.</returns>
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
                TOrderStrategy, NoOpReduceAlgebra<TNode>, Unit>(node, default, visited);

            count++;
        }

        return count;
    }

    // In the topology's own child order.
    /// <summary>Counts the components holding the nodes, in the topology's own child order.</summary>
    /// <param name="nodes">
    /// The nodes whose components are counted, each component once however many of them it holds.
    /// </param>
    /// <returns>The number of distinct components, 0 when there are no nodes.</returns>
    public static int Count<TNode, TTopology, TChildren, TOrderStrategy>(
        IEnumerable<TNode> nodes)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        => Count<TNode, TTopology, TChildren, NaturalChildOrder<TNode, TChildren>, TChildren, TOrderStrategy>(nodes);

    // Each component reduced on its own, in the order the nodes first reach one: the walk Count
    // rides, with the caller's algebra in place of the no-op and every component starting from
    // the same seed - what a per-island area, total or flag needs. The seed is copied into each
    // walk, so a value seed keeps the components apart, and a seed holding a reference hands
    // every component the same object.
    /// <summary>Reduces every component the nodes reach, each from the same seed.</summary>
    /// <param name="nodes">The candidate roots, each starting a component unless an earlier walk reached it.</param>
    /// <param name="seed">The state each component's walk starts from.</param>
    /// <returns>Each component's reduced state, in the order its first node appears in <paramref name="nodes"/>.</returns>
    public static List<TState> ReduceEach<TNode, TTopology, TChildren, TOrderStrategy, TAlgebra, TState>(
        IEnumerable<TNode> nodes, TState seed)
        where TNode : class
        where TTopology : struct, IGraphTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrderStrategy : struct, IReduceOrderStrategy<TNode>
        where TAlgebra : struct, IReduceAlgebra<TNode, TState>
    {
        var visited = new HashSet<TNode>();
        var states = new List<TState>();

        foreach (var node in nodes)
        {
            if (!visited.Contains(node))
            {
                var component = Reduce.Graph<TNode, TTopology, TChildren, TOrderStrategy, TAlgebra, TState>(node, seed, visited);
                states.Add(component);
            }
        }

        return states;
    }

    // Marks a component's nodes as visited via the walk itself; the count comes
    // from how many times a fresh root was found above, not from anything this
    // algebra computes.
    private readonly struct NoOpReduceAlgebra<TNode> : IReduceAlgebra<TNode, Unit>
    {
        public static Unit Seed => default;
    }
}
