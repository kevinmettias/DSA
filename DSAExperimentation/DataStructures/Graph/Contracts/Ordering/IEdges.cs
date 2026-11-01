namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

// The edge-aware counterpart to IChildren: each entry carries a destination *and*
// data about the connection itself (a weight, a label, a capacity - whatever the
// algorithm needs attached to the transition rather than the node). Deliberately
// kept separate from IChildren rather than unifying them (with IChildren as the
// "TEdgeData = Unit" case) - every existing IChildren consumer would otherwise pay
// to unpack data it never asked for.
/// <summary>An indexed view over one node's edges, each a target node plus data such as a weight.</summary>
public interface IEdges<TNode, TEdgeData>
{
    /// <summary>The number of edges leaving the node.</summary>
    int Count { get; }

    /// <summary>Returns the edge at a 0-based position below <see cref="Count"/>.</summary>
    /// <param name="index">The position, from 0 to <see cref="Count"/> - 1.</param>
    /// <returns>The edge's data and the node it leads to.</returns>
    (TEdgeData Data, TNode Target) Get(int index);
}
