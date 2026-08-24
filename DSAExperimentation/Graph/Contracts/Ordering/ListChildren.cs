namespace DSAExperimentation.Graph.Contracts.Ordering;

internal readonly struct ListChildren<TNode>(List<TNode> items) : IChildren<TNode>
{
    public int Count => items.Count;

    public TNode Get(int index) => items[index];
}
