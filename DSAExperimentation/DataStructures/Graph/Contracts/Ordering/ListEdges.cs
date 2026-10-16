namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

public readonly struct ListEdges<TNode, TEdgeData>(List<(TEdgeData Data, TNode Target)> items)
    : IEdges<TNode, TEdgeData>
{
    public int Count => items.Count;

    public (TEdgeData Data, TNode Target) Get(int index) => items[index];
}
