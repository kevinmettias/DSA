namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

// Orthogonal to visit-timing: this only decides the sequence a node's own children
// are walked in. A node's *structural* order comes from the topology itself
// (GetChildren yields them in whatever order the node type defines - a named
// Left/Middle/Right field, or a list); IChildOrder is an optional override for
// walking that same structure differently (reverse, priority-sorted) without
// touching the topology.
/// <summary>Chooses the order a walk takes one node's children in, leaving the topology unchanged.</summary>
public interface IChildOrder<TNode, TChildren, TOrderedChildren>
    where TChildren : struct, IChildren<TNode>
    where TOrderedChildren : struct, IChildren<TNode>
{
    /// <summary>Rearranges one node's children into this order, keeping exactly the same children.</summary>
    /// <param name="children">The node's children in the order its topology gives them.</param>
    /// <returns>A view of the same children in this order.</returns>
    static abstract TOrderedChildren Apply(TChildren children);
}
