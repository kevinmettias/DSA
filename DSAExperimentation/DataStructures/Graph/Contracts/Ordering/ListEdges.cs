namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

/// <summary>Presents a list of (data, target) pairs as one node's edges, in list order.</summary>
/// <param name="items">The list to present, read in place rather than copied.</param>
public readonly struct ListEdges<TNode, TEdgeData>(List<(TEdgeData Data, TNode Target)> items)
    : IEdges<TNode, TEdgeData>
{
    public int Count => items.Count;

    public (TEdgeData Data, TNode Target) Get(int index) => items[index];
}
