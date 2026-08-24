namespace DSAExperimentation.Graph;

// Forgets the edge data, keeping only the destinations - lets anything with edges
// be handed to every existing IChildren-based algorithm (GraphReduce, TreeFold,
// ...) for free, with no second representation to keep in sync.
public readonly struct EdgeTargets<TNode, TEdgeData, TEdges>(TEdges edges) : IChildren<TNode>
    where TEdges : struct, IEdges<TNode, TEdgeData>
{
    public int Count => edges.Count;

    public TNode this[int index] => edges[index].Target;
}
