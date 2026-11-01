namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

/// <summary>Presents a list of nodes as one node's children, in list order.</summary>
/// <param name="items">The list to present, read in place rather than copied.</param>
public readonly struct ListChildren<TNode>(List<TNode> items) : IChildren<TNode>
{
    public int Count => items.Count;

    public TNode Get(int index) => items[index];
}
