namespace DSAExperimentation.Graph;

public readonly struct ListChildren<TNode>(List<TNode> items) : IChildren<TNode>
{
    public int Count => items.Count;

    public TNode this[int index] => items[index];
}
