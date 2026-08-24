namespace DSAExperimentation.Graph.Contracts.Ordering;

// The edge-aware counterpart to IChildren: each entry carries a destination *and*
// data about the connection itself (a weight, a label, a capacity - whatever the
// algorithm needs attached to the transition rather than the node). Deliberately
// kept separate from IChildren rather than unifying them (with IChildren as the
// "TEdgeData = Unit" case) - every existing IChildren consumer would otherwise pay
// to unpack data it never asked for.
internal interface IEdges<TNode, TEdgeData>
{
    int Count { get; }

    (TEdgeData Data, TNode Target) Get(int index);
}
