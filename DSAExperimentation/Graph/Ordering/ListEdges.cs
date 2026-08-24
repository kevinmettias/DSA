namespace DSAExperimentation.Graph;

public readonly struct ListEdges<TNode, TEdgeData>(List<(TEdgeData Data, TNode Target)> items)
    : IEdges<TNode, TEdgeData>
{
    public int Count => items.Count;

    public (TEdgeData Data, TNode Target) this[int index] => items[index];
}
