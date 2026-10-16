namespace DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

public readonly struct ListChildren<TNode>(List<TNode> items) : IChildren<TNode>
{
    public int Count => items.Count;

    public TNode Get(int index) => items[index];
}
